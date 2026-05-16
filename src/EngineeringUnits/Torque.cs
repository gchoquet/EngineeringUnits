using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>A torque (moment of force) quantity. SI base unit: newton-meter (N·m).</summary>
    /// <remarks>
    /// Shares dimension with <see cref="Energy"/> but uses force-first display (<c>N*m</c>,
    /// <c>lbf*ft</c>). See <see cref="Energy"/> for the disambiguation rule.
    /// </remarks>
    public sealed class Torque : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim =
            DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 2;

        public Torque(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Torque(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Torque In(string unit) => new Torque(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Torque Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <torque-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out Torque? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new Torque(u, u.ToCanonical(v));
            return true;
        }

        public static Torque operator +(Torque a, Torque b) => new Torque(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Torque operator -(Torque a, Torque b) => new Torque(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Torque operator -(Torque a) => new Torque(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Torque operator *(Torque a, double scalar) => new Torque(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Torque operator *(double scalar, Torque a) => a * scalar;
        public static Torque operator /(Torque a, double scalar) => new Torque(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Reinterpret this torque as an energy quantity (no value change, only semantic).</summary>
        public Energy AsEnergy() => new Energy(UnitCatalog.Get("J"), CanonicalValue) { Precision = Precision };

        public static bool operator <(Torque a, Torque b) => a.CompareTo(b) < 0;
        public static bool operator >(Torque a, Torque b) => a.CompareTo(b) > 0;
        public static bool operator <=(Torque a, Torque b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Torque a, Torque b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Torque? a, Torque? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Torque? a, Torque? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
