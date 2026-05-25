using System;
using System.Globalization;

namespace EngineeringUnits
{
    /// <summary>A molar density quantity. SI canonical: mol/m³ (dimension N/L³).</summary>
    /// <remarks>
    /// Common units: mol/m³, mol/L, kmol/m³ (= mol/L numerically), mol/dm³ (= mol/L),
    /// lbmol/ft³. Pipeline / chemical-engineering work often quotes density in mol/L
    /// because that matches AGA8 / GERG-2008 internal conventions.
    /// </remarks>
    public sealed class MolarDensity : EngineeringUnit
    {
        public static readonly DimensionSignature Dim =
            DimensionSignature.AmountOfSubstance - (DimensionSignature.Length * 3);

        public MolarDensity(double value, string unit) : base(value, RequireUnit(unit)) { }
        internal MolarDensity(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        private static Unit RequireUnit(string symbol)
        {
            var u = UnitCatalog.Get(symbol);
            if (u.Dimension != Dim) throw new DimensionMismatchException(Dim, u.Dimension);
            return u;
        }

        public MolarDensity In(string unit) => new MolarDensity(RequireUnit(unit), CanonicalValue) { Precision = Precision };

        public static MolarDensity Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <molar-density-unit>'");
            return r!;
        }

        public static bool TryParse(string? s, out MolarDensity? result)
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
            result = new MolarDensity(unit, unit.ToCanonical(value));
            return true;
        }

        public static MolarDensity operator +(MolarDensity a, MolarDensity b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new MolarDensity(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        }
        public static MolarDensity operator -(MolarDensity a, MolarDensity b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new MolarDensity(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        }
        public static MolarDensity operator *(MolarDensity a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new MolarDensity(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        }
        public static MolarDensity operator *(double scalar, MolarDensity a) => a * scalar;
        public static MolarDensity operator /(MolarDensity a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new MolarDensity(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };
        }

        public static bool operator <(MolarDensity a, MolarDensity b) => a.CompareTo(b) < 0;
        public static bool operator >(MolarDensity a, MolarDensity b) => a.CompareTo(b) > 0;
        public static bool operator <=(MolarDensity a, MolarDensity b) => a.CompareTo(b) <= 0;
        public static bool operator >=(MolarDensity a, MolarDensity b) => a.CompareTo(b) >= 0;
        public static bool operator ==(MolarDensity? a, MolarDensity? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(MolarDensity? a, MolarDensity? b) => !(a == b);

        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
