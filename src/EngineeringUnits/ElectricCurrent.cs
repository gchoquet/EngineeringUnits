using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Electric current. SI base unit: ampere (A).</summary>
    public sealed class ElectricCurrent : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.ElectricCurrent;

        public ElectricCurrent(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal ElectricCurrent(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public ElectricCurrent In(string unit) => new ElectricCurrent(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static ElectricCurrent Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <current-unit>'"); return r!; }
        public static bool TryParse(string? s, out ElectricCurrent? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new ElectricCurrent(u, u.ToCanonical(v)); return true; }

        public static ElectricCurrent operator +(ElectricCurrent a, ElectricCurrent b) => new ElectricCurrent(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static ElectricCurrent operator -(ElectricCurrent a, ElectricCurrent b) => new ElectricCurrent(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static ElectricCurrent operator -(ElectricCurrent a) => new ElectricCurrent(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static ElectricCurrent operator *(ElectricCurrent a, double scalar) => new ElectricCurrent(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static ElectricCurrent operator *(double scalar, ElectricCurrent a) => a * scalar;
        public static ElectricCurrent operator /(ElectricCurrent a, double scalar) => new ElectricCurrent(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>ElectricCurrent * Time → ElectricCharge.</summary>
        public static ElectricCharge operator *(ElectricCurrent i, Time t) => new ElectricCharge(UnitCatalog.Get("C"), i.CanonicalValue * t.CanonicalValue);
        /// <summary>ElectricCurrent * Voltage → Power.</summary>
        public static Power operator *(ElectricCurrent i, Voltage v) => new Power(UnitCatalog.Get("W"), i.CanonicalValue * v.CanonicalValue);

        public static bool operator <(ElectricCurrent a, ElectricCurrent b) => a.CompareTo(b) < 0;
        public static bool operator >(ElectricCurrent a, ElectricCurrent b) => a.CompareTo(b) > 0;
        public static bool operator <=(ElectricCurrent a, ElectricCurrent b) => a.CompareTo(b) <= 0;
        public static bool operator >=(ElectricCurrent a, ElectricCurrent b) => a.CompareTo(b) >= 0;
        public static bool operator ==(ElectricCurrent? a, ElectricCurrent? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(ElectricCurrent? a, ElectricCurrent? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
