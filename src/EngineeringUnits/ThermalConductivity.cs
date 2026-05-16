using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Thermal conductivity. SI base unit: watt per meter-kelvin (W/(m·K)).</summary>
    public sealed class ThermalConductivity : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Length + DimensionSignature.Mass - DimensionSignature.Time * 3 - DimensionSignature.Temperature;

        public ThermalConductivity(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal ThermalConductivity(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public ThermalConductivity In(string unit) => new ThermalConductivity(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static ThermalConductivity Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <thermal-conductivity-unit>'"); return r!; }
        public static bool TryParse(string? s, out ThermalConductivity? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new ThermalConductivity(u, u.ToCanonical(v)); return true; }

        public static ThermalConductivity operator +(ThermalConductivity a, ThermalConductivity b) => new ThermalConductivity(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static ThermalConductivity operator -(ThermalConductivity a, ThermalConductivity b) => new ThermalConductivity(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static ThermalConductivity operator -(ThermalConductivity a) => new ThermalConductivity(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static ThermalConductivity operator *(ThermalConductivity a, double scalar) => new ThermalConductivity(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static ThermalConductivity operator *(double scalar, ThermalConductivity a) => a * scalar;
        public static ThermalConductivity operator /(ThermalConductivity a, double scalar) => new ThermalConductivity(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        public static bool operator <(ThermalConductivity a, ThermalConductivity b) => a.CompareTo(b) < 0;
        public static bool operator >(ThermalConductivity a, ThermalConductivity b) => a.CompareTo(b) > 0;
        public static bool operator <=(ThermalConductivity a, ThermalConductivity b) => a.CompareTo(b) <= 0;
        public static bool operator >=(ThermalConductivity a, ThermalConductivity b) => a.CompareTo(b) >= 0;
        public static bool operator ==(ThermalConductivity? a, ThermalConductivity? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(ThermalConductivity? a, ThermalConductivity? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
