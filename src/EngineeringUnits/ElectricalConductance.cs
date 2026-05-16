using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Electrical conductance (reciprocal of resistance). SI unit: siemens (S = 1/Ω).</summary>
    public sealed class ElectricalConductance : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            -(DimensionSignature.Length * 2) - DimensionSignature.Mass + DimensionSignature.Time * 3 + DimensionSignature.ElectricCurrent * 2;

        public ElectricalConductance(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal ElectricalConductance(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public ElectricalConductance In(string unit) => new ElectricalConductance(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static ElectricalConductance Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <conductance-unit>'"); return r!; }
        public static bool TryParse(string? s, out ElectricalConductance? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new ElectricalConductance(u, u.ToCanonical(v)); return true; }

        public static ElectricalConductance operator +(ElectricalConductance a, ElectricalConductance b) => new ElectricalConductance(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static ElectricalConductance operator -(ElectricalConductance a, ElectricalConductance b) => new ElectricalConductance(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static ElectricalConductance operator -(ElectricalConductance a) => new ElectricalConductance(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static ElectricalConductance operator *(ElectricalConductance a, double scalar) => new ElectricalConductance(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static ElectricalConductance operator *(double scalar, ElectricalConductance a) => a * scalar;
        public static ElectricalConductance operator /(ElectricalConductance a, double scalar) => new ElectricalConductance(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        public static bool operator <(ElectricalConductance a, ElectricalConductance b) => a.CompareTo(b) < 0;
        public static bool operator >(ElectricalConductance a, ElectricalConductance b) => a.CompareTo(b) > 0;
        public static bool operator <=(ElectricalConductance a, ElectricalConductance b) => a.CompareTo(b) <= 0;
        public static bool operator >=(ElectricalConductance a, ElectricalConductance b) => a.CompareTo(b) >= 0;
        public static bool operator ==(ElectricalConductance? a, ElectricalConductance? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(ElectricalConductance? a, ElectricalConductance? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
