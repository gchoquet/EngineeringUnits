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
        // Primary index: exact-case symbol lookup ("m" vs "M" matter).
        private static readonly Dictionary<string, Unit> _units = new Dictionary<string, Unit>(StringComparer.Ordinal);

        // Secondary index: case-insensitive long-name lookup ("foot", "Foot", "FOOT" all match).
        // Built once after all units are seeded.
        private static readonly Dictionary<string, Unit> _byLongName = new Dictionary<string, Unit>(StringComparer.OrdinalIgnoreCase);

        // Tertiary index: irregular plural / common-name aliases mapping the alternate form
        // to the unit's long name. English plurals are too irregular to derive ("foot" -> "feet"),
        // so we list them explicitly.
        private static readonly Dictionary<string, string> _aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        static UnitCatalog()
        {
            SeedDimensionless();
            SeedLength();
            SeedMass();
            SeedTime();
            SeedTemperature();
            SeedAmountOfSubstance();
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
            SeedDynamicViscosity();
            SeedKinematicViscosity();
            SeedThermalConductivity();
            SeedSpecificHeatCapacity();
            SeedHeatCapacity();
            SeedHeatFluxDensity();
            SeedMomentum();
            SeedSurfaceTension();
            SeedEnergyPerArea();
            SeedSpecificEnergy();
            SeedEnergyDensity();
            SeedSpecificVolume();
            SeedAreaDensity();
            SeedElectricCurrent();
            SeedElectricCharge();
            SeedVoltage();
            SeedElectricResistance();
            SeedElectricalConductance();
            SeedElectricCapacitance();
            SeedInductance();
            SeedMolarMass();
            SeedMolarEnergy();
            SeedMolarHeatCapacity();
            SeedMolarDensity();
            SeedJouleThomson();
            BuildLongNameIndex();
            SeedAliases();
            RegisterSubclasses();
        }

        private static void RegisterSubclasses()
        {
            var R = typeof(Internal.SubclassRegistry);
            // Base
            Internal.SubclassRegistry.Register(DimensionSignature.Length, "m",  (u, cv) => new Length(u, cv));
            Internal.SubclassRegistry.Register(DimensionSignature.Mass,   "kg", (u, cv) => new Mass(u, cv));
            Internal.SubclassRegistry.Register(DimensionSignature.Time,   "s",  (u, cv) => new Time(u, cv));
            Internal.SubclassRegistry.Register(DimensionSignature.Temperature, "K", (u, cv) => new Temperature(u, cv));
            Internal.SubclassRegistry.Register(DimensionSignature.PlaneAngle, "rad", (u, cv) => new PlaneAngle(u, cv));
            // Geometric / kinematic
            Internal.SubclassRegistry.Register(Area.Dim,            "m^2",     (u, cv) => new Area(u, cv));
            Internal.SubclassRegistry.Register(Volume.Dim,          "m^3",     (u, cv) => new Volume(u, cv));
            Internal.SubclassRegistry.Register(Velocity.Dim,        "m/s",     (u, cv) => new Velocity(u, cv));
            Internal.SubclassRegistry.Register(Acceleration.Dim,    "m/s^2",   (u, cv) => new Acceleration(u, cv));
            Internal.SubclassRegistry.Register(AngularVelocity.Dim, "rad/s",   (u, cv) => new AngularVelocity(u, cv));
            Internal.SubclassRegistry.Register(Frequency.Dim,       "Hz",      (u, cv) => new Frequency(u, cv));
            // Dynamic
            Internal.SubclassRegistry.Register(Force.Dim,    "N",  (u, cv) => new Force(u, cv));
            Internal.SubclassRegistry.Register(Pressure.Dim, "Pa", (u, cv) => new Pressure(u, cv));
            Internal.SubclassRegistry.Register(Energy.Dim,   "J",  (u, cv) => new Energy(u, cv));  // Torque shares dim; Energy registered as the default
            Internal.SubclassRegistry.Register(Power.Dim,    "W",  (u, cv) => new Power(u, cv));
            // Flow / material
            Internal.SubclassRegistry.Register(Density.Dim,            "kg/m^3",  (u, cv) => new Density(u, cv));
            Internal.SubclassRegistry.Register(MassFlowRate.Dim,       "kg/s",    (u, cv) => new MassFlowRate(u, cv));
            Internal.SubclassRegistry.Register(VolumetricFlowRate.Dim, "m^3/s",   (u, cv) => new VolumetricFlowRate(u, cv));
            // Tier A
            Internal.SubclassRegistry.Register(DynamicViscosity.Dim,    "Pa*s",     (u, cv) => new DynamicViscosity(u, cv));
            Internal.SubclassRegistry.Register(KinematicViscosity.Dim,  "m^2/s",    (u, cv) => new KinematicViscosity(u, cv));
            Internal.SubclassRegistry.Register(ThermalConductivity.Dim, "W/(m*K)",  (u, cv) => new ThermalConductivity(u, cv));
            Internal.SubclassRegistry.Register(SpecificHeatCapacity.Dim,"J/(kg*K)", (u, cv) => new SpecificHeatCapacity(u, cv));
            Internal.SubclassRegistry.Register(HeatCapacity.Dim,        "J/K",      (u, cv) => new HeatCapacity(u, cv));
            Internal.SubclassRegistry.Register(HeatFluxDensity.Dim,     "W/m^2",    (u, cv) => new HeatFluxDensity(u, cv));
            Internal.SubclassRegistry.Register(Momentum.Dim,            "kg*m/s",   (u, cv) => new Momentum(u, cv));
            Internal.SubclassRegistry.Register(SurfaceTension.Dim,      "N/m",      (u, cv) => new SurfaceTension(u, cv));
            Internal.SubclassRegistry.Register(SpecificEnergy.Dim,      "J/kg",     (u, cv) => new SpecificEnergy(u, cv));
            // Note: EnergyDensity shares dim with Pressure (already registered); SpecificVolume:
            Internal.SubclassRegistry.Register(SpecificVolume.Dim,      "m^3/kg",   (u, cv) => new SpecificVolume(u, cv));
            Internal.SubclassRegistry.Register(AreaDensity.Dim,         "kg/m^2",   (u, cv) => new AreaDensity(u, cv));
            // Tier C — Electrical
            Internal.SubclassRegistry.Register(ElectricCurrent.Dim,        "A", (u, cv) => new ElectricCurrent(u, cv));
            Internal.SubclassRegistry.Register(ElectricCharge.Dim,         "C", (u, cv) => new ElectricCharge(u, cv));
            Internal.SubclassRegistry.Register(Voltage.Dim,                "V", (u, cv) => new Voltage(u, cv));
            Internal.SubclassRegistry.Register(ElectricResistance.Dim,     "Ω", (u, cv) => new ElectricResistance(u, cv));
            Internal.SubclassRegistry.Register(ElectricalConductance.Dim,  "S", (u, cv) => new ElectricalConductance(u, cv));
            Internal.SubclassRegistry.Register(ElectricCapacitance.Dim,    "F", (u, cv) => new ElectricCapacitance(u, cv));
            Internal.SubclassRegistry.Register(Inductance.Dim,             "H", (u, cv) => new Inductance(u, cv));
            // Molar
            Internal.SubclassRegistry.Register(AmountOfSubstance.Dim,      "mol",        (u, cv) => new AmountOfSubstance(u, cv));
            Internal.SubclassRegistry.Register(MolarMass.Dim,              "kg/mol",     (u, cv) => new MolarMass(u, cv));
            Internal.SubclassRegistry.Register(MolarEnergy.Dim,            "J/mol",      (u, cv) => new MolarEnergy(u, cv));
            Internal.SubclassRegistry.Register(MolarHeatCapacity.Dim,      "J/(mol*K)",  (u, cv) => new MolarHeatCapacity(u, cv));
            Internal.SubclassRegistry.Register(MolarDensity.Dim,           "mol/m^3",    (u, cv) => new MolarDensity(u, cv));
            // Dimensionless
            Internal.SubclassRegistry.Register(DimensionSignature.Dimensionless, "1", (u, cv) => new DimensionlessQuantity(u, cv));
            _ = R;
        }

        /// <summary>
        /// Attempts to look up a unit by symbol, long name, or registered alias.
        /// </summary>
        /// <remarks>
        /// Lookup order:
        /// <list type="number">
        ///   <item>Exact symbol match, case-sensitive (so <c>"m"</c> ≠ <c>"M"</c>).</item>
        ///   <item>Long-name match, case-insensitive (so <c>"foot"</c>, <c>"Foot"</c>, <c>"FOOT"</c> all match the unit whose <see cref="Unit.LongName"/> is <c>"foot"</c>).</item>
        ///   <item>Alias match (irregular plurals and common alternate forms), case-insensitive — e.g. <c>"feet"</c>, <c>"inches"</c>.</item>
        /// </list>
        /// Returns false if nothing matches.
        /// </remarks>
        public static bool TryGet(string symbol, out Unit unit)
        {
            if (symbol is null) { unit = default; return false; }
            if (_units.TryGetValue(symbol, out unit)) return true;
            if (_byLongName.TryGetValue(symbol, out unit)) return true;
            if (_aliases.TryGetValue(symbol, out var canonicalLongName)
                && _byLongName.TryGetValue(canonicalLongName, out unit))
                return true;
            unit = default;
            return false;
        }

        /// <summary>
        /// Looks up a unit by symbol, long name, or alias. Uses the same resolution
        /// order as <see cref="TryGet"/>.
        /// </summary>
        /// <exception cref="UnknownUnitException">If nothing matches.</exception>
        public static Unit Get(string symbol)
        {
            if (symbol is null) throw new ArgumentNullException(nameof(symbol));
            if (!TryGet(symbol, out var u)) throw new UnknownUnitException(symbol);
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

        /// <summary>
        /// Builds the case-insensitive long-name index after all primary entries have been
        /// added. First-write-wins: if multiple units share a long name (rare; mostly happens
        /// with aliased symbols like "degC"/"°C" both saying "degree Celsius"), the
        /// first-seeded one is what the long-name lookup returns.
        /// </summary>
        private static void BuildLongNameIndex()
        {
            foreach (var u in _units.Values)
            {
                if (!string.IsNullOrEmpty(u.LongName) && !_byLongName.ContainsKey(u.LongName))
                {
                    _byLongName[u.LongName] = u;
                }
            }
        }

        /// <summary>
        /// Seeds common alternate-form aliases (mostly irregular plurals like "feet" -> "foot")
        /// that English doesn't derive mechanically. Each alias maps to the canonical long name
        /// the corresponding unit was registered with.
        /// </summary>
        private static void SeedAliases()
        {
            // Length
            _aliases["feet"]        = "foot";
            _aliases["inches"]      = "inch";
            _aliases["yards"]       = "yard";
            _aliases["miles"]       = "mile";
            _aliases["meters"]      = "meter";
            _aliases["metres"]      = "meter";          // British spelling
            _aliases["metre"]       = "meter";
            _aliases["kilometers"]  = "kilometer";
            _aliases["kilometres"]  = "kilometer";
            _aliases["kilometre"]   = "kilometer";
            _aliases["centimeters"] = "centimeter";
            _aliases["centimetres"] = "centimeter";
            _aliases["centimetre"]  = "centimeter";
            _aliases["millimeters"] = "millimeter";
            _aliases["millimetres"] = "millimeter";
            _aliases["millimetre"]  = "millimeter";
            _aliases["micrometers"] = "micrometer";
            _aliases["nanometers"]  = "nanometer";

            // Mass
            _aliases["kilograms"]   = "kilogram";
            _aliases["grams"]       = "gram";
            _aliases["milligrams"]  = "milligram";
            _aliases["micrograms"]  = "microgram";
            _aliases["pounds"]      = "pound";
            _aliases["ounces"]      = "ounce";
            _aliases["tonnes"]      = "tonne";
            _aliases["tons"]        = "short ton";
            _aliases["slugs"]       = "slug";

            // Time
            _aliases["seconds"]      = "second";
            _aliases["milliseconds"] = "millisecond";
            _aliases["microseconds"] = "microsecond";
            _aliases["nanoseconds"]  = "nanosecond";
            _aliases["minutes"]      = "minute";
            _aliases["hours"]        = "hour";
            _aliases["days"]         = "day";
            _aliases["weeks"]        = "week";
            _aliases["years"]        = "year";

            // Temperature — long names contain spaces, so the alias gives a single-word handle too
            _aliases["celsius"]    = "degree Celsius";
            _aliases["fahrenheit"] = "degree Fahrenheit";
            _aliases["rankine"]    = "degree Rankine";
            _aliases["kelvins"]    = "kelvin";

            // Amount of substance
            _aliases["moles"]      = "mole";
            _aliases["kilomoles"]  = "kilomole";
            _aliases["lbmoles"]    = "pound-mole";
            _aliases["lb-mol"]     = "pound-mole";    // hyphenated form some engineers use

            // Angle
            _aliases["radians"]      = "radian";
            _aliases["degrees"]      = "degree";
            _aliases["revolutions"]  = "revolution";
            _aliases["gradians"]     = "gradian";

            // Area
            _aliases["acres"]    = "acre";
            _aliases["hectares"] = "hectare";

            // Volume
            _aliases["liters"]     = "liter";
            _aliases["litres"]     = "liter";
            _aliases["litre"]      = "liter";
            _aliases["milliliters"]= "milliliter";
            _aliases["millilitres"]= "milliliter";
            _aliases["gallons"]    = "US gallon";
            _aliases["quarts"]     = "US quart";
            _aliases["pints"]      = "US pint";
            _aliases["cups"]       = "US cup";
            _aliases["barrels"]    = "barrel (petroleum)";

            // Force
            _aliases["newtons"]     = "newton";
            _aliases["kilonewtons"] = "kilonewton";
            _aliases["dynes"]       = "dyne";

            // Pressure
            _aliases["pascals"]    = "pascal";
            _aliases["kilopascals"]= "kilopascal";
            _aliases["megapascals"]= "megapascal";
            _aliases["bars"]       = "bar";
            _aliases["atmospheres"]= "standard atmosphere";

            // Energy
            _aliases["joules"]    = "joule";
            _aliases["kilojoules"]= "kilojoule";
            _aliases["megajoules"]= "megajoule";
            _aliases["calories"]  = "calorie (thermochemical)";
            _aliases["kilocalories"]= "kilocalorie";

            // Power
            _aliases["watts"]      = "watt";
            _aliases["kilowatts"]  = "kilowatt";
            _aliases["megawatts"]  = "megawatt";
            _aliases["horsepower"] = "mechanical horsepower";

            // Frequency
            _aliases["hertz"]     = "hertz";

            // Electrical
            _aliases["amperes"]    = "ampere";
            _aliases["amps"]       = "ampere";
            _aliases["volts"]      = "volt";
            _aliases["ohms"]       = "ohm";
            _aliases["farads"]     = "farad";
            _aliases["henries"]    = "henry";
            _aliases["henrys"]     = "henry";
            _aliases["coulombs"]   = "coulomb";
        }

        // ── Catalog ──────────────────────────────────────────────────

        private static void SeedDimensionless()
        {
            Add("1", "dimensionless", DimensionSignature.Dimensionless, 1.0);
            // Interpretation symbols (Re, Ma, Fr, etc.) are registered as dimensionless
            // units for the catalog, but DimensionlessQuantity.PreferredInterpretation
            // is what drives display.
        }

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
            Add("Da",   "dalton",     M, 1.66053906660e-27);  // CODATA 2018 / 2019 SI
            Add("u",    "atomic mass unit", M, 1.66053906660e-27);
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
            Add("HP",      "mechanical horsepower", P, 745.6998715822702);
            Add("BHP",     "brake horsepower",      P, 745.6998715822702);
            Add("bhp",     "brake horsepower",      P, 745.6998715822702);
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
            // Normal cubic meter per hour: geometric scale identical to m^3/h.
            // The "N" prefix is informational (denotes that the volume is measured
            // at a normal base condition such as 0 °C / 1 atm). EU does not apply
            // a base-correction factor — see how MMSCFD / MMSCFD_petro / MMSCFD_iupac
            // all share the same geometric scale.
            Add("Nm^3/h",  "normal cubic meter per hour (geometric scale identical to m^3/h)", Vf, 1.0 / 3600.0);
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

            // MMSCFD: million standard cubic feet per day. Geometric flow conversion
            // is the same regardless of "standard" conditions (the conditions matter
            // when converting to MASS flow via the ideal gas law, not for volumetric).
            // The variants are bookkeeping: they record which standard the value is
            // measured at, so a downstream conversion to mass uses the right basis.
            // See specification §5.4 and Decision 14.12.
            var mmscfdScale = 1e6 * (0.3048 * 0.3048 * 0.3048) / 86400.0;
            Add("MMSCFD",       "million standard cubic feet per day (14.73 psia, 60 °F — GPSA/AGA)", Vf, mmscfdScale);
            Add("MMSCFD_petro", "million standard cubic feet per day (14.696 psia, 60 °F — petroleum)", Vf, mmscfdScale);
            Add("MMSCFD_iupac", "million standard cubic feet per day (100 kPa, 0 °C — IUPAC)",       Vf, mmscfdScale);
            Add("MSCFD",        "thousand standard cubic feet per day (14.73 psia, 60 °F)",         Vf, mmscfdScale / 1000.0);
            Add("SCFD",         "standard cubic feet per day (14.73 psia, 60 °F)",                  Vf, mmscfdScale / 1e6);
            Add("SCFM",         "standard cubic feet per minute (14.73 psia, 60 °F)",              Vf, (0.3048 * 0.3048 * 0.3048) / 60.0);
            Add("SCFH",         "standard cubic feet per hour (14.73 psia, 60 °F)",                Vf, (0.3048 * 0.3048 * 0.3048) / 3600.0);
            Add("MCFH",         "thousand standard cubic feet per hour (14.73 psia, 60 °F)",       Vf, (0.3048 * 0.3048 * 0.3048) * 1000.0 / 3600.0);
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

        private static void SeedDynamicViscosity()
        {
            // M / (L * T)
            var Dv = DimensionSignature.Mass - DimensionSignature.Length - DimensionSignature.Time;
            Add("Pa*s",  "pascal-second",        Dv, 1.0);
            Add("Pa*s_alt","pascal-second",      Dv, 1.0); // placeholder for alt symbol (not used)
            Add("mPa*s", "millipascal-second",   Dv, 1e-3);
            Add("P",     "poise",                Dv, 0.1);
            Add("cP",    "centipoise",           Dv, 1e-3);   // 1 cP = 1 mPa*s
            Add("lb/(ft*s)", "pound per foot-second", Dv, 0.45359237 / 0.3048);
            Add("lb/(ft*hr)","pound per foot-hour",   Dv, 0.45359237 / 0.3048 / 3600.0);
        }

        private static void SeedKinematicViscosity()
        {
            // L^2 / T
            var Kv = DimensionSignature.Length * 2 - DimensionSignature.Time;
            Add("m^2/s",  "square meter per second", Kv, 1.0);
            Add("m²/s",   "square meter per second", Kv, 1.0);
            Add("mm^2/s", "square millimeter per second", Kv, 1e-6);
            Add("mm²/s",  "square millimeter per second", Kv, 1e-6);
            Add("St",     "stokes",                  Kv, 1e-4);  // 1 St = 1 cm^2/s = 1e-4 m^2/s
            Add("cSt",    "centistokes",             Kv, 1e-6);  // 1 cSt = 1 mm^2/s
            Add("ft^2/s", "square foot per second",  Kv, 0.3048 * 0.3048);
            Add("ft²/s",  "square foot per second",  Kv, 0.3048 * 0.3048);
        }

        private static void SeedThermalConductivity()
        {
            // L * M / (T^3 * Θ)
            var Tc = DimensionSignature.Length + DimensionSignature.Mass - DimensionSignature.Time * 3 - DimensionSignature.Temperature;
            Add("W/(m*K)",      "watt per meter-kelvin",         Tc, 1.0);
            Add("W/(m·K)",      "watt per meter-kelvin",         Tc, 1.0);
            Add("kW/(m*K)",     "kilowatt per meter-kelvin",     Tc, 1000.0);
            Add("mW/(m*K)",     "milliwatt per meter-kelvin",    Tc, 1e-3);
            Add("BTU/(hr*ft*degF)", "BTU per hour-foot-degree-Fahrenheit", Tc, 1.730734666374);
            Add("BTU/(hr*ft*°F)",   "BTU per hour-foot-degree-Fahrenheit", Tc, 1.730734666374);
            Add("cal/(s*cm*degC)",  "calorie per second-centimeter-degree-Celsius", Tc, 418.4);
        }

        private static void SeedSpecificHeatCapacity()
        {
            // L^2 / (T^2 * Θ)
            var Shc = DimensionSignature.Length * 2 - DimensionSignature.Time * 2 - DimensionSignature.Temperature;
            Add("J/(kg*K)", "joule per kilogram-kelvin", Shc, 1.0);
            Add("J/(kg·K)", "joule per kilogram-kelvin", Shc, 1.0);
            Add("kJ/(kg*K)","kilojoule per kilogram-kelvin", Shc, 1000.0);
            Add("kJ/(kg·K)","kilojoule per kilogram-kelvin", Shc, 1000.0);
            Add("cal/(g*degC)", "calorie per gram-degree-Celsius", Shc, 4184.0);
            Add("BTU/(lb*degF)","BTU per pound-degree-Fahrenheit", Shc, 4186.8);
            Add("BTU/(lb*°F)",  "BTU per pound-degree-Fahrenheit", Shc, 4186.8);
        }

        private static void SeedHeatCapacity()
        {
            // L^2 * M / (T^2 * Θ)
            var Hc = DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 2 - DimensionSignature.Temperature;
            Add("J/K",      "joule per kelvin",                Hc, 1.0);
            Add("kJ/K",     "kilojoule per kelvin",            Hc, 1000.0);
            Add("BTU/degF", "BTU per degree-Fahrenheit",       Hc, 1899.10073600);
            Add("BTU/°F",   "BTU per degree-Fahrenheit",       Hc, 1899.10073600);
            Add("cal/K",    "calorie per kelvin",              Hc, 4.184);
        }

        private static void SeedHeatFluxDensity()
        {
            // M / T^3
            var Hf = DimensionSignature.Mass - DimensionSignature.Time * 3;
            Add("W/m^2",       "watt per square-meter",      Hf, 1.0);
            Add("W/m²",        "watt per square-meter",      Hf, 1.0);
            Add("kW/m^2",      "kilowatt per square-meter",  Hf, 1000.0);
            Add("kW/m²",       "kilowatt per square-meter",  Hf, 1000.0);
            Add("BTU/(hr*ft^2)","BTU per hour-square-foot",  Hf, 3.15459074506);
            Add("BTU/(hr*ft²)", "BTU per hour-square-foot",  Hf, 3.15459074506);
            Add("cal/(s*cm^2)", "calorie per second-square-centimeter", Hf, 41840.0);
        }

        private static void SeedMomentum()
        {
            // L * M / T
            var P = DimensionSignature.Length + DimensionSignature.Mass - DimensionSignature.Time;
            Add("kg*m/s", "kilogram-meter per second", P, 1.0);
            Add("kg·m/s", "kilogram-meter per second", P, 1.0);
            Add("N*s",    "newton-second",             P, 1.0);
            Add("N·s",    "newton-second",             P, 1.0);
            Add("lb*ft/s","pound-foot per second",     P, 0.45359237 * 0.3048);
            Add("slug*ft/s","slug-foot per second",    P, 14.59390294 * 0.3048);
            Add("lbf*s",  "pound-force second",        P, 4.4482216152605);
        }

        private static void SeedSurfaceTension()
        {
            // M / T^2 (= force per length, dimensionally)
            var St = DimensionSignature.Mass - DimensionSignature.Time * 2;
            Add("N/m",    "newton per meter",    St, 1.0);
            Add("mN/m",   "millinewton per meter", St, 1e-3);
            Add("dyn/cm", "dyne per centimeter", St, 1e-3);   // 1 dyn/cm = 1 mN/m
            Add("lbf/ft", "pound-force per foot", St, 4.4482216152605 / 0.3048);
            Add("lbf/in", "pound-force per inch", St, 4.4482216152605 / 0.0254);
        }

        private static void SeedEnergyPerArea()
        {
            // M / T^2 — same dimension as SurfaceTension; symbols here are the
            // fracture-mechanics / surface-energy spellings. Lookup is by symbol,
            // so adding J/m^2 here does not conflict with N/m already seeded for
            // SurfaceTension (they're numerically identical: 1 J/m² = 1 N/m).
            var Ea = DimensionSignature.Mass - DimensionSignature.Time * 2;
            Add("J/m^2",       "joule per square meter",         Ea, 1.0);
            Add("J/m²",        "joule per square meter",         Ea, 1.0);
            Add("kJ/m^2",      "kilojoule per square meter",     Ea, 1000.0);
            Add("kJ/m²",       "kilojoule per square meter",     Ea, 1000.0);
            Add("MJ/m^2",      "megajoule per square meter",     Ea, 1e6);
            Add("MJ/m²",       "megajoule per square meter",     Ea, 1e6);
            Add("J/cm^2",      "joule per square centimeter",    Ea, 1e4);
            Add("J/cm²",       "joule per square centimeter",    Ea, 1e4);
            Add("N/mm",        "newton per millimeter",          Ea, 1000.0);
            // 1 ft·lbf = 1.355817948331 J; 1 in² = 0.00064516 m². Factor: 2101.452043...
            Add("ft*lbf/in^2", "foot-pound-force per square inch", Ea, 1.355817948331 / 0.00064516);
            Add("ft·lbf/in²",  "foot-pound-force per square inch", Ea, 1.355817948331 / 0.00064516);
            Add("ft-lb/in^2",  "foot-pound per square inch",       Ea, 1.355817948331 / 0.00064516);
            Add("ft-lb/in²",   "foot-pound per square inch",       Ea, 1.355817948331 / 0.00064516);
        }

        private static void SeedSpecificEnergy()
        {
            // L^2 / T^2
            var Se = DimensionSignature.Length * 2 - DimensionSignature.Time * 2;
            Add("J/kg",   "joule per kilogram",     Se, 1.0);
            Add("kJ/kg",  "kilojoule per kilogram", Se, 1000.0);
            Add("MJ/kg",  "megajoule per kilogram", Se, 1e6);
            Add("BTU/lb", "BTU per pound",          Se, 2326.0);
            Add("BTU/lbm","BTU per pound-mass",     Se, 2326.0);
            Add("cal/g",  "calorie per gram",       Se, 4184.0);
            Add("kcal/kg","kilocalorie per kilogram", Se, 4184.0);
            Add("Wh/kg",  "watt-hour per kilogram", Se, 3600.0);
            Add("kWh/kg", "kilowatt-hour per kilogram", Se, 3.6e6);
            // Compressor head — ft·lbf per lbm. 1 ft·lbf = 1.355817948331 J; 1 lbm = 0.45359237 kg.
            Add("ft*lbf/lbm", "foot-pound-force per pound-mass", Se, 1.355817948331 / 0.45359237);
            Add("ft·lbf/lbm", "foot-pound-force per pound-mass", Se, 1.355817948331 / 0.45359237);
        }

        private static void SeedEnergyDensity()
        {
            // M / (L * T^2) — same dimension as Pressure
            var Ed = DimensionSignature.Mass - DimensionSignature.Length - DimensionSignature.Time * 2;
            Add("J/m^3",   "joule per cubic-meter",     Ed, 1.0);
            Add("J/m³",    "joule per cubic-meter",     Ed, 1.0);
            Add("kJ/m^3",  "kilojoule per cubic-meter", Ed, 1000.0);
            Add("MJ/m^3",  "megajoule per cubic-meter", Ed, 1e6);
            Add("BTU/ft^3","BTU per cubic-foot",        Ed, 37258.94576);
            Add("BTU/ft³", "BTU per cubic-foot",        Ed, 37258.94576);
            // SCF is the same geometric volume as ft^3 (the "standard" prefix only
            // documents the (P,T) at which the gas amount is referenced — it does
            // not change the geometric conversion factor). Likewise Nm^3 is the
            // same geometric volume as m^3. Per natural-gas industry convention,
            // heating values are reported as BTU/SCF (USC) or MJ/Nm^3 (SIC).
            Add("BTU/SCF", "BTU per standard cubic foot", Ed, 37258.94576);
            Add("BTU/scf", "BTU per standard cubic foot", Ed, 37258.94576);
            Add("MJ/Nm^3", "megajoule per normal cubic meter", Ed, 1e6);
            Add("MJ/m^3",  "megajoule per cubic meter",   Ed, 1e6);
            Add("kJ/Nm^3", "kilojoule per normal cubic meter", Ed, 1000.0);
            Add("Wh/L",    "watt-hour per liter",       Ed, 3.6e6);
            Add("kWh/L",   "kilowatt-hour per liter",   Ed, 3.6e9);
        }

        private static void SeedSpecificVolume()
        {
            // L^3 / M (reciprocal of density)
            var Sv = DimensionSignature.Length * 3 - DimensionSignature.Mass;
            Add("m^3/kg", "cubic-meter per kilogram", Sv, 1.0);
            Add("m³/kg",  "cubic-meter per kilogram", Sv, 1.0);
            Add("L/kg",   "liter per kilogram",       Sv, 1e-3);
            Add("ft^3/lb","cubic-foot per pound",     Sv, (0.3048 * 0.3048 * 0.3048) / 0.45359237);
            Add("ft³/lb", "cubic-foot per pound",     Sv, (0.3048 * 0.3048 * 0.3048) / 0.45359237);
            Add("cm^3/g", "cubic-centimeter per gram", Sv, 1e-3);
        }

        private static void SeedAreaDensity()
        {
            // M / L^2
            var Ad = DimensionSignature.Mass - DimensionSignature.Length * 2;
            Add("kg/m^2",  "kilogram per square-meter", Ad, 1.0);
            Add("kg/m²",   "kilogram per square-meter", Ad, 1.0);
            Add("g/m^2",   "gram per square-meter",     Ad, 1e-3);
            Add("g/m²",    "gram per square-meter",     Ad, 1e-3);
            Add("lb/ft^2", "pound per square-foot",     Ad, 0.45359237 / (0.3048 * 0.3048));
            Add("lb/ft²",  "pound per square-foot",     Ad, 0.45359237 / (0.3048 * 0.3048));
            Add("oz/yd^2", "ounce per square-yard",     Ad, 0.028349523125 / (0.9144 * 0.9144));
        }

        private static void SeedElectricCurrent()
        {
            var I = DimensionSignature.ElectricCurrent;
            Add("A",  "ampere",      I, 1.0);
            Add("mA", "milliampere", I, 1e-3);
            Add("μA", "microampere", I, 1e-6);
            Add("uA", "microampere", I, 1e-6);
            Add("nA", "nanoampere",  I, 1e-9);
            Add("kA", "kiloampere",  I, 1000.0);
        }

        private static void SeedElectricCharge()
        {
            // T * A
            var Q = DimensionSignature.Time + DimensionSignature.ElectricCurrent;
            Add("C",   "coulomb",      Q, 1.0);
            Add("mC",  "millicoulomb", Q, 1e-3);
            Add("μC",  "microcoulomb", Q, 1e-6);
            Add("uC",  "microcoulomb", Q, 1e-6);
            Add("nC",  "nanocoulomb",  Q, 1e-9);
            Add("pC",  "picocoulomb",  Q, 1e-12);
            Add("Ah",  "ampere-hour",  Q, 3600.0);
            Add("mAh", "milliampere-hour", Q, 3.6);
        }

        private static void SeedVoltage()
        {
            // L^2 * M / (T^3 * A)
            var V = DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 3 - DimensionSignature.ElectricCurrent;
            Add("V",  "volt",       V, 1.0);
            Add("mV", "millivolt",  V, 1e-3);
            Add("μV", "microvolt",  V, 1e-6);
            Add("uV", "microvolt",  V, 1e-6);
            Add("kV", "kilovolt",   V, 1000.0);
            Add("MV", "megavolt",   V, 1e6);
        }

        private static void SeedElectricResistance()
        {
            // L^2 * M / (T^3 * A^2)
            var R = DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 3 - DimensionSignature.ElectricCurrent * 2;
            Add("Ω",   "ohm",      R, 1.0);
            Add("ohm", "ohm",      R, 1.0);
            Add("mΩ",  "milliohm", R, 1e-3);
            Add("kΩ",  "kilohm",   R, 1000.0);
            Add("MΩ",  "megohm",   R, 1e6);
            Add("GΩ",  "gigohm",   R, 1e9);
        }

        private static void SeedElectricalConductance()
        {
            // 1 / (L^2 * M / (T^3 * A^2)) = T^3 * A^2 / (L^2 * M)
            var S = -(DimensionSignature.Length * 2) - DimensionSignature.Mass + DimensionSignature.Time * 3 + DimensionSignature.ElectricCurrent * 2;
            Add("S",  "siemens",      S, 1.0);
            Add("mS", "millisiemens", S, 1e-3);
            Add("μS", "microsiemens", S, 1e-6);
            Add("uS", "microsiemens", S, 1e-6);
        }

        private static void SeedElectricCapacitance()
        {
            // T^4 * A^2 / (L^2 * M)
            var F = -(DimensionSignature.Length * 2) - DimensionSignature.Mass + DimensionSignature.Time * 4 + DimensionSignature.ElectricCurrent * 2;
            Add("F",   "farad",      F, 1.0);
            Add("mF",  "millifarad", F, 1e-3);
            Add("μF",  "microfarad", F, 1e-6);
            Add("uF",  "microfarad", F, 1e-6);
            Add("nF",  "nanofarad",  F, 1e-9);
            Add("pF",  "picofarad",  F, 1e-12);
        }

        private static void SeedInductance()
        {
            // L^2 * M / (T^2 * A^2)
            var H = DimensionSignature.Length * 2 + DimensionSignature.Mass - DimensionSignature.Time * 2 - DimensionSignature.ElectricCurrent * 2;
            Add("H",   "henry",      H, 1.0);
            Add("mH",  "millihenry", H, 1e-3);
            Add("μH",  "microhenry", H, 1e-6);
            Add("uH",  "microhenry", H, 1e-6);
            Add("nH",  "nanohenry",  H, 1e-9);
            Add("pH",  "picohenry",  H, 1e-12);
        }

        private static void SeedAmountOfSubstance()
        {
            var N = DimensionSignature.AmountOfSubstance;
            Add("mol",   "mole",       N, 1.0);
            Add("kmol",  "kilomole",   N, 1000.0);
            // 1 lbmol = "the mass in pounds equal to the molecular weight" expressed
            // in moles → 1 lbmol = 453.59237 mol (same factor as lb→g).
            Add("lbmol", "pound-mole", N, 453.59237);
        }

        private static void SeedMolarMass()
        {
            // M / N. Canonical: kg/mol.
            // Numerically g/mol = kg/kmol = lb/lbmol (the value of "molecular weight"
            // doesn't depend on which mole convention you use).
            var MM = DimensionSignature.Mass - DimensionSignature.AmountOfSubstance;
            Add("kg/mol",   "kilogram per mole",      MM, 1.0);
            Add("g/mol",    "gram per mole",          MM, 1e-3);
            Add("kg/kmol",  "kilogram per kilomole",  MM, 1e-3);
            Add("lb/lbmol", "pound per pound-mole",   MM, 1e-3);
        }

        private static void SeedMolarEnergy()
        {
            // (L^2 * M / T^2) / N. Canonical: J/mol.
            var ME = (DimensionSignature.Length * 2) + DimensionSignature.Mass
                     - (DimensionSignature.Time * 2) - DimensionSignature.AmountOfSubstance;
            Add("J/mol",     "joule per mole",        ME, 1.0);
            Add("kJ/mol",    "kilojoule per mole",    ME, 1000.0);
            Add("kJ/kmol",   "kilojoule per kilomole", ME, 1.0);          // numerically = J/mol
            Add("J/kmol",    "joule per kilomole",    ME, 1e-3);
            // 1 BTU/lbmol = 1055.05585 J / 453.59237 mol = 2.32600 J/mol
            Add("BTU/lbmol", "BTU per pound-mole",    ME, 1055.05585262 / 453.59237);
            // Cal-based variants (engineering / spectroscopy)
            Add("cal/mol",   "calorie per mole",      ME, 4.184);
            Add("kcal/mol",  "kilocalorie per mole",  ME, 4184.0);
        }

        private static void SeedMolarHeatCapacity()
        {
            // (L^2 * M / T^2) / (N * Θ). Canonical: J/(mol·K). Also molar entropy.
            var MHC = (DimensionSignature.Length * 2) + DimensionSignature.Mass
                      - (DimensionSignature.Time * 2)
                      - DimensionSignature.AmountOfSubstance - DimensionSignature.Temperature;
            Add("J/(mol*K)",       "joule per mole-kelvin",                 MHC, 1.0);
            Add("J/mol-K",         "joule per mole-kelvin",                 MHC, 1.0);
            Add("kJ/(mol*K)",      "kilojoule per mole-kelvin",             MHC, 1000.0);
            Add("kJ/(kmol*K)",     "kilojoule per kilomole-kelvin",         MHC, 1.0);   // numerically = J/(mol*K)
            Add("J/(kmol*K)",      "joule per kilomole-kelvin",             MHC, 1e-3);
            Add("BTU/(lbmol*R)",   "BTU per pound-mole-Rankine",            MHC, 1055.05585262 / 453.59237 * 1.8);
            Add("BTU/lbmol-R",     "BTU per pound-mole-Rankine",            MHC, 1055.05585262 / 453.59237 * 1.8);
            // ASCII spellings using "degR" — same unit, alternate symbol convention used by
            // some engineering style guides (parallels degF/degC).
            Add("BTU/(lbmol*degR)","BTU per pound-mole-Rankine",            MHC, 1055.05585262 / 453.59237 * 1.8);
            Add("BTU/lbmol-degR",  "BTU per pound-mole-Rankine",            MHC, 1055.05585262 / 453.59237 * 1.8);
            Add("cal/(mol*K)",     "calorie per mole-kelvin",               MHC, 4.184);
        }

        private static void SeedJouleThomson()
        {
            // Joule-Thomson coefficient: temperature change per pressure change at
            // constant enthalpy. Dimension = Θ / (M·L⁻¹·T⁻²) = Θ·L·M⁻¹·T².
            // Canonical: K/Pa.
            // Note: temperature DIFFERENCES are unit-equivalent K↔degC and degR↔degF,
            // so "K/Pa" and "degC/Pa" are the same scale; "degR/psi" and "degF/psi"
            // are the same scale. The named units below cover both the natural-gas
            // engineering convention (degF/psi, K/bar) and the EoS canonical convention
            // (K/kPa).
            var JT = DimensionSignature.Temperature
                     - DimensionSignature.Mass + DimensionSignature.Length
                     + DimensionSignature.Time * 2;
            Add("K/Pa",     "kelvin per pascal",          JT, 1.0);
            Add("K/kPa",    "kelvin per kilopascal",      JT, 1e-3);
            Add("K/MPa",    "kelvin per megapascal",      JT, 1e-6);
            Add("K/bar",    "kelvin per bar",             JT, 1e-5);
            Add("K/mbar",   "kelvin per millibar",        JT, 0.01);
            Add("degC/bar", "degree Celsius per bar",     JT, 1e-5);    // ΔT same scale K↔degC
            Add("degC/kPa", "degree Celsius per kilopascal", JT, 1e-3);
            Add("degF/psi", "degree Fahrenheit per psi",  JT, (5.0 / 9.0) / 6894.757293168);
            Add("degR/psi", "degree Rankine per psi",     JT, (5.0 / 9.0) / 6894.757293168);
        }

        private static void SeedMolarDensity()
        {
            // N / L^3. Canonical: mol/m^3. Conjugate of MolarMass when paired with
            // mass-based Density via the mixture's molar mass.
            var MD = DimensionSignature.AmountOfSubstance - (DimensionSignature.Length * 3);
            Add("mol/m^3",   "mole per cubic meter",        MD, 1.0);
            Add("mol/L",     "mole per liter",              MD, 1000.0);
            Add("kmol/m^3",  "kilomole per cubic meter",    MD, 1000.0);   // numerically = mol/L
            Add("mol/dm^3",  "mole per cubic decimeter",    MD, 1000.0);   // numerically = mol/L
            Add("lbmol/ft^3","pound-mole per cubic foot",   MD, 453.59237 / (0.3048 * 0.3048 * 0.3048));
        }
    }
}
