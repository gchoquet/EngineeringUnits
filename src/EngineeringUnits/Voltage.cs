using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Electrical potential difference (voltage). SI unit: volt (V = W/A = J/C).</summary>
    public sealed class Voltage : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 3 - DimensionSignature.ElectricCurrent;

        public Voltage(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Voltage(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Voltage In(string unit) => new Voltage(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Voltage Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <voltage-unit>'"); return r!; }
        public static bool TryParse(string? s, out Voltage? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new Voltage(u, u.ToCanonical(v)); return true; }

        public static Voltage operator +(Voltage a, Voltage b) => new Voltage(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Voltage operator -(Voltage a, Voltage b) => new Voltage(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Voltage operator -(Voltage a) => new Voltage(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Voltage operator *(Voltage a, double scalar) => new Voltage(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Voltage operator *(double scalar, Voltage a) => a * scalar;
        public static Voltage operator /(Voltage a, double scalar) => new Voltage(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Voltage / Current → Resistance (Ohm's law).</summary>
        public static ElectricResistance operator /(Voltage v, ElectricCurrent i) => new ElectricResistance(UnitCatalog.Get("Ω"), v.CanonicalValue / i.CanonicalValue);
        /// <summary>Voltage / Resistance → Current (Ohm's law, rearranged).</summary>
        public static ElectricCurrent operator /(Voltage v, ElectricResistance R) => new ElectricCurrent(UnitCatalog.Get("A"), v.CanonicalValue / R.CanonicalValue);
        /// <summary>Voltage * Current → Power.</summary>
        public static Power operator *(Voltage v, ElectricCurrent i) => new Power(UnitCatalog.Get("W"), v.CanonicalValue * i.CanonicalValue);

        public static bool operator <(Voltage a, Voltage b) => a.CompareTo(b) < 0;
        public static bool operator >(Voltage a, Voltage b) => a.CompareTo(b) > 0;
        public static bool operator <=(Voltage a, Voltage b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Voltage a, Voltage b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Voltage? a, Voltage? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Voltage? a, Voltage? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
