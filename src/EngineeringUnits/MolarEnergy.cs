using System;
using System.Globalization;

namespace EngineeringUnits
{
    /// <summary>A molar energy quantity. SI canonical unit: J/mol (dimension L²·M/(T²·N)).</summary>
    /// <remarks>
    /// Common units: J/mol, kJ/mol, kJ/kmol (= J/mol numerically), BTU/lbmol,
    /// cal/mol, kcal/mol. Used throughout natural-gas property work for U, H, G, A.
    /// </remarks>
    public sealed class MolarEnergy : EngineeringUnit
    {
        public static readonly DimensionSignature Dim =
            (DimensionSignature.Length * 2) + DimensionSignature.Mass
            - (DimensionSignature.Time * 2) - DimensionSignature.AmountOfSubstance;

        public MolarEnergy(double value, string unit) : base(value, RequireUnit(unit)) { }
        internal MolarEnergy(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        private static Unit RequireUnit(string symbol)
        {
            var u = UnitCatalog.Get(symbol);
            if (u.Dimension != Dim) throw new DimensionMismatchException(Dim, u.Dimension);
            return u;
        }

        public MolarEnergy In(string unit) => new MolarEnergy(RequireUnit(unit), CanonicalValue) { Precision = Precision };

        public static MolarEnergy Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<value> <molar-energy-unit>'");
            return r!;
        }

        public static bool TryParse(string? s, out MolarEnergy? result)
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
            result = new MolarEnergy(unit, unit.ToCanonical(value));
            return true;
        }

        public static MolarEnergy operator +(MolarEnergy a, MolarEnergy b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new MolarEnergy(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        }
        public static MolarEnergy operator -(MolarEnergy a, MolarEnergy b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new MolarEnergy(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };
        }
        public static MolarEnergy operator -(MolarEnergy a)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new MolarEnergy(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        }
        public static MolarEnergy operator *(MolarEnergy a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new MolarEnergy(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        }
        public static MolarEnergy operator *(double scalar, MolarEnergy a) => a * scalar;
        public static MolarEnergy operator /(MolarEnergy a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new MolarEnergy(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };
        }

        public static bool operator <(MolarEnergy a, MolarEnergy b) => a.CompareTo(b) < 0;
        public static bool operator >(MolarEnergy a, MolarEnergy b) => a.CompareTo(b) > 0;
        public static bool operator <=(MolarEnergy a, MolarEnergy b) => a.CompareTo(b) <= 0;
        public static bool operator >=(MolarEnergy a, MolarEnergy b) => a.CompareTo(b) >= 0;
        public static bool operator ==(MolarEnergy? a, MolarEnergy? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(MolarEnergy? a, MolarEnergy? b) => !(a == b);

        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
