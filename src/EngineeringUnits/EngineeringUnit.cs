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

        /// <inheritdoc cref="IFormattable.ToString(string, IFormatProvider)" />
        public virtual string ToString(string? format, IFormatProvider? formatProvider)
        {
            formatProvider ??= CultureInfo.InvariantCulture;
            // Phase 1: only the default "G" form. Other format codes arrive in Phase 3.
            var numeric = FormatValue(Value, Precision, formatProvider);
            var symbol = DisplayUnit.Symbol;
            return string.IsNullOrEmpty(symbol) ? numeric : $"{numeric} {symbol}";
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
    }
}
