using System;
using System.Globalization;

namespace EngineeringUnits
{
    /// <summary>An absolute temperature. SI base unit: kelvin.</summary>
    /// <remarks>
    /// Supported units include K, °C / degC, °F / degF, and °R / degR. Note that
    /// non-Kelvin temperature units use affine (scale + offset) conversion rather
    /// than purely multiplicative scaling.
    /// <para>
    /// <b>Arithmetic on absolute temperatures is intentionally restricted in v1.</b>
    /// Adding two temperatures is physically meaningless and throws. Subtraction
    /// produces a result in kelvin since the difference of two temperatures is a
    /// delta, and a proper <c>TemperatureDelta</c> type is deferred to v1.x
    /// (specification §14.9). For now, callers needing temperature arithmetic
    /// should reason in kelvin via <see cref="EngineeringUnit.As"/>.
    /// </para>
    /// </remarks>
    public sealed class Temperature : EngineeringUnit
    {
        /// <summary>Creates a temperature from a value and unit symbol.</summary>
        public Temperature(double value, string unit) : base(value, RequireTemperatureUnit(unit)) { }

        private Temperature(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        private static Unit RequireTemperatureUnit(string symbol)
        {
            var u = UnitCatalog.Get(symbol);
            if (u.Dimension != DimensionSignature.Temperature)
                throw new DimensionMismatchException(DimensionSignature.Temperature, u.Dimension);
            return u;
        }

        /// <summary>Returns a new <see cref="Temperature"/> with the same canonical value expressed in the named unit.</summary>
        public Temperature In(string unit) => new Temperature(RequireTemperatureUnit(unit), CanonicalValue) { Precision = Precision };

        /// <summary>Parses a temperature from a string of the form <c>"25 degC"</c> or <c>"77 °F"</c>.</summary>
        public static Temperature Parse(string s)
        {
            if (!TryParse(s, out var result))
                throw new UnitParseException(s ?? "(null)", "expected '<value> <temperature-unit>'");
            return result!;
        }

        /// <summary>Attempts to parse a temperature from a string. Returns false on any failure.</summary>
        public static bool TryParse(string? s, out Temperature? result)
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
            if (!UnitCatalog.TryGet(unitPart, out var unit)) return false;
            if (unit.Dimension != DimensionSignature.Temperature) return false;
            result = new Temperature(unit, unit.ToCanonical(value));
            return true;
        }

        /// <summary>Adding two absolute temperatures is physically meaningless and throws.</summary>
        /// <exception cref="InvalidOperationException">Always.</exception>
        public static Temperature operator +(Temperature a, Temperature b)
        {
            throw new InvalidOperationException(
                "Adding two absolute temperatures is physically meaningless. " +
                "To raise a temperature by a delta, compute in kelvin via .As(\"K\") " +
                "and reconstruct. (TemperatureDelta type planned for v1.x.)");
        }

        /// <summary>Subtraction yields a temperature difference, returned in kelvin.</summary>
        public static Temperature operator -(Temperature a, Temperature b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            var kelvinDelta = a.CanonicalValue - b.CanonicalValue;
            return new Temperature(UnitCatalog.Get("K"), kelvinDelta)
            { Precision = Math.Min(a.Precision, b.Precision) };
        }

        public static bool operator <(Temperature a, Temperature b) => a.CompareTo(b) < 0;
        public static bool operator >(Temperature a, Temperature b) => a.CompareTo(b) > 0;
        public static bool operator <=(Temperature a, Temperature b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Temperature a, Temperature b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Temperature? a, Temperature? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Temperature? a, Temperature? b) => !(a == b);

        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
