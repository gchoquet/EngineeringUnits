using System;
using System.Globalization;

namespace EngineeringUnits
{
    /// <summary>
    /// Abstract base class for all physical-quantity types. Carries a value expressed
    /// in canonical SI base units, a display unit, and significant-figure metadata.
    /// </summary>
    /// <remarks>
    /// Instances are immutable. Subclasses ( <see cref="Length"/>, <see cref="Mass"/>,
    /// <see cref="Time"/>, <see cref="Temperature"/>, …) represent specific dimensions
    /// and carry their own typed arithmetic operator overloads.
    /// <para>
    /// Internally every quantity is stored in canonical SI base units, with the original
    /// display unit retained for formatting and for the left-operand-wins rule on
    /// arithmetic results (see specification §14.4).
    /// </para>
    /// </remarks>
    public abstract class EngineeringUnit : IFormattable, IComparable<EngineeringUnit>, IEquatable<EngineeringUnit>
    {
        /// <summary>The value expressed in canonical SI base units.</summary>
        public double CanonicalValue { get; }

        /// <summary>The unit used for display and as the result-unit when this is the left operand of arithmetic.</summary>
        public Unit DisplayUnit { get; }

        /// <summary>The dimension signature, identifying this quantity's physical dimension.</summary>
        public DimensionSignature Dimension => DisplayUnit.Dimension;

        /// <summary>Significant figures used for formatted display. Default is 4.</summary>
        public byte Precision { get; init; } = 4;

        /// <summary>The value expressed in <see cref="DisplayUnit"/>.</summary>
        public double Value => DisplayUnit.FromCanonical(CanonicalValue);

        /// <summary>
        /// Constructs a quantity from a value expressed in the given display unit.
        /// The value is immediately converted to canonical SI for storage.
        /// </summary>
        protected EngineeringUnit(double value, Unit displayUnit)
        {
            DisplayUnit = displayUnit;
            CanonicalValue = displayUnit.ToCanonical(value);
        }

        /// <summary>
        /// Constructs a quantity from a canonical SI value plus a display unit.
        /// Used internally by arithmetic to avoid double-converting.
        /// </summary>
        private protected EngineeringUnit(Unit displayUnit, double canonicalValue)
        {
            DisplayUnit = displayUnit;
            CanonicalValue = canonicalValue;
        }

        /// <summary>Returns the value expressed in the named unit.</summary>
        /// <param name="unit">Target unit symbol (must have the same dimension as this quantity).</param>
        /// <returns>The numeric value in the target unit.</returns>
        /// <exception cref="UnknownUnitException">If <paramref name="unit"/> is not registered.</exception>
        /// <exception cref="DimensionMismatchException">If <paramref name="unit"/> has a different dimension.</exception>
        public double As(string unit)
        {
            var u = UnitCatalog.Get(unit);
            if (u.Dimension != Dimension)
                throw new DimensionMismatchException(Dimension, u.Dimension);
            return u.FromCanonical(CanonicalValue);
        }

        /// <summary>
        /// Default culture-neutral string form: value (in <see cref="DisplayUnit"/>, rounded
        /// to <see cref="Precision"/> significant figures) followed by the unit symbol.
        /// </summary>
        public override string ToString() => ToString(null, CultureInfo.InvariantCulture);

        /// <inheritdoc cref="IFormattable.ToString(string, IFormatProvider)" />
        public string ToString(string? format) => ToString(format, CultureInfo.InvariantCulture);

