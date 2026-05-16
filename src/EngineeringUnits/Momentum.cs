using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Linear momentum (mass × velocity). SI base unit: kilogram-meter per second (kg·m/s).</summary>
    public sealed class Momentum : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Length + DimensionSignature.Mass - DimensionSignature.Time;

        public Momentum(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Momentum(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Momentum In(string unit) => new Momentum(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Momentum Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <momentum-unit>'"); return r!; }
        public static bool TryParse(string? s, out Momentum? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new Momentum(u, u.ToCanonical(v)); return true; }

        public static Momentum operator +(Momentum a, Momentum b) => new Momentum(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Momentum operator -(Momentum a, Momentum b) => new Momentum(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Momentum operator -(Momentum a) => new Momentum(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Momentum operator *(Momentum a, double scalar) => new Momentum(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Momentum operator *(double scalar, Momentum a) => a * scalar;
        public static Momentum operator /(Momentum a, double scalar) => new Momentum(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Momentum / Mass → Velocity.</summary>
        public static Velocity operator /(Momentum p, Mass m) => new Velocity(UnitCatalog.Get("m/s"), p.CanonicalValue / m.CanonicalValue);
        /// <summary>Momentum / Velocity → Mass.</summary>
        public static Mass operator /(Momentum p, Velocity v) => new Mass(UnitCatalog.Get("kg"), p.CanonicalValue / v.CanonicalValue);
        /// <summary>Momentum / Time → Force (impulse-momentum relation).</summary>
        public static Force operator /(Momentum p, Time t) => new Force(UnitCatalog.Get("N"), p.CanonicalValue / t.CanonicalValue);

        public static bool operator <(Momentum a, Momentum b) => a.CompareTo(b) < 0;
        public static bool operator >(Momentum a, Momentum b) => a.CompareTo(b) > 0;
        public static bool operator <=(Momentum a, Momentum b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Momentum a, Momentum b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Momentum? a, Momentum? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Momentum? a, Momentum? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
