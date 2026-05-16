using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>An acceleration quantity. SI base unit: meter per second squared (m/s²).</summary>
    public sealed class Acceleration : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Length - DimensionSignature.Time * 2;

        public Acceleration(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Acceleration(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Acceleration In(string unit) => new Acceleration(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Acceleration Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <acceleration-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out Acceleration? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new Acceleration(u, u.ToCanonical(v));
            return true;
        }

        public static Acceleration operator +(Acceleration a, Acceleration b) => new Acceleration(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Acceleration operator -(Acceleration a, Acceleration b) => new Acceleration(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Acceleration operator -(Acceleration a) => new Acceleration(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Acceleration operator *(Acceleration a, double scalar) => new Acceleration(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Acceleration operator *(double scalar, Acceleration a) => a * scalar;
        public static Acceleration operator /(Acceleration a, double scalar) => new Acceleration(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Acceleration * Time → Velocity.</summary>
        public static Velocity operator *(Acceleration a, Time t) => new Velocity(UnitCatalog.Get("m/s"), a.CanonicalValue * t.CanonicalValue);
        /// <summary>Acceleration * Mass → Force.</summary>
        public static Force operator *(Acceleration a, Mass m) => new Force(UnitCatalog.Get("N"), a.CanonicalValue * m.CanonicalValue);

        public static bool operator <(Acceleration a, Acceleration b) => a.CompareTo(b) < 0;
        public static bool operator >(Acceleration a, Acceleration b) => a.CompareTo(b) > 0;
        public static bool operator <=(Acceleration a, Acceleration b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Acceleration a, Acceleration b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Acceleration? a, Acceleration? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Acceleration? a, Acceleration? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
