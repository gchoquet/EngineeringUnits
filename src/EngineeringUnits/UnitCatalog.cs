using System;
using System.Collections.Generic;

namespace EngineeringUnits
{
    /// <summary>
    /// The library's catalog of recognized units. Populated once at module load and
    /// then frozen — concurrent reads are safe without locking.
    /// </summary>
    /// <remarks>
    /// Lookup is case-sensitive by default to avoid ambiguity between SI prefixes
    /// and unit symbols (e.g. <c>M</c> for mega versus <c>m</c> for meter). Use
    /// <see cref="TryGet"/> for fallible lookups.
    /// </remarks>
    public static class UnitCatalog
    {
        private static readonly Dictionary<string, Unit> _units = new Dictionary<string, Unit>(StringComparer.Ordinal);

        static UnitCatalog()
        {
            // Dimensionless registration deferred to Phase 4 (DimensionlessQuantity).
            SeedLength();
            SeedMass();
            SeedTime();
            SeedTemperature();
        }

        /// <summary>Attempts to look up a unit by its symbol. Returns false if not registered.</summary>
        public static bool TryGet(string symbol, out Unit unit)
        {
            if (symbol is null) { unit = default; return false; }
            return _units.TryGetValue(symbol, out unit);
        }

        /// <summary>Looks up a unit by its symbol.</summary>
        /// <exception cref="UnknownUnitException">If the symbol is not registered.</exception>
        public static Unit Get(string symbol)
        {
            if (symbol is null) throw new ArgumentNullException(nameof(symbol));
            if (!_units.TryGetValue(symbol, out var u))
                throw new UnknownUnitException(symbol);
            return u;
        }

        /// <summary>True if the given symbol is registered in the catalog.</summary>
        public static bool IsRegistered(string symbol) => symbol != null && _units.ContainsKey(symbol);

        /// <summary>Enumerates every registered unit. Safe to call from any thread.</summary>
        public static IEnumerable<Unit> All => _units.Values;

        private static void Add(string symbol, string longName, DimensionSignature dim, double scale, double offset = 0.0)
        {
            _units[symbol] = new Unit(symbol, longName, dim, scale, offset);
        }

        // ── Catalog ──────────────────────────────────────────────────

        private static void SeedLength()
        {
            var L = DimensionSignature.Length;
            // SI base + prefixes
            Add("m",  "meter",       L, 1.0);
            Add("km", "kilometer",   L, 1000.0);
            Add("cm", "centimeter",  L, 0.01);
            Add("mm", "millimeter",  L, 0.001);
            Add("μm", "micrometer",  L, 1e-6);
            Add("um", "micrometer",  L, 1e-6);    // ASCII alias
            Add("nm", "nanometer",   L, 1e-9);
            Add("Mm", "megameter",   L, 1e6);
            // US customary
            Add("in",  "inch",          L, 0.0254);
            Add("ft",  "foot",          L, 0.3048);
            Add("yd",  "yard",          L, 0.9144);
            Add("mi",  "mile",          L, 1609.344);
            Add("nmi", "nautical mile", L, 1852.0);
            Add("fathom", "fathom",     L, 1.8288);
            Add("furlong","furlong",    L, 201.168);
        }

        private static void SeedMass()
        {
            var M = DimensionSignature.Mass;
            // SI (note: kilogram is the base; gram is derived)
            Add("kg", "kilogram",  M, 1.0);
            Add("g",  "gram",      M, 1e-3);
            Add("mg", "milligram", M, 1e-6);
            Add("μg", "microgram", M, 1e-9);
            Add("ug", "microgram", M, 1e-9);
            Add("t",  "tonne",     M, 1000.0);     // metric ton
            // US customary
            Add("lb",   "pound",      M, 0.45359237);
            Add("lbm",  "pound mass", M, 0.45359237);
            Add("oz",   "ounce",      M, 0.028349523125);
            Add("slug", "slug",       M, 14.59390294);
            Add("ton",  "short ton",  M, 907.18474);
            Add("lt",   "long ton",   M, 1016.0469088);   // UK ton
        }

        private static void SeedTime()
        {
            var T = DimensionSignature.Time;
            Add("s",    "second",      T, 1.0);
            Add("ms",   "millisecond", T, 1e-3);
            Add("μs",   "microsecond", T, 1e-6);
            Add("us",   "microsecond", T, 1e-6);
            Add("ns",   "nanosecond",  T, 1e-9);
            Add("min",  "minute",      T, 60.0);
            Add("h",    "hour",        T, 3600.0);
            Add("hr",   "hour",        T, 3600.0);
            Add("day",  "day",         T, 86400.0);
            Add("week", "week",        T, 604800.0);
            Add("year", "year",        T, 31557600.0);   // Julian year (365.25 d)
            Add("yr",   "year",        T, 31557600.0);
        }

        private static void SeedTemperature()
        {
            // Canonical: kelvin.  K = display * Scale + Offset
            //   K  : Scale=1,         Offset=0
            //   °C : Scale=1,         Offset=273.15
            //   °F : Scale=5/9,       Offset=459.67*5/9 = 255.37222...
            //   °R : Scale=5/9,       Offset=0
            var TH = DimensionSignature.Temperature;
            const double FToKScale = 5.0 / 9.0;
            const double FToKOffset = 459.67 * 5.0 / 9.0;
            Add("K",    "kelvin",            TH, 1.0,      0.0);
            Add("degC", "degree Celsius",    TH, 1.0,      273.15);
            Add("°C",   "degree Celsius",    TH, 1.0,      273.15);
            Add("degF", "degree Fahrenheit", TH, FToKScale, FToKOffset);
            Add("°F",   "degree Fahrenheit", TH, FToKScale, FToKOffset);
            Add("degR", "degree Rankine",    TH, FToKScale, 0.0);
            Add("°R",   "degree Rankine",    TH, FToKScale, 0.0);
        }
    }
}
