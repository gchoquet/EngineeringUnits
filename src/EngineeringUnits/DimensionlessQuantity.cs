using System;
using System.Collections.Generic;
using System.Globalization;
using EngineeringUnits.Internal;

namespace EngineeringUnits
{
    /// <summary>
    /// A quantity with no physical dimension — a pure number. Produced by arithmetic
    /// that reduces to all-zero dimensions (e.g. velocity / velocity, length / length).
    /// </summary>
    /// <remarks>
    /// May optionally be tagged with a <see cref="PreferredInterpretation"/> identifying
    /// its physical meaning (Reynolds number, Mach number, etc.). The library never
    /// auto-infers the interpretation — many velocity ratios are not Mach numbers and
    /// many length ratios are not strain. The user opts in (Decision 14.10).
    /// </remarks>
    public sealed class DimensionlessQuantity : EngineeringUnit
    {
        /// <summary>
        /// Known interpretation symbols for dimensionless numbers, surfaced in
        /// <c>ToString</c> when <see cref="PreferredInterpretation"/> is set.
        /// </summary>
        public static readonly IReadOnlyDictionary<string, string> KnownInterpretations =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Reynolds"]   = "Re",
                ["Mach"]       = "Ma",
                ["Froude"]     = "Fr",
                ["Prandtl"]    = "Pr",
                ["Nusselt"]    = "Nu",
                ["Biot"]       = "Bi",
                ["Péclet"]     = "Pe",
                ["Peclet"]     = "Pe",
                ["Euler"]      = "Eu",
                ["Eckert"]     = "Ec",
                ["Jakob"]      = "Ja",
                ["Schmidt"]    = "Sc",
                ["Sherwood"]   = "Sh",
                ["Lewis"]      = "Le",
                ["Weber"]      = "We",
                ["Knudsen"]    = "Kn",
                ["Strouhal"]   = "St",
            };

        /// <summary>
        /// Optional physical interpretation, e.g. <c>"Reynolds"</c>, <c>"Mach"</c>.
        /// When set to a name in <see cref="KnownInterpretations"/>, <c>ToString</c>
        /// displays the conventional symbol (<c>Re</c>, <c>Ma</c>); otherwise the raw value.
        /// </summary>
        public string? PreferredInterpretation { get; set; }

        /// <summary>Creates a dimensionless quantity from a raw number.</summary>
        public DimensionlessQuantity(double value)
            : base(value, UnitCatalog.Get("1")) { }

        internal DimensionlessQuantity(Unit displayUnit, double canonicalValue)
            : base(displayUnit, canonicalValue) { }

        /// <summary>Parses a bare numeric string to a dimensionless quantity.</summary>
        public static DimensionlessQuantity Parse(string s)
        {
            if (!TryParse(s, out var r)) throw new UnitParseException(s ?? "(null)", "expected '<number>' or '<number> 1'");
            return r!;
        }

        /// <summary>Attempts to parse a numeric string (with optional <c>"1"</c> unit) to a dimensionless quantity.</summary>
        public static bool TryParse(string? s, out DimensionlessQuantity? result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var trimmed = s!.Trim();
            // Either pure number, or "<number> 1"
            if (double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
            {
                result = new DimensionlessQuantity(v);
                return true;
            }
            if (QuantityHelpers.TryParseValueAndUnit(trimmed, DimensionSignature.Dimensionless, out var val, out var u))
            {
                result = new DimensionlessQuantity(u, u.ToCanonical(val));
                return true;
            }
            return false;
        }

        public static DimensionlessQuantity operator +(DimensionlessQuantity a, DimensionlessQuantity b) =>
            new DimensionlessQuantity(a.CanonicalValue + b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };

        public static DimensionlessQuantity operator -(DimensionlessQuantity a, DimensionlessQuantity b) =>
            new DimensionlessQuantity(a.CanonicalValue - b.CanonicalValue) { Precision = Math.Min(a.Precision, b.Precision) };

        public static DimensionlessQuantity operator *(DimensionlessQuantity a, double scalar) =>
            new DimensionlessQuantity(a.CanonicalValue * scalar) { Precision = a.Precision };

        public static DimensionlessQuantity operator *(double scalar, DimensionlessQuantity a) => a * scalar;

        public static DimensionlessQuantity operator /(DimensionlessQuantity a, double scalar) =>
            new DimensionlessQuantity(a.CanonicalValue / scalar) { Precision = a.Precision };

        public override string ToString(string? format, IFormatProvider? formatProvider)
        {
            formatProvider ??= CultureInfo.InvariantCulture;
            var num = FormatValue(Value, Precision, formatProvider);
            if (!string.IsNullOrEmpty(PreferredInterpretation) &&
                KnownInterpretations.TryGetValue(PreferredInterpretation!, out var symbol))
            {
                return $"{num} {symbol}";
            }
            // No interpretation set — emit bare number
            return num;
        }

        public static bool operator <(DimensionlessQuantity a, DimensionlessQuantity b) => a.CompareTo(b) < 0;
        public static bool operator >(DimensionlessQuantity a, DimensionlessQuantity b) => a.CompareTo(b) > 0;
        public static bool operator <=(DimensionlessQuantity a, DimensionlessQuantity b) => a.CompareTo(b) <= 0;
        public static bool operator >=(DimensionlessQuantity a, DimensionlessQuantity b) => a.CompareTo(b) >= 0;
        public static bool operator ==(DimensionlessQuantity? a, DimensionlessQuantity? b) { if (a is null) return b is null; return a.Equals(b); }
        public static bool operator !=(DimensionlessQuantity? a, DimensionlessQuantity? b) => !(a == b);
        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
