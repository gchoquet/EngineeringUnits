using System;
using System.Globalization;

namespace EngineeringUnits.Internal
{
    /// <summary>
    /// Helpers shared by the typed quantity subclasses (Length, Mass, …, Area, …)
    /// to reduce boilerplate while keeping operator overloads strongly typed.
    /// </summary>
    internal static class QuantityHelpers
    {
        /// <summary>
        /// Resolves a unit symbol to a <see cref="Unit"/> and verifies its dimension.
        /// Throws if unknown or if the dimension doesn't match.
        /// </summary>
        public static Unit RequireUnit(string symbol, DimensionSignature expected)
        {
            var u = UnitCatalog.Get(symbol);
            if (u.Dimension != expected)
                throw new DimensionMismatchException(expected, u.Dimension);
            return u;
        }

        /// <summary>
        /// Parses a <c>"&lt;value&gt; &lt;unit&gt;"</c> string into a value + Unit pair.
        /// Returns false on any failure or wrong-dimension unit.
        /// </summary>
        public static bool TryParseValueAndUnit(string? s, DimensionSignature expected, out double value, out Unit unit)
        {
            value = 0;
            unit = default;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var trimmed = s!.Trim();
            var space = trimmed.IndexOf(' ');
            if (space < 0) return false;
            var numPart = trimmed.Substring(0, space).Trim();
            var unitPart = trimmed.Substring(space + 1).Trim();
            if (!double.TryParse(numPart, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                return false;
            if (!UnitCatalog.TryGet(unitPart, out var u)) return false;
            if (u.Dimension != expected) return false;
            unit = u;
            return true;
        }
    }
}
