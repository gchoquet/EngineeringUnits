using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>A mass density quantity (mass per unit volume). SI base unit: kg/m³.</summary>
    public sealed class Density : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Mass - DimensionSignature.Length * 3;

        public Density(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Density(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Density In(string unit) => new Density(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Density Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <density-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out Density? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new Density(u, u.ToCanonical(v));
            return true;
        }

        public static Density operator +(Density a, Density b) => new Density(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Density operator -(Density a, Density b) => new Density(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Density operator -(Density a) => new Density(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Density operator *(Density a, double scalar) => new Density(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Density operator *(double scalar, Density a) => a * scalar;
        public static Density operator /(Density a, double scalar) => new Density(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Density * Volume → Mass.</summary>
        public static Mass operator *(Density d, Volume v) => new Mass(UnitCatalog.Get("kg"), d.CanonicalValue * v.CanonicalValue);

        public static bool operator <(Density a, Density b) => a.CompareTo(b) < 0;
        public static bool operator >(Density a, Density b) => a.CompareTo(b) > 0;
        public static bool operator <=(Density a, Density b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Density a, Density b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Density? a, Density? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Density? a, Density? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
