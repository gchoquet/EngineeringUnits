using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Dynamic (absolute) viscosity. SI base unit: pascal-second (Pa·s).</summary>
    public sealed class DynamicViscosity : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Mass - DimensionSignature.Length - DimensionSignature.Time;

        public DynamicViscosity(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal DynamicViscosity(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public DynamicViscosity In(string unit) => new DynamicViscosity(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static DynamicViscosity Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <dynamic-viscosity-unit>'"); return r!; }
        public static bool TryParse(string? s, out DynamicViscosity? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new DynamicViscosity(u, u.ToCanonical(v)); return true; }

        public static DynamicViscosity operator +(DynamicViscosity a, DynamicViscosity b) => new DynamicViscosity(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static DynamicViscosity operator -(DynamicViscosity a, DynamicViscosity b) => new DynamicViscosity(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static DynamicViscosity operator -(DynamicViscosity a) => new DynamicViscosity(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static DynamicViscosity operator *(DynamicViscosity a, double scalar) => new DynamicViscosity(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static DynamicViscosity operator *(double scalar, DynamicViscosity a) => a * scalar;
        public static DynamicViscosity operator /(DynamicViscosity a, double scalar) => new DynamicViscosity(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>DynamicViscosity / Density → KinematicViscosity.</summary>
        public static KinematicViscosity operator /(DynamicViscosity dv, Density rho) => new KinematicViscosity(UnitCatalog.Get("m^2/s"), dv.CanonicalValue / rho.CanonicalValue);

        public static bool operator <(DynamicViscosity a, DynamicViscosity b) => a.CompareTo(b) < 0;
        public static bool operator >(DynamicViscosity a, DynamicViscosity b) => a.CompareTo(b) > 0;
        public static bool operator <=(DynamicViscosity a, DynamicViscosity b) => a.CompareTo(b) <= 0;
        public static bool operator >=(DynamicViscosity a, DynamicViscosity b) => a.CompareTo(b) >= 0;
        public static bool operator ==(DynamicViscosity? a, DynamicViscosity? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(DynamicViscosity? a, DynamicViscosity? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
