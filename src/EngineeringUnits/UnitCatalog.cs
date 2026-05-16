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
            SeedAngle();
            SeedArea();
            SeedVolume();
            SeedVelocity();
            SeedAcceleration();
            SeedForce();
            SeedPressure();
            SeedEnergy();
            SeedTorque();
            SeedPower();
            SeedDensity();
            SeedMassFlowRate();
            SeedVolumetricFlowRate();
            SeedFrequency();
            SeedAngularVelocity();
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

        private static void SeedAngle()
        {
            var A = DimensionSignature.PlaneAngle;
            Add("rad",  "radian",      A, 1.0);
            Add("deg",  "degree",      A, System.Math.PI / 180.0);
            Add("°",    "degree",      A, System.Math.PI / 180.0);
            Add("rev",  "revolution",  A, 2.0 * System.Math.PI);
            Add("grad", "gradian",     A, System.Math.PI / 200.0);
        }

        private static void SeedArea()
        {
            var Ar = DimensionSignature.Length * 2;
            // SI
            Add("m^2",  "square meter",      Ar, 1.0);
            Add("m²",   "square meter",      Ar, 1.0);
            Add("cm^2", "square centimeter", Ar, 1e-4);
            Add("cm²",  "square centimeter", Ar, 1e-4);
            Add("mm^2", "square millimeter", Ar, 1e-6);
            Add("mm²",  "square millimeter", Ar, 1e-6);
            Add("km^2", "square kilometer",  Ar, 1e6);
            Add("km²",  "square kilometer",  Ar, 1e6);
            Add("ha",   "hectare",           Ar, 1e4);
            // US customary
            Add("in^2", "square inch",       Ar, 0.0254 * 0.0254);
            Add("in²",  "square inch",       Ar, 0.0254 * 0.0254);
            Add("ft^2", "square foot",       Ar, 0.3048 * 0.3048);
            Add("ft²",  "square foot",       Ar, 0.3048 * 0.3048);
            Add("yd^2", "square yard",       Ar, 0.9144 * 0.9144);
            Add("yd²",  "square yard",       Ar, 0.9144 * 0.9144);
            Add("mi^2", "square mile",       Ar, 1609.344 * 1609.344);
            Add("mi²",  "square mile",       Ar, 1609.344 * 1609.344);
            Add("acre", "acre",              Ar, 4046.8564224);
        }

        private static void SeedVolume()
        {
            var V = DimensionSignature.Length * 3;
            // SI
            Add("m^3",  "cubic meter",      V, 1.0);
            Add("m³",   "cubic meter",      V, 1.0);
            Add("cm^3", "cubic centimeter", V, 1e-6);
            Add("cm³",  "cubic centimeter", V, 1e-6);
            Add("mm^3", "cubic millimeter", V, 1e-9);
            Add("mm³",  "cubic millimeter", V, 1e-9);
            Add("L",    "liter",            V, 1e-3);
            Add("mL",   "milliliter",       V, 1e-6);
            Add("μL",   "microliter",       V, 1e-9);
            // US customary
            Add("in^3", "cubic inch",       V, 0.0254 * 0.0254 * 0.0254);
            Add("in³",  "cubic inch",       V, 0.0254 * 0.0254 * 0.0254);
            Add("ft^3", "cubic foot",       V, 0.3048 * 0.3048 * 0.3048);
            Add("ft³",  "cubic foot",       V, 0.3048 * 0.3048 * 0.3048);
            Add("yd^3", "cubic yard",       V, 0.9144 * 0.9144 * 0.9144);
            Add("yd³",  "cubic yard",       V, 0.9144 * 0.9144 * 0.9144);
            Add("gal",  "US gallon",        V, 3.785411784e-3);
            Add("gal_imp", "imperial gallon", V, 4.54609e-3);
            Add("qt",   "US quart",         V, 0.946352946e-3);
            Add("pt",   "US pint",          V, 0.473176473e-3);
            Add("cup",  "US cup",           V, 0.2365882365e-3);
            Add("floz", "US fluid ounce",   V, 2.95735295625e-5);
            Add("bbl",  "barrel (petroleum)", V, 0.158987294928);   // 42 US gal
        }

        private static void SeedVelocity()
        {
            var Vel = DimensionSignature.Length - DimensionSignature.Time;
            Add("m/s",  "meter per second",     Vel, 1.0);
            Add("cm/s", "centimeter per second", Vel, 0.01);
            Add("mm/s", "millimeter per second", Vel, 0.001);
            Add("km/h", "kilometer per hour",   Vel, 1000.0 / 3600.0);
            Add("kph",  "kilometer per hour",   Vel, 1000.0 / 3600.0);
            Add("ft/s", "foot per second",      Vel, 0.3048);
            Add("ft/min","foot per minute",     Vel, 0.3048 / 60.0);
            Add("mph",  "mile per hour",        Vel, 1609.344 / 3600.0);
            Add("knot", "knot",                 Vel, 1852.0 / 3600.0);
            Add("kn",   "knot",                 Vel, 1852.0 / 3600.0);
        }

        private static void SeedAcceleration()
        {
            var Ac = DimensionSignature.Length - DimensionSignature.Time * 2;
            Add("m/s^2",  "meter per second squared", Ac, 1.0);
            Add("m/s²",   "meter per second squared", Ac, 1.0);
            Add("ft/s^2", "foot per second squared",  Ac, 0.3048);
            Add("ft/s²",  "foot per second squared",  Ac, 0.3048);
            Add("g_n",    "standard gravity",         Ac, 9.80665);
            Add("Gal",    "galileo",                  Ac, 0.01);
        }

        private static void SeedForce()
        {
            var F = DimensionSignature.Length + DimensionSignature.Mass - DimensionSignature.Time * 2;
            Add("N",    "newton",     F, 1.0);
            Add("kN",   "kilonewton", F, 1000.0);
            Add("MN",   "meganewton", F, 1e6);
            Add("mN",   "millinewton", F, 1e-3);
            Add("lbf",  "pound-force", F, 4.4482216152605);
            Add("kgf",  "kilogram-force", F, 9.80665);
            Add("dyn",  "dyne",       F, 1e-5);
            Add("ozf",  "ounce-force", F, 4.4482216152605 / 16.0);
        }

        private static void SeedPressure()
        {
            // Force / Area = M*L^-1*T^-2
            var P = DimensionSignature.Mass - DimensionSignature.Length - DimensionSignature.Time * 2;
            Add("Pa",   "pascal",        P, 1.0);
            Add("kPa",  "kilopascal",    P, 1000.0);
            Add("MPa",  "megapascal",    P, 1e6);
            Add("GPa",  "gigapascal",    P, 1e9);
            Add("hPa",  "hectopascal",   P, 100.0);
            Add("mPa",  "millipascal",   P, 1e-3);
            Add("bar",  "bar",           P, 1e5);
            Add("mbar", "millibar",      P, 100.0);
            Add("atm",  "standard atmosphere", P, 101325.0);
            Add("psi",  "pound per square inch", P, 6894.757293168);
            Add("psia", "psi absolute",  P, 6894.757293168);
            Add("psig", "psi gauge",     P, 6894.757293168);   // gauge vs absolute is context, scale is same
            Add("ksi",  "kilo-psi",      P, 6894757.293168);
            Add("Torr", "torr",          P, 133.322387415);
            Add("mmHg", "millimeter of mercury", P, 133.322387415);
            Add("inHg", "inch of mercury (60°F)", P, 3376.85);
            Add("inH2O","inch of water (60°F)", P, 248.84);
        }

        private static void SeedEnergy()
        {
            // L^2 * M / T^2
            var E = DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 2;
            Add("J",     "joule",          E, 1.0);
            Add("kJ",    "kilojoule",      E, 1000.0);
            Add("MJ",    "megajoule",      E, 1e6);
            Add("GJ",    "gigajoule",      E, 1e9);
            Add("mJ",    "millijoule",     E, 1e-3);
            Add("Wh",    "watt-hour",      E, 3600.0);
            Add("kWh",   "kilowatt-hour",  E, 3.6e6);
            Add("MWh",   "megawatt-hour",  E, 3.6e9);
            Add("BTU",   "British thermal unit (IT)", E, 1055.05585262);
            Add("kBTU",  "kilo-BTU",       E, 1055055.85262);
            Add("MMBTU", "million BTU",    E, 1.05505585262e9);
            Add("cal",   "calorie (thermochemical)", E, 4.184);
            Add("kcal",  "kilocalorie",    E, 4184.0);
            Add("erg",   "erg",            E, 1e-7);
            Add("ft*lbf","foot-pound (energy)", E, 1.355817948331);   // length-first display
        }

        private static void SeedTorque()
        {
            // Same dimension as Energy (L^2 * M / T^2) but different display convention.
            // Per Decision 14.14: torque uses force-first notation (lbf*ft, N*m).
            var Tq = DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 2;
            Add("N*m",   "newton-meter (torque)",   Tq, 1.0);
            Add("lbf*ft","pound-foot (torque)",     Tq, 1.355817948331);
            Add("lbf*in","pound-inch (torque)",     Tq, 0.1129848290276);
            Add("kgf*m", "kilogram-force meter",    Tq, 9.80665);
        }

        private static void SeedPower()
        {
            // L^2 * M / T^3
            var P = DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 3;
            Add("W",       "watt",            P, 1.0);
            Add("kW",      "kilowatt",        P, 1000.0);
            Add("MW",      "megawatt",        P, 1e6);
            Add("GW",      "gigawatt",        P, 1e9);
            Add("mW",      "milliwatt",       P, 1e-3);
            Add("hp",      "mechanical horsepower", P, 745.6998715822702);
            Add("hp_e",    "electric horsepower",   P, 746.0);
            Add("hp_m",    "metric horsepower",     P, 735.49875);
            Add("BTU/hr",  "BTU per hour",    P, 1055.05585262 / 3600.0);
            Add("BTU/s",   "BTU per second",  P, 1055.05585262);
            Add("ft*lbf/s","foot-pound per second", P, 1.355817948331);
            Add("MMBTU/hr","million BTU per hour", P, 1.05505585262e9 / 3600.0);
        }

        private static void SeedDensity()
        {
            // M / L^3
            var D = DimensionSignature.Mass - DimensionSignature.Length * 3;
            Add("kg/m^3",  "kilogram per cubic meter", D, 1.0);
            Add("kg/m³",   "kilogram per cubic meter", D, 1.0);
            Add("g/cm^3",  "gram per cubic centimeter", D, 1000.0);
            Add("g/cm³",   "gram per cubic centimeter", D, 1000.0);
            Add("g/mL",    "gram per milliliter",    D, 1000.0);
            Add("kg/L",    "kilogram per liter",     D, 1000.0);
            Add("lb/ft^3", "pound per cubic foot",   D, 0.45359237 / (0.3048 * 0.3048 * 0.3048));
            Add("lb/ft³",  "pound per cubic foot",   D, 0.45359237 / (0.3048 * 0.3048 * 0.3048));
            Add("lb/in^3", "pound per cubic inch",   D, 0.45359237 / (0.0254 * 0.0254 * 0.0254));
            Add("lb/in³",  "pound per cubic inch",   D, 0.45359237 / (0.0254 * 0.0254 * 0.0254));
            Add("lb/gal",  "pound per US gallon",    D, 0.45359237 / 3.785411784e-3);
        }

        private static void SeedMassFlowRate()
        {
            // M / T
            var Mf = DimensionSignature.Mass - DimensionSignature.Time;
            Add("kg/s",   "kilogram per second", Mf, 1.0);
            Add("kg/min", "kilogram per minute", Mf, 1.0 / 60.0);
            Add("kg/h",   "kilogram per hour",   Mf, 1.0 / 3600.0);
            Add("kg/hr",  "kilogram per hour",   Mf, 1.0 / 3600.0);
            Add("g/s",    "gram per second",     Mf, 1e-3);
            Add("lb/s",   "pound per second",    Mf, 0.45359237);
            Add("lb/min", "pound per minute",    Mf, 0.45359237 / 60.0);
            Add("lb/h",   "pound per hour",      Mf, 0.45359237 / 3600.0);
            Add("lb/hr",  "pound per hour",      Mf, 0.45359237 / 3600.0);
            Add("t/h",    "tonne per hour",      Mf, 1000.0 / 3600.0);
        }

        private static void SeedVolumetricFlowRate()
        {
            // L^3 / T
            var Vf = DimensionSignature.Length * 3 - DimensionSignature.Time;
            Add("m^3/s",   "cubic meter per second", Vf, 1.0);
            Add("m³/s",    "cubic meter per second", Vf, 1.0);
            Add("m^3/h",   "cubic meter per hour",   Vf, 1.0 / 3600.0);
            Add("L/s",     "liter per second",       Vf, 1e-3);
            Add("L/min",   "liter per minute",       Vf, 1e-3 / 60.0);
            Add("L/h",     "liter per hour",         Vf, 1e-3 / 3600.0);
            Add("ft^3/s",  "cubic foot per second",  Vf, 0.3048 * 0.3048 * 0.3048);
            Add("ft³/s",   "cubic foot per second",  Vf, 0.3048 * 0.3048 * 0.3048);
            Add("cfs",     "cubic feet per second",  Vf, 0.3048 * 0.3048 * 0.3048);
            Add("ft^3/min","cubic foot per minute",  Vf, 0.3048 * 0.3048 * 0.3048 / 60.0);
            Add("cfm",     "cubic feet per minute",  Vf, 0.3048 * 0.3048 * 0.3048 / 60.0);
            Add("gpm",     "US gallon per minute",   Vf, 3.785411784e-3 / 60.0);
            Add("bbl/day", "barrel per day",         Vf, 0.158987294928 / 86400.0);
            Add("bpd",     "barrel per day",         Vf, 0.158987294928 / 86400.0);
        }

        private static void SeedFrequency()
        {
            // 1 / T (no angle)
            var Fr = -DimensionSignature.Time;
            Add("Hz",  "hertz",     Fr, 1.0);
            Add("kHz", "kilohertz", Fr, 1000.0);
            Add("MHz", "megahertz", Fr, 1e6);
            Add("GHz", "gigahertz", Fr, 1e9);
            Add("1/s", "per second", Fr, 1.0);
            Add("1/min","per minute", Fr, 1.0 / 60.0);
        }

        private static void SeedAngularVelocity()
        {
            // A / T  (radians per time)
            var Aw = DimensionSignature.PlaneAngle - DimensionSignature.Time;
            Add("rad/s",  "radian per second", Aw, 1.0);
            Add("rad/min","radian per minute", Aw, 1.0 / 60.0);
            Add("deg/s",  "degree per second", Aw, System.Math.PI / 180.0);
            Add("rpm",    "revolution per minute", Aw, 2.0 * System.Math.PI / 60.0);
            Add("rps",    "revolution per second", Aw, 2.0 * System.Math.PI);
        }
    }
}
