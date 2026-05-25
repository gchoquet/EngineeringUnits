using System;
using System.Globalization;

namespace EngineeringUnits
{
    /// <summary>A molar mass quantity. SI canonical unit: kg/mol (dimension M/N).</summary>
    /// <remarks>
    /// Supported units: g/mol, kg/mol, kg/kmol, lb/lbmol. Numerically g/mol = kg/kmol
    /// = lb/lbmol, which is why pipeline engineers can quote "molecular weight 16.04"
    /// without specifying units — the value is the same in all three systems.
    /// <para>
    /// Constants: <see cref="AirStandard"/> = 28.9647 g/mol (AGA8 convention).
    /// </para>
    /// </remarks>
    public sealed class MolarMass : EngineeringUnit
    {
        /// <summary>Dimension signature for molar mass: M / N.</summary>
        public static readonly DimensionSignature Dim =
            DimensionSignature.Mass - DimensionSignature.AmountOfSubstance;

        /// <summary>Molar mass of air per AGA8 convention: 28.9647 g/mol.</summary>
        public static MolarMass AirStandard => new MolarMass(28.9647, "g/mol");

        public MolarMass(double value, string unit) : base(value, RequireUnit(unit)) { }
        internal MolarMass(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        private static Unit RequireUnit(string symbol)
        {
            var u = UnitCatalog.Get(symbol);
            if (u.Dimension != Dim) throw new DimensionMismatchException(Dim, u.Dimension);
            return u;
        }

        public MolarMass In(string unit) => new MolarMass(RequireUnit(unit), CanonicalValue) { Precision = Precision };

        public static MolarMass Parse(string s)
        {
            if (!TryParse(s, out var result))
                throw new UnitParseException(s ?? "(null)", "expected '<value> <molar-mass-unit>'");
            return result!;
        }

        public static bool TryParse(string? s, out MolarMass? result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var trimmed = s!.Trim();
            var space = trimmed.IndexOf(' ');
            if (space < 0) return false;
            var numPart  = trimmed.Substring(0, space).Trim();
            var unitPart = trimmed.Substring(space + 1).Trim();
            if (!double.TryParse(numPart, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                return false;
            if (!UnitCatalog.TryGet(unitPart, out var unit)) return false;
            if (unit.Dimension != Dim) return false;
            result = new MolarMass(unit, unit.ToCanonical(value));
            return true;
        }

        public static MolarMass operator +(MolarMass a, MolarMass b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new MolarMass(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue)
            { Precision = Math.Min(a.Precision, b.Precision) };
        }

        public static MolarMass operator -(MolarMass a, MolarMass b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new MolarMass(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue)
            { Precision = Math.Min(a.Precision, b.Precision) };
        }

        public static MolarMass operator *(MolarMass a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new MolarMass(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        }
        public static MolarMass operator *(double scalar, MolarMass a) => a * scalar;
        public static MolarMass operator /(MolarMass a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new MolarMass(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };
        }

        public static bool operator <(MolarMass a, MolarMass b) => a.CompareTo(b) < 0;
        public static bool operator >(MolarMass a, MolarMass b) => a.CompareTo(b) > 0;
        public static bool operator <=(MolarMass a, MolarMass b) => a.CompareTo(b) <= 0;
        public static bool operator >=(MolarMass a, MolarMass b) => a.CompareTo(b) >= 0;
        public static bool operator ==(MolarMass? a, MolarMass? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(MolarMass? a, MolarMass? b) => !(a == b);

        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
