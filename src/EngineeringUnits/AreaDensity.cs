using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Area density (mass per unit area). SI base unit: kilogram per square-meter (kg/m²).</summary>
    public sealed class AreaDensity : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Mass - DimensionSignature.Length * 2;

        public AreaDensity(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal AreaDensity(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public AreaDensity In(string unit) => new AreaDensity(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static AreaDensity Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <area-density-unit>'"); return r!; }
        public static bool TryParse(string? s, out AreaDensity? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new AreaDensity(u, u.ToCanonical(v)); return true; }

        public static AreaDensity operator +(AreaDensity a, AreaDensity b) => new AreaDensity(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static AreaDensity operator -(AreaDensity a, AreaDensity b) => new AreaDensity(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static AreaDensity operator -(AreaDensity a) => new AreaDensity(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static AreaDensity operator *(AreaDensity a, double scalar) => new AreaDensity(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static AreaDensity operator *(double scalar, AreaDensity a) => a * scalar;
        public static AreaDensity operator /(AreaDensity a, double scalar) => new AreaDensity(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>AreaDensity * Area → Mass.</summary>
        public static Mass operator *(AreaDensity ad, Area A) => new Mass(UnitCatalog.Get("kg"), ad.CanonicalValue * A.CanonicalValue);

        public static bool operator <(AreaDensity a, AreaDensity b) => a.CompareTo(b) < 0;
        public static bool operator >(AreaDensity a, AreaDensity b) => a.CompareTo(b) > 0;
        public static bool operator <=(AreaDensity a, AreaDensity b) => a.CompareTo(b) <= 0;
        public static bool operator >=(AreaDensity a, AreaDensity b) => a.CompareTo(b) >= 0;
        public static bool operator ==(AreaDensity? a, AreaDensity? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(AreaDensity? a, AreaDensity? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
