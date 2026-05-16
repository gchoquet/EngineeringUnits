using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Specific heat capacity (per unit mass). SI base unit: joule per kilogram-kelvin (J/(kg·K)).</summary>
    public sealed class SpecificHeatCapacity : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Length * 2 - DimensionSignature.Time * 2 - DimensionSignature.Temperature;

        public SpecificHeatCapacity(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal SpecificHeatCapacity(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public SpecificHeatCapacity In(string unit) => new SpecificHeatCapacity(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static SpecificHeatCapacity Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <specific-heat-capacity-unit>'"); return r!; }
        public static bool TryParse(string? s, out SpecificHeatCapacity? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new SpecificHeatCapacity(u, u.ToCanonical(v)); return true; }

        public static SpecificHeatCapacity operator +(SpecificHeatCapacity a, SpecificHeatCapacity b) => new SpecificHeatCapacity(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static SpecificHeatCapacity operator -(SpecificHeatCapacity a, SpecificHeatCapacity b) => new SpecificHeatCapacity(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static SpecificHeatCapacity operator -(SpecificHeatCapacity a) => new SpecificHeatCapacity(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static SpecificHeatCapacity operator *(SpecificHeatCapacity a, double scalar) => new SpecificHeatCapacity(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static SpecificHeatCapacity operator *(double scalar, SpecificHeatCapacity a) => a * scalar;
        public static SpecificHeatCapacity operator /(SpecificHeatCapacity a, double scalar) => new SpecificHeatCapacity(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>SpecificHeatCapacity * Mass → HeatCapacity.</summary>
        public static HeatCapacity operator *(SpecificHeatCapacity shc, Mass m) => new HeatCapacity(UnitCatalog.Get("J/K"), shc.CanonicalValue * m.CanonicalValue);

        public static bool operator <(SpecificHeatCapacity a, SpecificHeatCapacity b) => a.CompareTo(b) < 0;
        public static bool operator >(SpecificHeatCapacity a, SpecificHeatCapacity b) => a.CompareTo(b) > 0;
        public static bool operator <=(SpecificHeatCapacity a, SpecificHeatCapacity b) => a.CompareTo(b) <= 0;
        public static bool operator >=(SpecificHeatCapacity a, SpecificHeatCapacity b) => a.CompareTo(b) >= 0;
        public static bool operator ==(SpecificHeatCapacity? a, SpecificHeatCapacity? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(SpecificHeatCapacity? a, SpecificHeatCapacity? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
