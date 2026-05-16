using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>Specific volume (volume per unit mass — reciprocal of density). SI base unit: cubic-meter per kilogram (m³/kg).</summary>
    public sealed class SpecificVolume : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Length * 3 - DimensionSignature.Mass;

        public SpecificVolume(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal SpecificVolume(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public SpecificVolume In(string unit) => new SpecificVolume(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static SpecificVolume Parse(string s) { if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <specific-volume-unit>'"); return r!; }
        public static bool TryParse(string? s, out SpecificVolume? result)
        { result = null; if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false; result = new SpecificVolume(u, u.ToCanonical(v)); return true; }

        public static SpecificVolume operator +(SpecificVolume a, SpecificVolume b) => new SpecificVolume(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static SpecificVolume operator -(SpecificVolume a, SpecificVolume b) => new SpecificVolume(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static SpecificVolume operator -(SpecificVolume a) => new SpecificVolume(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static SpecificVolume operator *(SpecificVolume a, double scalar) => new SpecificVolume(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static SpecificVolume operator *(double scalar, SpecificVolume a) => a * scalar;
        public static SpecificVolume operator /(SpecificVolume a, double scalar) => new SpecificVolume(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>SpecificVolume * Mass → Volume.</summary>
        public static Volume operator *(SpecificVolume sv, Mass m) => new Volume(UnitCatalog.Get("m^3"), sv.CanonicalValue * m.CanonicalValue);

        public static bool operator <(SpecificVolume a, SpecificVolume b) => a.CompareTo(b) < 0;
        public static bool operator >(SpecificVolume a, SpecificVolume b) => a.CompareTo(b) > 0;
        public static bool operator <=(SpecificVolume a, SpecificVolume b) => a.CompareTo(b) <= 0;
        public static bool operator >=(SpecificVolume a, SpecificVolume b) => a.CompareTo(b) >= 0;
        public static bool operator ==(SpecificVolume? a, SpecificVolume? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(SpecificVolume? a, SpecificVolume? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
