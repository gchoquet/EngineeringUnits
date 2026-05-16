using Xunit;

namespace EngineeringUnits.Tests
{
    public class DerivedQuantityTests
    {
        [Theory]
        [InlineData(1.0,    "m^2",  10.7639104167, "ft^2")]
        [InlineData(1.0,    "acre", 4046.8564224,  "m^2")]
        [InlineData(1.0,    "ha",   10000.0,       "m^2")]
        public void Area_Conversions(double v, string fromUnit, double expected, string toUnit)
        {
            Assert.Equal(expected, new Area(v, fromUnit).As(toUnit), 6);
        }

        [Theory]
        [InlineData(1.0, "m^3",  1000.0,        "L")]
        [InlineData(1.0, "gal",  3.785411784,   "L")]
        [InlineData(1.0, "bbl",  158.987294928, "L")]
        [InlineData(1.0, "ft^3", 28.316846592,  "L")]
        public void Volume_Conversions(double v, string fromUnit, double expected, string toUnit)
        {
            Assert.Equal(expected, new Volume(v, fromUnit).As(toUnit), 6);
        }

        [Theory]
        [InlineData(60.0, "mph",  26.8224, "m/s")]
        [InlineData(1.0,  "knot", 0.5144444444, "m/s")]
        [InlineData(1.0,  "km/h", 0.2777777778, "m/s")]
        public void Velocity_Conversions(double v, string fromUnit, double expected, string toUnit)
        {
            Assert.Equal(expected, new Velocity(v, fromUnit).As(toUnit), 6);
        }

        [Theory]
        [InlineData(1.0,  "g_n", 9.80665, "m/s^2")]
        public void Acceleration_StandardGravity(double v, string fromUnit, double expected, string toUnit)
        {
            Assert.Equal(expected, new Acceleration(v, fromUnit).As(toUnit), 9);
        }

        [Theory]
        [InlineData(1.0, "lbf", 4.4482216152605, "N")]
        [InlineData(1.0, "kgf", 9.80665,         "N")]
        public void Force_Conversions(double v, string fromUnit, double expected, string toUnit)
        {
            Assert.Equal(expected, new Force(v, fromUnit).As(toUnit), 9);
        }

        [Theory]
        [InlineData(1.0, "psi", 6894.757293168, "Pa")]
        [InlineData(1.0, "bar", 100000.0,       "Pa")]
        [InlineData(1.0, "atm", 101325.0,       "Pa")]
        public void Pressure_Conversions(double v, string fromUnit, double expected, string toUnit)
        {
            Assert.Equal(expected, new Pressure(v, fromUnit).As(toUnit), 6);
        }

        [Theory]
        [InlineData(1.0, "kWh",  3.6e6,           "J")]
        [InlineData(1.0, "BTU",  1055.05585262,   "J")]
        [InlineData(1.0, "cal",  4.184,           "J")]
        public void Energy_Conversions(double v, string fromUnit, double expected, string toUnit)
        {
            Assert.Equal(expected, new Energy(v, fromUnit).As(toUnit), 6);
        }

        [Theory]
        [InlineData(1.0, "hp",     745.6998715822702, "W")]
        [InlineData(1.0, "BTU/hr", 0.29307107017,     "W")]
        public void Power_Conversions(double v, string fromUnit, double expected, string toUnit)
        {
            Assert.Equal(expected, new Power(v, fromUnit).As(toUnit), 6);
        }

        [Fact]
        public void Density_OfWater_AtSTPApprox1000kgPerCubicMeter()
        {
            // Pure water at 4°C is ~1000 kg/m³.
            var rho = new Density(1.0, "g/cm^3");
            Assert.Equal(1000.0, rho.As("kg/m^3"), 9);
        }

        [Fact]
        public void Frequency_AndAngularVelocity_HaveDistinctDimensions()
        {
            // Both numerically per-second but the angle pseudo-dimension distinguishes them
            var f  = new Frequency(60, "Hz");
            var av = new AngularVelocity(60, "rad/s");
            Assert.NotEqual(f.Dimension, av.Dimension);
        }

        [Fact]
        public void Frequency_RpmConverts_To_AngularVelocity_NotFrequency()
        {
            // 60 rpm = 1 rev/s = 2*pi rad/s
            var av = new AngularVelocity(60, "rpm");
            Assert.Equal(2 * System.Math.PI, av.As("rad/s"), 9);
        }

        [Fact]
        public void MassFlowRate_Conversions()
        {
            Assert.Equal(3600.0, new MassFlowRate(1, "kg/s").As("kg/h"), 9);
        }

        [Fact]
        public void VolumetricFlowRate_Conversions()
        {
            // 1 gpm in m^3/s
            var q = new VolumetricFlowRate(1, "gpm");
            Assert.Equal(6.30901964e-5, q.As("m^3/s"), 9);
        }
    }
}
