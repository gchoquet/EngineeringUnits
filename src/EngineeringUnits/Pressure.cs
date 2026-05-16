using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>A pressure quantity. SI base unit: pascal (Pa).</summary>
    public sealed class Pressure : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Mass - DimensionSignature.Length - DimensionSignature.Time * 2;

        public Pressure(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Pressure(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Pressure In(string unit) => new Pressure(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Pressure Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <pressure-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out Pressure? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new Pressure(u, u.ToCanonical(v));
            return true;
        }

        public static Pressure operator +(Pressure a, Pressure b) => new Pressure(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Pressure operator -(Pressure a, Pressure b) => new Pressure(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Pressure operator -(Pressure a) => new Pressure(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Pressure operator *(Pressure a, double scalar) => new Pressure(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Pressure operator *(double scalar, Pressure a) => a * scalar;
        public static Pressure operator /(Pressure a, double scalar) => new Pressure(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Pressure * Area → Force.</summary>
        public static Force operator *(Pressure p, Area A) => new Force(UnitCatalog.Get("N"), p.CanonicalValue * A.CanonicalValue);

        public static bool operator <(Pressure a, Pressure b) => a.CompareTo(b) < 0;
        public static bool operator >(Pressure a, Pressure b) => a.CompareTo(b) > 0;
        public static bool operator <=(Pressure a, Pressure b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Pressure a, Pressure b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Pressure? a, Pressure? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Pressure? a, Pressure? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
