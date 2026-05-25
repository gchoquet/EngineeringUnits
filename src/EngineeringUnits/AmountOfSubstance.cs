using System;
using System.Globalization;

namespace EngineeringUnits
{
    /// <summary>An amount of substance. SI base unit: mole.</summary>
    /// <remarks>
    /// Supported units: mol, kmol, lbmol (pound-mole). The mole is one of the
    /// seven SI base dimensions and the prerequisite for any molar-basis quantity
    /// (MolarMass, MolarEnergy, MolarHeatCapacity, MolarDensity).
    /// </remarks>
    public sealed class AmountOfSubstance : EngineeringUnit
    {
        /// <summary>The dimension signature for amount of substance (N).</summary>
        public static readonly DimensionSignature Dim = DimensionSignature.AmountOfSubstance;

        public AmountOfSubstance(double value, string unit) : base(value, RequireUnit(unit)) { }
        internal AmountOfSubstance(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        private static Unit RequireUnit(string symbol)
        {
            var u = UnitCatalog.Get(symbol);
            if (u.Dimension != Dim) throw new DimensionMismatchException(Dim, u.Dimension);
            return u;
        }

        public AmountOfSubstance In(string unit) => new AmountOfSubstance(RequireUnit(unit), CanonicalValue) { Precision = Precision };

        public static AmountOfSubstance Parse(string s)
        {
            if (!TryParse(s, out var result))
                throw new UnitParseException(s ?? "(null)", "expected '<value> <amount-unit>'");
            return result!;
        }

        public static bool TryParse(string? s, out AmountOfSubstance? result)
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
            result = new AmountOfSubstance(unit, unit.ToCanonical(value));
            return true;
        }

        public static AmountOfSubstance operator +(AmountOfSubstance a, AmountOfSubstance b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new AmountOfSubstance(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue)
            { Precision = Math.Min(a.Precision, b.Precision) };
        }

        public static AmountOfSubstance operator -(AmountOfSubstance a, AmountOfSubstance b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new AmountOfSubstance(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue)
            { Precision = Math.Min(a.Precision, b.Precision) };
        }

        public static AmountOfSubstance operator -(AmountOfSubstance a)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new AmountOfSubstance(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        }

        public static AmountOfSubstance operator *(AmountOfSubstance a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new AmountOfSubstance(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        }

        public static AmountOfSubstance operator *(double scalar, AmountOfSubstance a) => a * scalar;

        public static AmountOfSubstance operator /(AmountOfSubstance a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new AmountOfSubstance(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };
        }

        public static bool operator <(AmountOfSubstance a, AmountOfSubstance b) => a.CompareTo(b) < 0;
        public static bool operator >(AmountOfSubstance a, AmountOfSubstance b) => a.CompareTo(b) > 0;
        public static bool operator <=(AmountOfSubstance a, AmountOfSubstance b) => a.CompareTo(b) <= 0;
        public static bool operator >=(AmountOfSubstance a, AmountOfSubstance b) => a.CompareTo(b) >= 0;
        public static bool operator ==(AmountOfSubstance? a, AmountOfSubstance? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(AmountOfSubstance? a, AmountOfSubstance? b) => !(a == b);

        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
