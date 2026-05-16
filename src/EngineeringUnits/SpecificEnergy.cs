using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Specific energy (energy per unit mass). SI base unit: joule per kilogram (J/kg).</summary>
    public sealed class SpecificEnergy : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Length * 2 - DimensionSignature.Time * 2;

        public SpecificEnergy(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal SpecificEnergy(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public SpecificEnergy In(string unit) => new SpecificEnergy(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static SpecificEnergy Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <specific-energy-unit>'"); return r!; }
        public static bool TryParse(string? s, out SpecificEnergy? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new SpecificEnergy(u, u.ToCanonical(v)); return true; }

        public static SpecificEnergy operator +(SpecificEnergy a, SpecificEnergy b) => new SpecificEnergy(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static SpecificEnergy operator -(SpecificEnergy a, SpecificEnergy b) => new SpecificEnergy(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static SpecificEnergy operator -(SpecificEnergy a) => new SpecificEnergy(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static SpecificEnergy operator *(SpecificEnergy a, double scalar) => new SpecificEnergy(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static SpecificEnergy operator *(double scalar, SpecificEnergy a) => a * scalar;
        public static SpecificEnergy operator /(SpecificEnergy a, double scalar) => new SpecificEnergy(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>SpecificEnergy * Mass → Energy.</summary>
        public static Energy operator *(SpecificEnergy se, Mass m) => new Energy(UnitCatalog.Get("J"), se.CanonicalValue * m.CanonicalValue);

        public static bool operator <(SpecificEnergy a, SpecificEnergy b) => a.CompareTo(b) < 0;
        public static bool operator >(SpecificEnergy a, SpecificEnergy b) => a.CompareTo(b) > 0;
        public static bool operator <=(SpecificEnergy a, SpecificEnergy b) => a.CompareTo(b) <= 0;
        public static bool operator >=(SpecificEnergy a, SpecificEnergy b) => a.CompareTo(b) >= 0;
        public static bool operator ==(SpecificEnergy? a, SpecificEnergy? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(SpecificEnergy? a, SpecificEnergy? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
