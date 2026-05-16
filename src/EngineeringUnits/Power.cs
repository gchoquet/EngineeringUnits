using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>A power quantity. SI base unit: watt (W).</summary>
    public sealed class Power : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 3;

        public Power(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Power(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Power In(string unit) => new Power(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Power Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <power-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out Power? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new Power(u, u.ToCanonical(v));
            return true;
        }

        public static Power operator +(Power a, Power b) => new Power(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Power operator -(Power a, Power b) => new Power(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Power operator -(Power a) => new Power(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Power operator *(Power a, double scalar) => new Power(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Power operator *(double scalar, Power a) => a * scalar;
        public static Power operator /(Power a, double scalar) => new Power(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Power * Time → Energy.</summary>
        public static Energy operator *(Power p, Time t) => new Energy(UnitCatalog.Get("J"), p.CanonicalValue * t.CanonicalValue);

        public static bool operator <(Power a, Power b) => a.CompareTo(b) < 0;
        public static bool operator >(Power a, Power b) => a.CompareTo(b) > 0;
        public static bool operator <=(Power a, Power b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Power a, Power b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Power? a, Power? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Power? a, Power? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