        /// <summary>
        /// Formats this quantity. Supported format codes (see specification §8.2 and §14.17):
        /// <list type="bullet">
        ///   <item><term>G or null</term><description>General default — short symbol.</description></item>
        ///   <item><term>L</term><description>Long unit name (e.g. <c>"feet"</c>).</description></item>
        ///   <item><term>D</term><description>Dual — primary value plus SI or US-customary equivalent in parens.</description></item>
        ///   <item><term>S</term><description>SI canonical (from <see cref="UnitPreferences.SIScientific"/>).</description></item>
        ///   <item><term>U</term><description>US-customary canonical (from <see cref="UnitPreferences.UsCustomary"/>).</description></item>
        ///   <item><term>P</term><description>Preferred (from <see cref="UnitPreferences.Default"/>).</description></item>
        ///   <item><term>E</term><description>Scientific notation.</description></item>
        ///   <item><term>N{n}</term><description>Force <c>n</c> significant figures, e.g. <c>"N6"</c>.</description></item>
        ///   <item><term>{unit}</term><description>Force a specific unit, e.g. <c>"{m}"</c>.</description></item>
        /// </list>
        /// </summary>
        public virtual string ToString(string? format, IFormatProvider? formatProvider)
        {
            formatProvider ??= CultureInfo.InvariantCulture;

            if (string.IsNullOrEmpty(format) || format == "G")
                return FormatWithUnit(Value, Precision, DisplayUnit, useLongName: false, formatProvider);

            if (format == "L")
                return FormatWithUnit(Value, Precision, DisplayUnit, useLongName: true, formatProvider);

            if (format == "S")
                return FormatInProfileUnit(UnitPreferences.SIScientific, formatProvider);

            if (format == "U")
                return FormatInProfileUnit(UnitPreferences.UsCustomary, formatProvider);

            if (format == "P")
                return FormatInProfileUnit(UnitPreferences.Default, formatProvider);

            if (format == "D")
                return ToDualString();

            if (format == "E")
            {
                var s = Value.ToString("E" + Math.Max(0, Precision - 1), formatProvider);
                var sym = DisplayUnit.Symbol;
                return string.IsNullOrEmpty(sym) ? s : $"{s} {sym}";
            }

            // N{n} — force n sig figs
            if (format!.Length > 1 && format[0] == 'N' && int.TryParse(format.Substring(1), out var sigFigs))
                return FormatWithUnit(Value, (byte)Math.Max(1, Math.Min(15, sigFigs)), DisplayUnit, useLongName: false, formatProvider);

            // {unit} — force a specific unit
            if (format.Length >= 2 && format[0] == '{' && format[format.Length - 1] == '}')
            {
                var unitSym = format.Substring(1, format.Length - 2);
                if (UnitCatalog.TryGet(unitSym, out var unit) && unit.Dimension == Dimension)
                    return FormatWithUnit(unit.FromCanonical(CanonicalValue), Precision, unit, useLongName: false, formatProvider);
            }

            // Unknown format — fall back to default
            return FormatWithUnit(Value, Precision, DisplayUnit, useLongName: false, formatProvider);
        }

        /// <summary>
        /// Returns a two-system display like <c>"0.500 m (1.640 ft)"</c>. The primary is
        /// this quantity's current display unit; the parenthesized secondary is the
        /// other system (US-customary if primary is SI, or SI if primary is US-customary).
        /// </summary>
        public virtual string ToDualString()
        {
            var siUnit = UnitPreferences.SIScientific.GetPreferred(Dimension);
            var usUnit = UnitPreferences.UsCustomary.GetPreferred(Dimension);
            var provider = CultureInfo.InvariantCulture;

            var primary = FormatWithUnit(Value, Precision, DisplayUnit, useLongName: false, provider);

            // Pick the secondary: prefer the system OPPOSITE to the current display.
            Unit? secondary = null;
            if (siUnit.HasValue && siUnit.Value.Symbol == DisplayUnit.Symbol)
                secondary = usUnit;
            else if (usUnit.HasValue && usUnit.Value.Symbol == DisplayUnit.Symbol)
                secondary = siUnit;
            else
                secondary = siUnit.HasValue && siUnit.Value.Symbol != DisplayUnit.Symbol ? siUnit : usUnit;

            if (secondary == null) return primary;
            var secondaryStr = FormatWithUnit(secondary.Value.FromCanonical(CanonicalValue), Precision, secondary.Value, useLongName: false, provider);
            return $"{primary} ({secondaryStr})";
        }

        private string FormatInProfileUnit(UnitPreferences profile, IFormatProvider provider)
        {
            var preferred = profile.GetPreferred(Dimension);
            if (preferred == null)
                return FormatWithUnit(Value, Precision, DisplayUnit, useLongName: false, provider);
            return FormatWithUnit(preferred.Value.FromCanonical(CanonicalValue), Precision, preferred.Value, useLongName: false, provider);
        }

        private static string FormatWithUnit(double value, byte sigFigs, Unit unit, bool useLongName, IFormatProvider provider)
        {
            var num = FormatValue(value, sigFigs, provider);
            var label = useLongName ? unit.LongName : unit.Symbol;
            return string.IsNullOrEmpty(label) ? num : $"{num} {label}";
        }

        /// <summary>
        /// Formats a double to a fixed number of significant figures, falling back to
        /// scientific notation for extreme magnitudes.
        /// </summary>
        internal static string FormatValue(double value, int sigFigs, IFormatProvider provider)
        {
            if (double.IsNaN(value)) return "NaN";
            if (double.IsPositiveInfinity(value)) return "Infinity";
            if (double.IsNegativeInfinity(value)) return "-Infinity";
            if (value == 0.0) return (0.0).ToString("F" + Math.Max(0, sigFigs - 1), provider);

            var abs = Math.Abs(value);
            var magnitude = (int)Math.Floor(Math.Log10(abs));

            // Switch to scientific for extreme magnitudes
            if (magnitude < -4 || magnitude >= 6)
            {
                return value.ToString("E" + (sigFigs - 1), provider);
            }

            var decimals = Math.Max(0, sigFigs - 1 - magnitude);
            return value.ToString("F" + decimals, provider);
        }

