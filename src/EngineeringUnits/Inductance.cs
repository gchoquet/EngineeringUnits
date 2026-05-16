using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Electrical inductance. SI unit: henry (H = V·s/A = Wb/A).</summary>
    public sealed class Inductance : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 2 - DimensionSignature.ElectricCurrent * 2;

        public Inductance(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Inductance(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Inductance In(string unit) => new Inductance(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Inductance Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <inductance-unit>'"); return r!; }
        public static bool TryParse(string? s, out Inductance? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new Inductance(u, u.ToCanonical(v)); return true; }

        public static Inductance operator +(Inductance a, Inductance b) => new Inductance(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Inductance operator -(Inductance a, Inductance b) => new Inductance(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Inductance operator -(Inductance a) => new Inductance(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Inductance operator *(Inductance a, double scalar) => new Inductance(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Inductance operator *(double scalar, Inductance a) => a * scalar;
        public static Inductance operator /(Inductance a, double scalar) => new Inductance(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        public static bool operator <(Inductance a, Inductance b) => a.CompareTo(b) < 0;
        public static bool operator >(Inductance a, Inductance b) => a.CompareTo(b) > 0;
        public static bool operator <=(Inductance a, Inductance b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Inductance a, Inductance b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Inductance? a, Inductance? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Inductance? a, Inductance? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
