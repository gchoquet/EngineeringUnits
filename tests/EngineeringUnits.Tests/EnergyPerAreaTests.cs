using Xunit;

namespace EngineeringUnits.Tests
{
    /// <summary>
    /// EnergyPerArea (e.g. fracture toughness E/A, surface energy). Dimensionally
    /// identical to SurfaceTension (M/T²) — these tests cover the conversion
    /// factors for the units fracture-mechanics workflows actually use.
    /// </summary>
    public class EnergyPerAreaTests
    {
        [Theory]
        [InlineData("J/m^2",      1.0)]
        [InlineData("kJ/m^2",     1000.0)]
        [InlineData("MJ/m^2",     1e6)]
        [InlineData("J/cm^2",     1e4)]
        [InlineData("N/mm",       1000.0)]
        public void Construct_FromSiUnits_RoundtripsToCanonical(string unit, double expectedCanonical)
        {
            var ea = new EnergyPerArea(1.0, unit);
            Assert.Equal(expectedCanonical, ea.CanonicalValue, 9);
        }

        [Fact]
        public void FtLbfPerIn2_ConvertsTo_kJPerM2()
        {
            // 1 ft·lbf = 1.355817948331 J; 1 in² = 0.00064516 m²
            // → 1 ft·lbf/in² = 1.355817948331/0.00064516 = 2101.522 J/m² = 2.10152 kJ/m².
            var ea = new EnergyPerArea(1.0, "ft*lbf/in^2").In("kJ/m^2");
            Assert.Equal(2.101522, ea.Value, 5);
        }

        [Fact]
        public void KJPerM2_ConvertsTo_FtLbfPerIn2()
        {
            var ea = new EnergyPerArea(1.0, "kJ/m^2").In("ft*lbf/in^2");
            Assert.Equal(0.475846, ea.Value, 5);
        }

        [Fact]
        public void Dimension_MatchesSurfaceTension()
        {
            // Same M/T² as SurfaceTension; separate class for semantic clarity (cf. Torque/Energy).
            Assert.Equal(new EnergyPerArea(1, "J/m^2").Dimension, new SurfaceTension(1, "N/m").Dimension);
        }

        [Fact]
        public void Ascii_And_UnicodeSuperscript_Aliases_Agree()
        {
            var a = new EnergyPerArea(500, "ft*lbf/in^2").CanonicalValue;
            var b = new EnergyPerArea(500, "ft·lbf/in²").CanonicalValue;
            Assert.Equal(a, b, 9);
        }
    }
}
