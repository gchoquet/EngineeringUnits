using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>A volume quantity. SI base unit: cubic meter (m³).</summary>
    public sealed class Volume : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Length * 3;

        public Volume(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Volume(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Volume In(string unit) => new Volume(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Volume Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <volume-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out Volume? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new Volume(u, u.ToCanonical(v));
            return true;
        }

        public static Volume operator +(Volume a, Volume b) => new Volume(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Volume operator -(Volume a, Volume b) => new Volume(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Volume operator -(Volume a) => new Volume(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Volume operator *(Volume a, double scalar) => new Volume(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Volume operator *(double scalar, Volume a) => a * scalar;
        public static Volume operator /(Volume a, double scalar) => new Volume(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        /// <summary>Volume / Area → Length.</summary>
        public static Length operator /(Volume a, Area A) => new Length(UnitCatalog.Get("m"), a.CanonicalValue / A.CanonicalValue);
        /// <summary>Volume / Length → Area.</summary>
        public static Area operator /(Volume a, Length L) => new Area(UnitCatalog.Get("m^2"), a.CanonicalValue / L.CanonicalValue);
        /// <summary>Volume / Time → VolumetricFlowRate.</summary>
        public static VolumetricFlowRate operator /(Volume a, Time t) => new VolumetricFlowRate(UnitCatalog.Get("m^3/s"), a.CanonicalValue / t.CanonicalValue);

        public static bool operator <(Volume a, Volume b) => a.CompareTo(b) < 0;
        public static bool operator >(Volume a, Volume b) => a.CompareTo(b) > 0;
        public static bool operator <=(Volume a, Volume b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Volume a, Volume b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Volume? a, Volume? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Volume? a, Volume? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
