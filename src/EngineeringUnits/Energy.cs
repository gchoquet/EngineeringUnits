using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>An energy / work quantity. SI base unit: joule (J).</summary>
    /// <remarks>
    /// Has the same fundamental dimension as <see cref="Torque"/> (M·L²·T⁻²) but a
    /// different physical meaning. The library distinguishes them by display-order
    /// convention: <c>length × force</c> yields Energy with length-first display (<c>ft*lbf</c>);
    /// <c>force × length</c> yields Torque with force-first display (<c>lbf*ft</c>).
    /// Per Decision 14.14.
    /// </remarks>
    public sealed class Energy : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 2;

        public Energy(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Energy(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Energy In(string unit) => new Energy(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Energy Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <energy-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out Energy? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new Energy(u, u.ToCanonical(v));
            return true;
        }

        public static Energy operator +(Energy a, Energy b) => new Energy(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Energy operator -(Energy a, Energy b) => new Energy(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Energy operator -(Energy a) => new Energy(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Energy operator *(Energy a, double scalar) => new Energy(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Energy operator *(double scalar, Energy a) => a * scalar;
        public static Energy operator /(Energy a, double scalar) => new Energy(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Energy / Time → Power.</summary>
        public static Power operator /(Energy e, Time t) => new Power(UnitCatalog.Get("W"), e.CanonicalValue / t.CanonicalValue);
        /// <summary>Energy / Force → Length (e.g. work / weight = lift distance).</summary>
        public static Length operator /(Energy e, Force f) => new Length(UnitCatalog.Get("m"), e.CanonicalValue / f.CanonicalValue);

        /// <summary>Reinterpret this energy quantity as a torque (no value change, only semantic).</summary>
        public Torque AsTorque() => new Torque(UnitCatalog.Get("N*m"), CanonicalValue) { Precision = Precision };

        public static bool operator <(Energy a, Energy b) => a.CompareTo(b) < 0;
        public static bool operator >(Energy a, Energy b) => a.CompareTo(b) > 0;
        public static bool operator <=(Energy a, Energy b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Energy a, Energy b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Energy? a, Energy? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Energy? a, Energy? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
