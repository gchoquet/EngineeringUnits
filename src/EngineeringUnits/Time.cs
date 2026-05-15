using System;
using System.Globalization;

namespace EngineeringUnits
{
    /// <summary>A time-duration quantity. SI base unit: second.</summary>
    /// <remarks>
    /// Supported units include s, ms, μs, ns, min, h, day, week, year. For wall-clock
    /// dates use <see cref="System.DateTime"/>; this type is for physics-context durations.
    /// </remarks>
    public sealed class Time : EngineeringUnit
    {
        /// <summary>Creates a time duration from a value and unit symbol.</summary>
        public Time(double value, string unit) : base(value, RequireTimeUnit(unit)) { }

        private Time(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        private static Unit RequireTimeUnit(string symbol)
        {
            var u = UnitCatalog.Get(symbol);
            if (u.Dimension != DimensionSignature.Time)
                throw new DimensionMismatchException(DimensionSignature.Time, u.Dimension);
            return u;
        }

        /// <summary>Returns a new <see cref="Time"/> with the same canonical value expressed in the named unit.</summary>
        public Time In(string unit) => new Time(RequireTimeUnit(unit), CanonicalValue) { Precision = Precision };

        /// <summary>Parses a time from a string of the form <c>"5 s"</c>.</summary>
        public static Time Parse(string s)
        {
            if (!TryParse(s, out var result))
                throw new UnitParseException(s ?? "(null)", "expected '<value> <time-unit>'");
            return result!;
        }

        /// <summary>Attempts to parse a time from a string. Returns false on any failure.</summary>
        public static bool TryParse(string? s, out Time? result)
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
            if (unit.Dimension != DimensionSignature.Time) return false;
            result = new Time(unit, unit.ToCanonical(value));
            return true;
        }

        public static Time operator +(Time a, Time b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new Time(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue)
            { Precision = Math.Min(a.Precision, b.Precision) };
        }

        public static Time operator -(Time a, Time b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new Time(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue)
            { Precision = Math.Min(a.Precision, b.Precision) };
        }

        public static Time operator -(Time a)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new Time(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        }

        public static Time operator *(Time a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new Time(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        }

        public static Time operator *(double scalar, Time a) => a * scalar;

        public static Time operator /(Time a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new Time(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };
        }

        public static bool operator <(Time a, Time b) => a.CompareTo(b) < 0;
        public static bool operator >(Time a, Time b) => a.CompareTo(b) > 0;
        public static bool operator <=(Time a, Time b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Time a, Time b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Time? a, Time? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Time? a, Time? b) => !(a == b);

        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