        // ── Equality and comparison ──────────────────────────────────

        /// <summary>
        /// True if both operands have the same canonical value (bitwise) and the same dimension.
        /// Display unit is ignored. Use <see cref="Equals(EngineeringUnit, double)"/> for tolerance comparison.
        /// </summary>
        public bool Equals(EngineeringUnit? other)
        {
            if (other is null) return false;
            return Dimension == other.Dimension && CanonicalValue.Equals(other.CanonicalValue);
        }

        /// <summary>True if both operands are the same dimension and their canonical values agree within <paramref name="relativeTolerance"/>.</summary>
        public bool Equals(EngineeringUnit? other, double relativeTolerance)
        {
            if (other is null) return false;
            if (Dimension != other.Dimension) return false;
            var a = CanonicalValue;
            var b = other.CanonicalValue;
            if (a == b) return true;
            var scale = Math.Max(Math.Abs(a), Math.Abs(b));
            return Math.Abs(a - b) <= relativeTolerance * scale;
        }

        public override bool Equals(object? obj) => Equals(obj as EngineeringUnit);

        public override int GetHashCode() => unchecked(Dimension.GetHashCode() * 31 + CanonicalValue.GetHashCode());

        /// <summary>
        /// Compares two quantities of the same dimension by their canonical value.
        /// Throws if the dimensions differ.
        /// </summary>
        public int CompareTo(EngineeringUnit? other)
        {
            if (other is null) return 1;
            if (Dimension != other.Dimension)
                throw new DimensionMismatchException(Dimension, other.Dimension);
            return CanonicalValue.CompareTo(other.CanonicalValue);
        }

        // ── Pow / Abs (dynamic dispatch via SubclassRegistry) ──────

        /// <summary>
        /// Raises this quantity to an integer power. The result's dimension is this
        /// dimension multiplied element-wise by the exponent; the runtime resolves to
        /// the matching subclass (e.g. <c>Length.Pow(2) → Area</c>,
        /// <c>Length.Pow(3) → Volume</c>) or to <see cref="DerivedUnit"/> if no
        /// subclass matches.
        /// </summary>
        public EngineeringUnit Pow(int exponent)
        {
            if (exponent == 0) return new DimensionlessQuantity(1.0);
            if (exponent == 1) return Internal.SubclassRegistry.Create(DisplayUnit, CanonicalValue);
            var newDim = Dimension * exponent;
            var newCanonical = Math.Pow(CanonicalValue, exponent);
            return Internal.SubclassRegistry.CreateCanonical(newDim, newCanonical);
        }

        /// <summary>
        /// Raises this quantity to a non-integer power. Fails (throws) if the result
        /// dimension is not integer-valued for every base dimension. Use the integer
        /// overload when applicable for cleaner semantics.
        /// </summary>
        public EngineeringUnit Pow(double exponent)
        {
            // Integer fast-path
            var intExp = (int)Math.Round(exponent);
            if (Math.Abs(exponent - intExp) < 1e-12) return Pow(intExp);

            // Verify result dimensions remain integer
            double Scaled(sbyte e) => e * exponent;
            var nL = Scaled(Dimension.L); var nM = Scaled(Dimension.M);
            var nT = Scaled(Dimension.T); var nI = Scaled(Dimension.I);
            var nTh = Scaled(Dimension.Theta); var nN = Scaled(Dimension.N);
            var nJ = Scaled(Dimension.J); var nA = Scaled(Dimension.A);
            static bool IsInt(double d) => Math.Abs(d - Math.Round(d)) < 1e-9;
            if (!(IsInt(nL) && IsInt(nM) && IsInt(nT) && IsInt(nI) && IsInt(nTh) && IsInt(nN) && IsInt(nJ) && IsInt(nA)))
                throw new InvalidOperationException(
                    $"Pow({exponent}) on a quantity of dimension {Dimension} would yield non-integer dimension components.");
            var newDim = new DimensionSignature(
                (sbyte)Math.Round(nL), (sbyte)Math.Round(nM), (sbyte)Math.Round(nT), (sbyte)Math.Round(nI),
                (sbyte)Math.Round(nTh), (sbyte)Math.Round(nN), (sbyte)Math.Round(nJ), (sbyte)Math.Round(nA));
            var newCanonical = Math.Pow(CanonicalValue, exponent);
            return Internal.SubclassRegistry.CreateCanonical(newDim, newCanonical);
        }

        /// <summary>
        /// Returns the absolute value of this quantity, preserving subclass type and
        /// display unit.
        /// </summary>
        public EngineeringUnit Abs() => Internal.SubclassRegistry.Create(DisplayUnit, Math.Abs(CanonicalValue));
    }
}
