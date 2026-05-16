using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Electric resistance. SI unit: ohm (Ω = V/A).</summary>
    public sealed class ElectricResistance : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 3 - DimensionSignature.ElectricCurrent * 2;

        public ElectricResistance(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal ElectricResistance(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public ElectricResistance In(string unit) => new ElectricResistance(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static ElectricResistance Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <resistance-unit>'"); return r!; }
        public static bool TryParse(string? s, out ElectricResistance? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new ElectricResistance(u, u.ToCanonical(v)); return true; }

        public static ElectricResistance operator +(ElectricResistance a, ElectricResistance b) => new ElectricResistance(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static ElectricResistance operator -(ElectricResistance a, ElectricResistance b) => new ElectricResistance(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static ElectricResistance operator -(ElectricResistance a) => new ElectricResistance(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static ElectricResistance operator *(ElectricResistance a, double scalar) => new ElectricResistance(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static ElectricResistance operator *(double scalar, ElectricResistance a) => a * scalar;
        public static ElectricResistance operator /(ElectricResistance a, double scalar) => new ElectricResistance(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Resistance * Current → Voltage (Ohm's law).</summary>
        public static Voltage operator *(ElectricResistance R, ElectricCurrent i) => new Voltage(UnitCatalog.Get("V"), R.CanonicalValue * i.CanonicalValue);

        public static bool operator <(ElectricResistance a, ElectricResistance b) => a.CompareTo(b) < 0;
        public static bool operator >(ElectricResistance a, ElectricResistance b) => a.CompareTo(b) > 0;
        public static bool operator <=(ElectricResistance a, ElectricResistance b) => a.CompareTo(b) <= 0;
        public static bool operator >=(ElectricResistance a, ElectricResistance b) => a.CompareTo(b) >= 0;
        public static bool operator ==(ElectricResistance? a, ElectricResistance? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(ElectricResistance? a, ElectricResistance? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
