using System;
using System.Globalization;

namespace EngineeringUnits
{
    /// <summary>A mass quantity. SI base unit: kilogram.</summary>
    /// <remarks>
    /// Supported units include kg, g, mg, μg, t (tonne), lb, lbm, oz, slug, ton (short),
    /// and lt (long ton). See <see cref="UnitCatalog"/> for the full enumeration.
    /// </remarks>
    public sealed class Mass : EngineeringUnit
    {
        /// <summary>Creates a mass from a value and unit symbol.</summary>
        public Mass(double value, string unit) : base(value, RequireMassUnit(unit)) { }

        private Mass(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        private static Unit RequireMassUnit(string symbol)
        {
            var u = UnitCatalog.Get(symbol);
            if (u.Dimension != DimensionSignature.Mass)
                throw new DimensionMismatchException(DimensionSignature.Mass, u.Dimension);
            return u;
        }

        /// <summary>Returns a new <see cref="Mass"/> with the same canonical value expressed in the named unit.</summary>
        public Mass In(string unit) => new Mass(RequireMassUnit(unit), CanonicalValue) { Precision = Precision };

        /// <summary>Parses a mass from a string of the form <c>"5 lb"</c>.</summary>
        public static Mass Parse(string s)
        {
            if (!TryParse(s, out var result))
                throw new UnitParseException(s ?? "(null)", "expected '<value> <mass-unit>'");
            return result!;
        }

        /// <summary>Attempts to parse a mass from a string. Returns false on any failure.</summary>
        public static bool TryParse(string? s, out Mass? result)
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
            if (unit.Dimension != DimensionSignature.Mass) return false;
            result = new Mass(unit, unit.ToCanonical(value));
            return true;
        }

        public static Mass operator +(Mass a, Mass b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new Mass(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue)
            { Precision = Math.Min(a.Precision, b.Precision) };
        }

        public static Mass operator -(Mass a, Mass b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new Mass(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue)
            { Precision = Math.Min(a.Precision, b.Precision) };
        }

        public static Mass operator -(Mass a)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new Mass(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        }

        public static Mass operator *(Mass a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new Mass(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        }

        public static Mass operator *(double scalar, Mass a) => a * scalar;

        public static Mass operator /(Mass a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new Mass(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };
        }

        public static bool operator <(Mass a, Mass b) => a.CompareTo(b) < 0;
        public static bool operator >(Mass a, Mass b) => a.CompareTo(b) > 0;
        public static bool operator <=(Mass a, Mass b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Mass a, Mass b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Mass? a, Mass? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Mass? a, Mass? b) => !(a == b);

        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
