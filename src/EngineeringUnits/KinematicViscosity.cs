using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Kinematic viscosity (dynamic viscosity divided by density). SI base unit: square-meter per second (m²/s).</summary>
    public sealed class KinematicViscosity : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Length * 2 - DimensionSignature.Time;

        public KinematicViscosity(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal KinematicViscosity(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public KinematicViscosity In(string unit) => new KinematicViscosity(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static KinematicViscosity Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <kinematic-viscosity-unit>'"); return r!; }
        public static bool TryParse(string? s, out KinematicViscosity? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new KinematicViscosity(u, u.ToCanonical(v)); return true; }

        public static KinematicViscosity operator +(KinematicViscosity a, KinematicViscosity b) => new KinematicViscosity(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static KinematicViscosity operator -(KinematicViscosity a, KinematicViscosity b) => new KinematicViscosity(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static KinematicViscosity operator -(KinematicViscosity a) => new KinematicViscosity(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static KinematicViscosity operator *(KinematicViscosity a, double scalar) => new KinematicViscosity(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static KinematicViscosity operator *(double scalar, KinematicViscosity a) => a * scalar;
        public static KinematicViscosity operator /(KinematicViscosity a, double scalar) => new KinematicViscosity(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>KinematicViscosity * Density → DynamicViscosity.</summary>
        public static DynamicViscosity operator *(KinematicViscosity kv, Density rho) => new DynamicViscosity(UnitCatalog.Get("Pa*s"), kv.CanonicalValue * rho.CanonicalValue);

        public static bool operator <(KinematicViscosity a, KinematicViscosity b) => a.CompareTo(b) < 0;
        public static bool operator >(KinematicViscosity a, KinematicViscosity b) => a.CompareTo(b) > 0;
        public static bool operator <=(KinematicViscosity a, KinematicViscosity b) => a.CompareTo(b) <= 0;
        public static bool operator >=(KinematicViscosity a, KinematicViscosity b) => a.CompareTo(b) >= 0;
        public static bool operator ==(KinematicViscosity? a, KinematicViscosity? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(KinematicViscosity? a, KinematicViscosity? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
