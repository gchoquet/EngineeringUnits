using Xunit;

namespace EngineeringUnits.Tests
{
    public class TierAEngineeringQuantityTests
    {
        // ── DynamicViscosity ────────────────────────────────────────
        [Theory]
        [InlineData(1.0,    "Pa*s",  1.0,        "Pa*s")]
        [InlineData(1.0,    "cP",    1e-3,       "Pa*s")]  // 1 cP = 1 mPa·s
        [InlineData(1000.0, "cP",    1.0,        "Pa*s")]
        [InlineData(1.0,    "P",     100.0,      "cP")]    // 1 P = 100 cP
        public void DynamicViscosity_Conversions(double v, string from, double expected, string to)
        {
            Assert.Equal(expected, new DynamicViscosity(v, from).As(to), 9);
        }

        // ── KinematicViscosity ──────────────────────────────────────
        [Theory]
        [InlineData(1.0, "cSt",  1e-6,    "m^2/s")]   // 1 cSt = 1 mm²/s
        [InlineData(1.0, "St",   100.0,   "cSt")]
        [InlineData(1.0, "m^2/s", 1e6,    "cSt")]
        public void KinematicViscosity_Conversions(double v, string from, double expected, string to)
        {
            Assert.Equal(expected, new KinematicViscosity(v, from).As(to), 9);
        }

        // ── ThermalConductivity ─────────────────────────────────────
        [Theory]
        [InlineData(1.0, "W/(m*K)", 1.0, "W/(m*K)")]
        [InlineData(1.0, "BTU/(hr*ft*degF)", 1.730734666374, "W/(m*K)")]
        public void ThermalConductivity_Conversions(double v, string from, double expected, string to)
        {
            Assert.Equal(expected, new ThermalConductivity(v, from).As(to), 6);
        }

        // ── SpecificHeatCapacity ────────────────────────────────────
        [Theory]
        [InlineData(1.0, "J/(kg*K)",    1.0,    "J/(kg*K)")]
        [InlineData(1.0, "cal/(g*degC)", 4184.0, "J/(kg*K)")]
        public void SpecificHeatCapacity_Conversions(double v, string from, double expected, string to)
        {
            Assert.Equal(expected, new SpecificHeatCapacity(v, from).As(to), 6);
        }

        // ── HeatCapacity ────────────────────────────────────────────
        [Fact]
        public void HeatCapacity_OverMass_GivesSpecificHeatCapacity()
        {
            var hc = new HeatCapacity(4184, "J/K");          // ~heat capacity of 1 kg water
            var m = new Mass(1, "kg");
            var shc = hc / m;
            Assert.IsType<SpecificHeatCapacity>(shc);
            Assert.Equal(4184.0, shc.CanonicalValue, 6);
        }

        // ── HeatFluxDensity ─────────────────────────────────────────
        [Theory]
        [InlineData(1.0, "W/m^2",  1.0, "W/m^2")]
        [InlineData(1.0, "kW/m^2", 1000.0, "W/m^2")]
        public void HeatFluxDensity_Conversions(double v, string from, double expected, string to)
        {
            Assert.Equal(expected, new HeatFluxDensity(v, from).As(to), 9);
        }

        // ── Momentum ────────────────────────────────────────────────
        [Fact]
        public void Momentum_OverTime_IsForce()
        {
            var p = new Momentum(10, "kg*m/s");
            var t = new Time(2, "s");
            var f = p / t;
            Assert.IsType<Force>(f);
            Assert.Equal(5.0, f.CanonicalValue, 9);
        }

        [Fact]
        public void Momentum_OverMass_IsVelocity()
        {
            var p = new Momentum(10, "kg*m/s");
            var m = new Mass(2, "kg");
            var v = p / m;
            Assert.IsType<Velocity>(v);
            Assert.Equal(5.0, v.CanonicalValue, 9);
        }

        // ── SurfaceTension ──────────────────────────────────────────
        [Theory]
        [InlineData(72.8, "mN/m",   72.8, "dyn/cm")]  // pure water at 20°C ≈ 72.8 mN/m = 72.8 dyn/cm
        [InlineData(1.0,  "N/m",    1000.0, "mN/m")]
        public void SurfaceTension_Conversions(double v, string from, double expected, string to)
        {
            Assert.Equal(expected, new SurfaceTension(v, from).As(to), 6);
        }

        // ── SpecificEnergy ──────────────────────────────────────────
        [Fact]
        public void SpecificEnergy_TimesMass_IsEnergy()
        {
            var se = new SpecificEnergy(2326, "J/kg");   // = 1 BTU/lb in SI
            var m = new Mass(1, "lb");
            var e = se * m;
            Assert.IsType<Energy>(e);
            Assert.Equal(1055.05585262, e.As("J"), 4);   // 1 BTU
        }

        [Theory]
        [InlineData(1.0, "BTU/lb", 2326.0, "J/kg")]
        [InlineData(1.0, "kJ/kg", 1000.0, "J/kg")]
        public void SpecificEnergy_Conversions(double v, string from, double expected, string to)
        {
            Assert.Equal(expected, new SpecificEnergy(v, from).As(to), 6);
        }

        // ── EnergyDensity ───────────────────────────────────────────
        [Fact]
        public void EnergyDensity_HasPressureDimension()
        {
            // EnergyDensity and Pressure share dimension M/(L*T^2). They're separate
            // classes by convention (semantic distinction), like Torque vs Energy.
            Assert.Equal(new Pressure(1, "Pa").Dimension, new EnergyDensity(1, "J/m^3").Dimension);
        }

        // ── SpecificVolume ──────────────────────────────────────────
        [Fact]
        public void SpecificVolume_IsReciprocalOfDensity_Numerically()
        {
            var sv = new SpecificVolume(1, "m^3/kg");
            // Same numeric magnitude as density's reciprocal (1 m³/kg ↔ 1 kg/m³)
            Assert.Equal(1.0, sv.CanonicalValue, 9);
        }

        [Fact]
        public void SpecificVolume_TimesMass_IsVolume()
        {
            var sv = new SpecificVolume(0.001, "m^3/kg");   // pure water at STP
            var m = new Mass(1000, "kg");
            var V = sv * m;
            Assert.IsType<Volume>(V);
            Assert.Equal(1.0, V.CanonicalValue, 9);
        }

        // ── AreaDensity ─────────────────────────────────────────────
        [Fact]
        public void AreaDensity_TimesArea_IsMass()
        {
            var ad = new AreaDensity(0.500, "kg/m^2");
            var A = new Area(10, "m^2");
            var m = ad * A;
            Assert.IsType<Mass>(m);
            Assert.Equal(5.0, m.CanonicalValue, 9);
        }

        // ── Viscosity cross-type ────────────────────────────────────
        [Fact]
        public void DynamicViscosity_OverDensity_IsKinematicViscosity()
        {
            // Water at 20°C: dynamic ≈ 1 cP = 1e-3 Pa·s, density ≈ 1000 kg/m³
            // → kinematic ≈ 1e-6 m²/s = 1 cSt
            var mu  = new DynamicViscosity(1.0, "cP");
            var rho = new Density(1000.0, "kg/m^3");
            var nu  = mu / rho;
            Assert.IsType<KinematicViscosity>(nu);
            Assert.Equal(1e-6, nu.CanonicalValue, 9);
            Assert.Equal(1.0, nu.As("cSt"), 9);
        }
    }
}
