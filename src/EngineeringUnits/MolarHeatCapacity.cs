using System;
using System.Globalization;

namespace EngineeringUnits
{
    /// <summary>A molar heat-capacity (or molar entropy) quantity. SI canonical: J/(mol·K).</summary>
    /// <remarks>
    /// Same dimension signature as molar entropy (S). Used for Cv, Cp, and S in
    /// natural-gas property calculations on a molar basis.
    /// </remarks>
    public sealed class MolarHeatCapacity : EngineeringUnit
    {
        public static readonly DimensionSignature Dim =
            (DimensionSignature.Length * 2) + DimensionSignature.Mass
            - (DimensionSignature.Time * 2)
            - DimensionSignature.AmountOfSubstance - DimensionSignature.Temperature;

        public MolarHeatCapacity(double value, string unit) : base(value, RequireUnit(unit)) { }
        internal MolarHeatCapacity(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        private static Unit RequireUnit(string symbol)
        {
            var u = UnitCatalog.Get(symbol);
            if (u.Dimension != Dim) throw new DimensionMismatchException(Dim, u.Dimension);
            return u;
        }

        public MolarHeatCapacity In(string unit) => new MolarHeatCapacity(RequireUnit(unit), CanonicalValue) { Precision = Precision };

        public static MolarHeatCapacity Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <molar-heat-capacity-unit>'");
            return r!;
        }

        public static bool TryParse(string? s, out MolarHeatCapacity? result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var trimmed = s!.Trim();
            var space = trimmed.IndexOf(' ');
            if (space < 0) return false;
            var numPart  = trimmed.Substring(0, space).Trim();
            var unitPart = trimmed.Substring(space + 1).Trim();
            if (!double.TryParse(numPart, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)) return false;
            if (!UnitCatalog.TryGet(unitPart, out var unit)) return false;
            if (unit.Dimension != Dim) return false;
            result = new MolarHeatCapacity(unit, unit.ToCanonical(value));
            return true;
        }

        public static MolarHeatCapacity operator +(MolarHeatCapacity a, MolarHeatCapacity b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new MolarHeatCapacity(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        }
        public static MolarHeatCapacity operator -(MolarHeatCapacity a, MolarHeatCapacity b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new MolarHeatCapacity(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        }
        public static MolarHeatCapacity operator *(MolarHeatCapacity a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new MolarHeatCapacity(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        }
        public static MolarHeatCapacity operator *(double scalar, MolarHeatCapacity a) => a * scalar;
        public static MolarHeatCapacity operator /(MolarHeatCapacity a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new MolarHeatCapacity(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };
        }

        public static bool operator <(MolarHeatCapacity a, MolarHeatCapacity b) => a.CompareTo(b) < 0;
        public static bool operator >(MolarHeatCapacity a, MolarHeatCapacity b) => a.CompareTo(b) > 0;
        public static bool operator <=(MolarHeatCapacity a, MolarHeatCapacity b) => a.CompareTo(b) <= 0;
        public static bool operator >=(MolarHeatCapacity a, MolarHeatCapacity b) => a.CompareTo(b) >= 0;
        public static bool operator ==(MolarHeatCapacity? a, MolarHeatCapacity? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(MolarHeatCapacity? a, MolarHeatCapacity? b) => !(a == b);

        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
