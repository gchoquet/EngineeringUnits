using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Electric capacitance. SI unit: farad (F = C/V).</summary>
    public sealed class ElectricCapacitance : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            -(DimensionSignature.Length * 2) - DimensionSignature.Mass + DimensionSignature.Time * 4 + DimensionSignature.ElectricCurrent * 2;

        public ElectricCapacitance(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal ElectricCapacitance(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public ElectricCapacitance In(string unit) => new ElectricCapacitance(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static ElectricCapacitance Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <capacitance-unit>'"); return r!; }
        public static bool TryParse(string? s, out ElectricCapacitance? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new ElectricCapacitance(u, u.ToCanonical(v)); return true; }

        public static ElectricCapacitance operator +(ElectricCapacitance a, ElectricCapacitance b) => new ElectricCapacitance(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static ElectricCapacitance operator -(ElectricCapacitance a, ElectricCapacitance b) => new ElectricCapacitance(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static ElectricCapacitance operator -(ElectricCapacitance a) => new ElectricCapacitance(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static ElectricCapacitance operator *(ElectricCapacitance a, double scalar) => new ElectricCapacitance(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static ElectricCapacitance operator *(double scalar, ElectricCapacitance a) => a * scalar;
        public static ElectricCapacitance operator /(ElectricCapacitance a, double scalar) => new ElectricCapacitance(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Capacitance * Voltage → Charge (Q = CV).</summary>
        public static ElectricCharge operator *(ElectricCapacitance c, Voltage v) => new ElectricCharge(UnitCatalog.Get("C"), c.CanonicalValue * v.CanonicalValue);

        public static bool operator <(ElectricCapacitance a, ElectricCapacitance b) => a.CompareTo(b) < 0;
        public static bool operator >(ElectricCapacitance a, ElectricCapacitance b) => a.CompareTo(b) > 0;
        public static bool operator <=(ElectricCapacitance a, ElectricCapacitance b) => a.CompareTo(b) <= 0;
        public static bool operator >=(ElectricCapacitance a, ElectricCapacitance b) => a.CompareTo(b) >= 0;
        public static bool operator ==(ElectricCapacitance? a, ElectricCapacitance? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(ElectricCapacitance? a, ElectricCapacitance? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
