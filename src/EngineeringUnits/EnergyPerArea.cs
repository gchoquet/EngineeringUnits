using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>
    /// Energy per unit area (e.g. fracture toughness E/A, surface energy).
    /// SI base unit: joule per square meter (J/m²). Dimensionally equal to
    /// <see cref="SurfaceTension"/> (M/T²) but semantically distinct — kept
    /// separate for the same reason <see cref="Torque"/> and <see cref="Energy"/>
    /// are separate despite sharing M·L²/T².
    /// </summary>
    public sealed class EnergyPerArea : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Mass - DimensionSignature.Time * 2;

        public EnergyPerArea(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal EnergyPerArea(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public EnergyPerArea In(string unit) => new EnergyPerArea(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static EnergyPerArea Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <energy-per-area-unit>'"); return r!; }
        public static bool TryParse(string? s, out EnergyPerArea? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new EnergyPerArea(u, u.ToCanonical(v)); return true; }

        public static EnergyPerArea operator +(EnergyPerArea a, EnergyPerArea b) => new EnergyPerArea(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static EnergyPerArea operator -(EnergyPerArea a, EnergyPerArea b) => new EnergyPerArea(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static EnergyPerArea operator -(EnergyPerArea a) => new EnergyPerArea(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static EnergyPerArea operator *(EnergyPerArea a, double scalar) => new EnergyPerArea(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static EnergyPerArea operator *(double scalar, EnergyPerArea a) => a * scalar;
        public static EnergyPerArea operator /(EnergyPerArea a, double scalar) => new EnergyPerArea(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        public static bool operator <(EnergyPerArea a, EnergyPerArea b) => a.CompareTo(b) < 0;
        public static bool operator >(EnergyPerArea a, EnergyPerArea b) => a.CompareTo(b) > 0;
        public static bool operator <=(EnergyPerArea a, EnergyPerArea b) => a.CompareTo(b) <= 0;
        public static bool operator >=(EnergyPerArea a, EnergyPerArea b) => a.CompareTo(b) >= 0;
        public static bool operator ==(EnergyPerArea? a, EnergyPerArea? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(EnergyPerArea? a, EnergyPerArea? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
