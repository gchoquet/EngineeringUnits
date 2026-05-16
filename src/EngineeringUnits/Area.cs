using System;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>An area quantity. SI base unit: square meter (m²).</summary>
    public sealed class Area : EngineeringUnit
    {
        internal static readonly DimensionSignature Dim = DimensionSignature.Length * 2;

        public Area(double value, string unit) : base(value, QuantityHelpers.RequireUnit(unit, Dim)) { }
        internal Area(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        public Area In(string unit) => new Area(QuantityHelpers.RequireUnit(unit, Dim), CanonicalValue) { Precision = Precision };

        public static Area Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <area-unit>'");
            return r!;
        }
        public static bool TryParse(string? s, out Area? result)
        {
            result = null;
            if (!QuantityHelpers.TryParseValueAndUnit(s, Dim, out var v, out var u)) return false;
            result = new Area(u, u.ToCanonical(v));
            return true;
        }

        public static Area operator +(Area a, Area b) => new Area(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Area operator -(Area a, Area b) => new Area(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        public static Area operator -(Area a) => new Area(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        public static Area operator *(Area a, double scalar) => new Area(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        public static Area operator *(double scalar, Area a) => a * scalar;
        public static Area operator /(Area a, double scalar) => new Area(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };

        // Cross-type
        /// <summary>Area * Length → Volume.</summary>
        public static Volume operator *(Area a, Length L) => new Volume(UnitCatalog.Get("m^3"), a.CanonicalValue * L.CanonicalValue);
        /// <summary>Area / Length → Length.</summary>
        public static Length operator /(Area a, Length L) => new Length(UnitCatalog.Get("m"), a.CanonicalValue / L.CanonicalValue);

        public static bool operator <(Area a, Area b) => a.CompareTo(b) < 0;
        public static bool operator >(Area a, Area b) => a.CompareTo(b) > 0;
        public static bool operator <=(Area a, Area b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Area a, Area b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Area? a, Area? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(Area? a, Area? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
