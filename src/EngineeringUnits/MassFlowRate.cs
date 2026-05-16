using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>A mass flow rate (mass per unit time). SI base unit: kg/s.</summary>
    public sealed class MassFlowRate : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Mass - DimensionSignature.Time;

        public MassFlowRate(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal MassFlowRate(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public MassFlowRate In(string unit) => new MassFlowRate(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static MassFlowRate Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <mass-flow-rate-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out MassFlowRate? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new MassFlowRate(u, u.ToCanonical(v));
            return true;
        }

        public static MassFlowRate operator +(MassFlowRate a, MassFlowRate b) => new MassFlowRate(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static MassFlowRate operator -(MassFlowRate a, MassFlowRate b) => new MassFlowRate(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static MassFlowRate operator -(MassFlowRate a) => new MassFlowRate(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static MassFlowRate operator *(MassFlowRate a, double scalar) => new MassFlowRate(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static MassFlowRate operator *(double scalar, MassFlowRate a) => a * scalar;
        public static MassFlowRate operator /(MassFlowRate a, double scalar) => new MassFlowRate(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>MassFlowRate * Time → Mass.</summary>
        public static Mass operator *(MassFlowRate r, Time t) => new Mass(UnitCatalog.Get("kg"), r.CanonicalValue * t.CanonicalValue);

        public static bool operator <(MassFlowRate a, MassFlowRate b) => a.CompareTo(b) < 0;
        public static bool operator >(MassFlowRate a, MassFlowRate b) => a.CompareTo(b) > 0;
        public static bool operator <=(MassFlowRate a, MassFlowRate b) => a.CompareTo(b) <= 0;
        public static bool operator >=(MassFlowRate a, MassFlowRate b) => a.CompareTo(b) >= 0;
        public static bool operator ==(MassFlowRate? a, MassFlowRate? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(MassFlowRate? a, MassFlowRate? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
