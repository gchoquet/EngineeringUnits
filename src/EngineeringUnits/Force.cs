using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>A force quantity. SI base unit: newton (N).</summary>
    public sealed class Force : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Length + DimensionSignature.Mass - DimensionSignature.Time * 2;

        public Force(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Force(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Force In(string unit) => new Force(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Force Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <force-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out Force? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new Force(u, u.ToCanonical(v));
            return true;
        }

        public static Force operator +(Force a, Force b) => new Force(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Force operator -(Force a, Force b) => new Force(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Force operator -(Force a) => new Force(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Force operator *(Force a, double scalar) => new Force(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Force operator *(double scalar, Force a) => a * scalar;
        public static Force operator /(Force a, double scalar) => new Force(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Force / Mass → Acceleration (Newton's 2nd law, rearranged).</summary>
        public static Acceleration operator /(Force F, Mass m) => new Acceleration(UnitCatalog.Get("m/s^2"), F.CanonicalValue / m.CanonicalValue);
        /// <summary>Force / Acceleration → Mass.</summary>
        public static Mass operator /(Force F, Acceleration a) => new Mass(UnitCatalog.Get("kg"), F.CanonicalValue / a.CanonicalValue);
        /// <summary>Force / Area → Pressure.</summary>
        public static Pressure operator /(Force F, Area A) => new Pressure(UnitCatalog.Get("Pa"), F.CanonicalValue / A.CanonicalValue);
        /// <summary>Force * Length → Torque (force-first display, per Decision 14.14).</summary>
        public static Torque operator *(Force F, Length L) => new Torque(UnitCatalog.Get("N*m"), F.CanonicalValue * L.CanonicalValue);
        /// <summary>Force * Velocity → Power.</summary>
        public static Power operator *(Force F, Velocity v) => new Power(UnitCatalog.Get("W"), F.CanonicalValue * v.CanonicalValue);

        public static bool operator <(Force a, Force b) => a.CompareTo(b) < 0;
        public static bool operator >(Force a, Force b) => a.CompareTo(b) > 0;
        public static bool operator <=(Force a, Force b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Force a, Force b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Force? a, Force? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Force? a, Force? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
