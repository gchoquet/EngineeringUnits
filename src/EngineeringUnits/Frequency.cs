using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>A frequency quantity. SI base unit: hertz (Hz = 1/s).</summary>
    /// <remarks>
    /// Distinct from <see cref="AngularVelocity"/>: frequency has no angle dimension,
    /// while angular velocity does. <c>1/Time → Frequency</c>; <c>Angle/Time → AngularVelocity</c>.
    /// </remarks>
    public sealed class Frequency : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = -DimensionSignature.Time;

        public Frequency(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Frequency(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Frequency In(string unit) => new Frequency(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Frequency Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <frequency-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out Frequency? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new Frequency(u, u.ToCanonical(v));
            return true;
        }

        public static Frequency operator +(Frequency a, Frequency b) => new Frequency(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Frequency operator -(Frequency a, Frequency b) => new Frequency(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Frequency operator -(Frequency a) => new Frequency(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Frequency operator *(Frequency a, double scalar) => new Frequency(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Frequency operator *(double scalar, Frequency a) => a * scalar;
        public static Frequency operator /(Frequency a, double scalar) => new Frequency(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        public static bool operator <(Frequency a, Frequency b) => a.CompareTo(b) < 0;
        public static bool operator >(Frequency a, Frequency b) => a.CompareTo(b) > 0;
        public static bool operator <=(Frequency a, Frequency b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Frequency a, Frequency b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Frequency? a, Frequency? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Frequency? a, Frequency? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
