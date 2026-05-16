using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>An angular velocity quantity. SI base unit: radian per second (rad/s).</summary>
    /// <remarks>
    /// Carries the plane-angle pseudo-dimension (see <see cref="DimensionSignature"/>),
    /// distinguishing it from <see cref="Frequency"/>. Both have the same SI value
    /// numerically but represent different physical concepts.
    /// </remarks>
    public sealed class AngularVelocity : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.PlaneAngle - DimensionSignature.Time;

        public AngularVelocity(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal AngularVelocity(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public AngularVelocity In(string unit) => new AngularVelocity(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static AngularVelocity Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <angular-velocity-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out AngularVelocity? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new AngularVelocity(u, u.ToCanonical(v));
            return true;
        }

        public static AngularVelocity operator +(AngularVelocity a, AngularVelocity b) => new AngularVelocity(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static AngularVelocity operator -(AngularVelocity a, AngularVelocity b) => new AngularVelocity(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static AngularVelocity operator -(AngularVelocity a) => new AngularVelocity(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static AngularVelocity operator *(AngularVelocity a, double scalar) => new AngularVelocity(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static AngularVelocity operator *(double scalar, AngularVelocity a) => a * scalar;
        public static AngularVelocity operator /(AngularVelocity a, double scalar) => new AngularVelocity(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        public static bool operator <(AngularVelocity a, AngularVelocity b) => a.CompareTo(b) < 0;
        public static bool operator >(AngularVelocity a, AngularVelocity b) => a.CompareTo(b) > 0;
        public static bool operator <=(AngularVelocity a, AngularVelocity b) => a.CompareTo(b) <= 0;
        public static bool operator >=(AngularVelocity a, AngularVelocity b) => a.CompareTo(b) >= 0;
        public static bool operator ==(AngularVelocity? a, AngularVelocity? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(AngularVelocity? a, AngularVelocity? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
