using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Heat capacity (per object, not per mass). SI base unit: joule per kelvin (J/K).</summary>
    public sealed class HeatCapacity : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 2 - DimensionSignature.Temperature;

        public HeatCapacity(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal HeatCapacity(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public HeatCapacity In(string unit) => new HeatCapacity(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static HeatCapacity Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <heat-capacity-unit>'"); return r!; }
        public static bool TryParse(string? s, out HeatCapacity? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new HeatCapacity(u, u.ToCanonical(v)); return true; }

        public static HeatCapacity operator +(HeatCapacity a, HeatCapacity b) => new HeatCapacity(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static HeatCapacity operator -(HeatCapacity a, HeatCapacity b) => new HeatCapacity(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static HeatCapacity operator -(HeatCapacity a) => new HeatCapacity(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static HeatCapacity operator *(HeatCapacity a, double scalar) => new HeatCapacity(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static HeatCapacity operator *(double scalar, HeatCapacity a) => a * scalar;
        public static HeatCapacity operator /(HeatCapacity a, double scalar) => new HeatCapacity(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>HeatCapacity / Mass → SpecificHeatCapacity.</summary>
        public static SpecificHeatCapacity operator /(HeatCapacity hc, Mass m) => new SpecificHeatCapacity(UnitCatalog.Get("J/(kg*K)"), hc.CanonicalValue / m.CanonicalValue);

        public static bool operator <(HeatCapacity a, HeatCapacity b) => a.CompareTo(b) < 0;
        public static bool operator >(HeatCapacity a, HeatCapacity b) => a.CompareTo(b) > 0;
        public static bool operator <=(HeatCapacity a, HeatCapacity b) => a.CompareTo(b) <= 0;
        public static bool operator >=(HeatCapacity a, HeatCapacity b) => a.CompareTo(b) >= 0;
        public static bool operator ==(HeatCapacity? a, HeatCapacity? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(HeatCapacity? a, HeatCapacity? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
