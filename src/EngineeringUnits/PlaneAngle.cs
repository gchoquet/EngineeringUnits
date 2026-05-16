using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>A plane angle quantity. SI base unit: radian (rad).</summary>
    /// <remarks>
    /// Radians are dimensionless in standard SI but are tracked as a pseudo-dimension
    /// here so frequency and angular velocity stay distinguishable. See Decision 14.15.
    /// </remarks>
    public sealed class PlaneAngle : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.PlaneAngle;

        public PlaneAngle(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal PlaneAngle(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public PlaneAngle In(string unit) => new PlaneAngle(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static PlaneAngle Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <angle-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out PlaneAngle? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new PlaneAngle(u, u.ToCanonical(v));
            return true;
        }

        public static PlaneAngle operator +(PlaneAngle a, PlaneAngle b) => new PlaneAngle(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static PlaneAngle operator -(PlaneAngle a, PlaneAngle b) => new PlaneAngle(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static PlaneAngle operator -(PlaneAngle a) => new PlaneAngle(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static PlaneAngle operator *(PlaneAngle a, double scalar) => new PlaneAngle(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static PlaneAngle operator *(double scalar, PlaneAngle a) => a * scalar;
        public static PlaneAngle operator /(PlaneAngle a, double scalar) => new PlaneAngle(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>PlaneAngle / Time → AngularVelocity.</summary>
        public static AngularVelocity operator /(PlaneAngle a, Time t) => new AngularVelocity(UnitCatalog.Get("rad/s"), a.CanonicalValue / t.CanonicalValue);

        public static bool operator <(PlaneAngle a, PlaneAngle b) => a.CompareTo(b) < 0;
        public static bool operator >(PlaneAngle a, PlaneAngle b) => a.CompareTo(b) > 0;
        public static bool operator <=(PlaneAngle a, PlaneAngle b) => a.CompareTo(b) <= 0;
        public static bool operator >=(PlaneAngle a, PlaneAngle b) => a.CompareTo(b) >= 0;
        public static bool operator ==(PlaneAngle? a, PlaneAngle? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(PlaneAngle? a, PlaneAngle? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
