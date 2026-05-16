using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>A velocity (linear speed) quantity. SI base unit: meter per second (m/s).</summary>
    public sealed class Velocity : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Length - DimensionSignature.Time;

        public Velocity(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Velocity(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Velocity In(string unit) => new Velocity(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Velocity Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <velocity-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out Velocity? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new Velocity(u, u.ToCanonical(v));
            return true;
        }

        public static Velocity operator +(Velocity a, Velocity b) => new Velocity(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Velocity operator -(Velocity a, Velocity b) => new Velocity(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Velocity operator -(Velocity a) => new Velocity(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Velocity operator *(Velocity a, double scalar) => new Velocity(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Velocity operator *(double scalar, Velocity a) => a * scalar;
        public static Velocity operator /(Velocity a, double scalar) => new Velocity(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Velocity * Time → Length.</summary>
        public static Length operator *(Velocity v, Time t) => new Length(UnitCatalog.Get("m"), v.CanonicalValue * t.CanonicalValue);
        /// <summary>Velocity / Time → Acceleration.</summary>
        public static Acceleration operator /(Velocity v, Time t) => new Acceleration(UnitCatalog.Get("m/s^2"), v.CanonicalValue / t.CanonicalValue);

        public static bool operator <(Velocity a, Velocity b) => a.CompareTo(b) < 0;
        public static bool operator >(Velocity a, Velocity b) => a.CompareTo(b) > 0;
        public static bool operator <=(Velocity a, Velocity b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Velocity a, Velocity b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Velocity? a, Velocity? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Velocity? a, Velocity? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
