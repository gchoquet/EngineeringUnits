using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Heat flux density (power per unit area). SI base unit: watt per square-meter (W/m²).</summary>
    public sealed class HeatFluxDensity : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Mass - DimensionSignature.Time * 3;

        public HeatFluxDensity(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal HeatFluxDensity(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public HeatFluxDensity In(string unit) => new HeatFluxDensity(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static HeatFluxDensity Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <heat-flux-density-unit>'"); return r!; }
        public static bool TryParse(string? s, out HeatFluxDensity? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new HeatFluxDensity(u, u.ToCanonical(v)); return true; }

        public static HeatFluxDensity operator +(HeatFluxDensity a, HeatFluxDensity b) => new HeatFluxDensity(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static HeatFluxDensity operator -(HeatFluxDensity a, HeatFluxDensity b) => new HeatFluxDensity(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static HeatFluxDensity operator -(HeatFluxDensity a) => new HeatFluxDensity(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static HeatFluxDensity operator *(HeatFluxDensity a, double scalar) => new HeatFluxDensity(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static HeatFluxDensity operator *(double scalar, HeatFluxDensity a) => a * scalar;
        public static HeatFluxDensity operator /(HeatFluxDensity a, double scalar) => new HeatFluxDensity(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        public static bool operator <(HeatFluxDensity a, HeatFluxDensity b) => a.CompareTo(b) < 0;
        public static bool operator >(HeatFluxDensity a, HeatFluxDensity b) => a.CompareTo(b) > 0;
        public static bool operator <=(HeatFluxDensity a, HeatFluxDensity b) => a.CompareTo(b) <= 0;
        public static bool operator >=(HeatFluxDensity a, HeatFluxDensity b) => a.CompareTo(b) >= 0;
        public static bool operator ==(HeatFluxDensity? a, HeatFluxDensity? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(HeatFluxDensity? a, HeatFluxDensity? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
