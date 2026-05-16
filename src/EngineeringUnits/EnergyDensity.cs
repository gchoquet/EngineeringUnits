using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Energy density (energy per unit volume). SI base unit: joule per cubic-meter (J/m³).</summary>
    /// <remarks>Note: has the same dimension as <see cref="Pressure"/>, so the catalog
    /// units differ but the dimension comparison is shared. Construct explicitly via
    /// the appropriate type to preserve semantic meaning.</remarks>
    public sealed class EnergyDensity : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Mass - DimensionSignature.Length - DimensionSignature.Time * 2;

        public EnergyDensity(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal EnergyDensity(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public EnergyDensity In(string unit) => new EnergyDensity(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static EnergyDensity Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <energy-density-unit>'"); return r!; }
        public static bool TryParse(string? s, out EnergyDensity? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new EnergyDensity(u, u.ToCanonical(v)); return true; }

        public static EnergyDensity operator +(EnergyDensity a, EnergyDensity b) => new EnergyDensity(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static EnergyDensity operator -(EnergyDensity a, EnergyDensity b) => new EnergyDensity(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static EnergyDensity operator -(EnergyDensity a) => new EnergyDensity(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static EnergyDensity operator *(EnergyDensity a, double scalar) => new EnergyDensity(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static EnergyDensity operator *(double scalar, EnergyDensity a) => a * scalar;
        public static EnergyDensity operator /(EnergyDensity a, double scalar) => new EnergyDensity(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        public static bool operator <(EnergyDensity a, EnergyDensity b) => a.CompareTo(b) < 0;
        public static bool operator >(EnergyDensity a, EnergyDensity b) => a.CompareTo(b) > 0;
        public static bool operator <=(EnergyDensity a, EnergyDensity b) => a.CompareTo(b) <= 0;
        public static bool operator >=(EnergyDensity a, EnergyDensity b) => a.CompareTo(b) >= 0;
        public static bool operator ==(EnergyDensity? a, EnergyDensity? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(EnergyDensity? a, EnergyDensity? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
