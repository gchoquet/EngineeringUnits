using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Surface tension (force per unit length). SI base unit: newton per meter (N/m).</summary>
    public sealed class SurfaceTension : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Mass - DimensionSignature.Time * 2;

        public SurfaceTension(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal SurfaceTension(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public SurfaceTension In(string unit) => new SurfaceTension(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static SurfaceTension Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <surface-tension-unit>'"); return r!; }
        public static bool TryParse(string? s, out SurfaceTension? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new SurfaceTension(u, u.ToCanonical(v)); return true; }

        public static SurfaceTension operator +(SurfaceTension a, SurfaceTension b) => new SurfaceTension(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static SurfaceTension operator -(SurfaceTension a, SurfaceTension b) => new SurfaceTension(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static SurfaceTension operator -(SurfaceTension a) => new SurfaceTension(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static SurfaceTension operator *(SurfaceTension a, double scalar) => new SurfaceTension(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static SurfaceTension operator *(double scalar, SurfaceTension a) => a * scalar;
        public static SurfaceTension operator /(SurfaceTension a, double scalar) => new SurfaceTension(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        public static bool operator <(SurfaceTension a, SurfaceTension b) => a.CompareTo(b) < 0;
        public static bool operator >(SurfaceTension a, SurfaceTension b) => a.CompareTo(b) > 0;
        public static bool operator <=(SurfaceTension a, SurfaceTension b) => a.CompareTo(b) <= 0;
        public static bool operator >=(SurfaceTension a, SurfaceTension b) => a.CompareTo(b) >= 0;
        public static bool operator ==(SurfaceTension? a, SurfaceTension? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(SurfaceTension? a, SurfaceTension? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
