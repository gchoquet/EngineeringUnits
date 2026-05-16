using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Electric charge. SI unit: coulomb (C = A·s).</summary>
    public sealed class ElectricCharge : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Time + DimensionSignature.ElectricCurrent;

        public ElectricCharge(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal ElectricCharge(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public ElectricCharge In(string unit) => new ElectricCharge(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static ElectricCharge Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <charge-unit>'"); return r!; }
        public static bool TryParse(string? s, out ElectricCharge? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new ElectricCharge(u, u.ToCanonical(v)); return true; }

        public static ElectricCharge operator +(ElectricCharge a, ElectricCharge b) => new ElectricCharge(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static ElectricCharge operator -(ElectricCharge a, ElectricCharge b) => new ElectricCharge(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static ElectricCharge operator -(ElectricCharge a) => new ElectricCharge(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static ElectricCharge operator *(ElectricCharge a, double scalar) => new ElectricCharge(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static ElectricCharge operator *(double scalar, ElectricCharge a) => a * scalar;
        public static ElectricCharge operator /(ElectricCharge a, double scalar) => new ElectricCharge(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>ElectricCharge / Time → ElectricCurrent.</summary>
        public static ElectricCurrent operator /(ElectricCharge q, Time t) => new ElectricCurrent(UnitCatalog.Get("A"), q.CanonicalValue / t.CanonicalValue);
        /// <summary>ElectricCharge * Voltage → Energy.</summary>
        public static Energy operator *(ElectricCharge q, Voltage v) => new Energy(UnitCatalog.Get("J"), q.CanonicalValue * v.CanonicalValue);

        public static bool operator <(ElectricCharge a, ElectricCharge b) => a.CompareTo(b) < 0;
        public static bool operator >(ElectricCharge a, ElectricCharge b) => a.CompareTo(b) > 0;
        public static bool operator <=(ElectricCharge a, ElectricCharge b) => a.CompareTo(b) <= 0;
        public static bool operator >=(ElectricCharge a, ElectricCharge b) => a.CompareTo(b) >= 0;
        public static bool operator ==(ElectricCharge? a, ElectricCharge? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(ElectricCharge? a, ElectricCharge? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
