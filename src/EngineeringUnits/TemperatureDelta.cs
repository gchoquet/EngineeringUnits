using System;
using System.Globalization;

namespace EngineeringUnits
{
    /// <summary>
    /// A temperature difference (delta). Distinct from <see cref="Temperature"/> because
    /// temperature differences obey linear-only conversion (no offset), while absolute
    /// temperatures use affine conversion.
    /// </summary>
    /// <remarks>
    /// Examples:
    /// <list type="bullet">
    ///   <item><description>1 °C delta = 1 K delta (same magnitude — both linear in K)</description></item>
    ///   <item><description>1 °F delta = 1 °R delta = 5/9 K delta (different scale, no offset)</description></item>
    /// </list>
    /// <para>
    /// Temperature - Temperature produces a TemperatureDelta. TemperatureDelta + Temperature
    /// produces a Temperature (raising the absolute by the delta). TemperatureDelta +
    /// TemperatureDelta is itself a TemperatureDelta. Two absolute temperatures cannot
    /// be added (throws — see <see cref="Temperature.operator +"/>).
    /// </para>
    /// </remarks>
    public sealed class TemperatureDelta : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Temperature;

        /// <summary>
        /// Creates a temperature-delta from a value and the unit symbol of the scale
        /// the delta is expressed on. For Celsius and Kelvin deltas use the same symbols
        /// as absolute temperatures (numerically equivalent). For Fahrenheit / Rankine
        /// deltas the symbol's scale factor (5/9) is honored but the offset is ignored.
        /// </summary>
        public TemperatureDelta(double value, string unit)
            : base(displayUnit: RequireTempUnit(unit),
                   canonicalValue: value * RequireTempUnit(unit).Scale)
        { }

        internal TemperatureDelta(Unit displayUnit, double canonicalDeltaKelvin)
            : base(displayUnit, canonicalDeltaKelvin) { }

        private static Unit RequireTempUnit(string symbol)
        {
            var u = UnitCatalog.Get(symbol);
            if (u.Dimension != Dim)
                throw new DimensionMismatchException(Dim, u.Dimension);
            return u;
        }

        /// <summary>
        /// The numeric value of this delta in <see cref="EngineeringUnit.DisplayUnit"/>.
        /// Computed with linear conversion only (no offset).
        /// </summary>
        public new double Value => CanonicalValue / DisplayUnit.Scale;

        /// <summary>
        /// Returns this delta expressed in the named temperature unit. Linear conversion
        /// only — the unit's offset is intentionally ignored.
        /// </summary>
        public new double As(string unit)
        {
            var u = RequireTempUnit(unit);
            return CanonicalValue / u.Scale;
        }

        /// <summary>Returns a new <see cref="TemperatureDelta"/> with this delta expressed in the named unit.</summary>
        public TemperatureDelta In(string unit)
        {
            var u = RequireTempUnit(unit);
            return new TemperatureDelta(u, CanonicalValue) { Precision = Precision };
        }

        /// <summary>Parses a string of the form <c>"10 K"</c> or <c>"18 degF"</c> as a temperature delta.</summary>
        public static TemperatureDelta Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <temperature-unit>'");
            return r!;
        }

        /// <summary>Attempts to parse a temperature delta. Returns false on any failure.</summary>
        public static bool TryParse(string? s, out TemperatureDelta? result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var trimmed = s!.Trim();
            var space = trimmed.IndexOf(' ');
            if (space < 0) return false;
            var numPart = trimmed.Substring(0, space).Trim();
            var unitPart = trimmed.Substring(space + 1).Trim();
            if (!double.TryParse(numPart, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                return false;
            if (!UnitCatalog.TryGet(unitPart, out var u)) return false;
            if (u.Dimension != Dim) return false;
            result = new TemperatureDelta(u, value * u.Scale);
            return true;
        }

        public override string ToString(string? format, IFormatProvider? formatProvider)
        {
            formatProvider ??= CultureInfo.InvariantCulture;
            var num = FormatValue(Value, Precision, formatProvider);
            return $"{num} Δ{DisplayUnit.Symbol}";
        }

        // ── Arithmetic ─────────────────────────────────────────────

        public static TemperatureDelta operator +(TemperatureDelta a, TemperatureDelta b) =>
            new TemperatureDelta(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };

        public static TemperatureDelta operator -(TemperatureDelta a, TemperatureDelta b) =>
            new TemperatureDelta(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };

        public static TemperatureDelta operator -(TemperatureDelta a) =>
            new TemperatureDelta(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };

        public static TemperatureDelta operator *(TemperatureDelta a, double scalar) =>
            new TemperatureDelta(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };

        public static TemperatureDelta operator *(double scalar, TemperatureDelta a) => a * scalar;

        public static TemperatureDelta operator /(TemperatureDelta a, double scalar) =>
            new TemperatureDelta(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Temperature + TemperatureDelta → Temperature (raises the absolute by the delta).</summary>
        public static Temperature operator +(Temperature t, TemperatureDelta d) =>
            new Temperature(t.DisplayUnit, t.CanonicalValue + d.CanonicalValue) { Precision = Math.Min(t.Precision, d.Precision) };

        /// <summary>TemperatureDelta + Temperature → Temperature.</summary>
        public static Temperature operator +(TemperatureDelta d, Temperature t) => t + d;

        /// <summary>Temperature - TemperatureDelta → Temperature.</summary>
        public static Temperature operator -(Temperature t, TemperatureDelta d) =>
            new Temperature(t.DisplayUnit, t.CanonicalValue - d.CanonicalValue) { Precision = Math.Min(t.Precision, d.Precision) };

        public static bool operator <(TemperatureDelta a, TemperatureDelta b) => a.CompareTo(b) < 0;
        public static bool operator >(TemperatureDelta a, TemperatureDelta b) => a.CompareTo(b) > 0;
        public static bool operator <=(TemperatureDelta a, TemperatureDelta b) => a.CompareTo(b) <= 0;
        public static bool operator >=(TemperatureDelta a, TemperatureDelta b) => a.CompareTo(b) >= 0;
        public static bool operator ==(TemperatureDelta? a, TemperatureDelta? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(TemperatureDelta? a, TemperatureDelta? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
