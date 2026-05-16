using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>A volumetric flow rate (volume per unit time). SI base unit: m³/s.</summary>
    public sealed class VolumetricFlowRate : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Length * 3 - DimensionSignature.Time;

        public VolumetricFlowRate(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal VolumetricFlowRate(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public VolumetricFlowRate In(string unit) => new VolumetricFlowRate(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static VolumetricFlowRate Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <volumetric-flow-rate-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out VolumetricFlowRate? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new VolumetricFlowRate(u, u.ToCanonical(v));
            return true;
        }

        public static VolumetricFlowRate operator +(VolumetricFlowRate a, VolumetricFlowRate b) => new VolumetricFlowRate(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static VolumetricFlowRate operator -(VolumetricFlowRate a, VolumetricFlowRate b) => new VolumetricFlowRate(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static VolumetricFlowRate operator -(VolumetricFlowRate a) => new VolumetricFlowRate(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static VolumetricFlowRate operator *(VolumetricFlowRate a, double scalar) => new VolumetricFlowRate(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static VolumetricFlowRate operator *(double scalar, VolumetricFlowRate a) => a * scalar;
        public static VolumetricFlowRate operator /(VolumetricFlowRate a, double scalar) => new VolumetricFlowRate(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>VolumetricFlowRate * Time → Volume.</summary>
        public static Volume operator *(VolumetricFlowRate r, Time t) => new Volume(UnitCatalog.Get("m^3"), r.CanonicalValue * t.CanonicalValue);

        public static bool operator <(VolumetricFlowRate a, VolumetricFlowRate b) => a.CompareTo(b) < 0;
        public static bool operator >(VolumetricFlowRate a, VolumetricFlowRate b) => a.CompareTo(b) > 0;
        public static bool operator <=(VolumetricFlowRate a, VolumetricFlowRate b) => a.CompareTo(b) <= 0;
        public static bool operator >=(VolumetricFlowRate a, VolumetricFlowRate b) => a.CompareTo(b) >= 0;
        public static bool operator ==(VolumetricFlowRate? a, VolumetricFlowRate? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(VolumetricFlowRate? a, VolumetricFlowRate? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
