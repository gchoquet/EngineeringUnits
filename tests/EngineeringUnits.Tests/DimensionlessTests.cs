using Xunit;

namespace EngineeringUnits.Tests
{
    public class DimensionlessTests
    {
        [Fact]
        public void Constructor_AcceptsRawDouble()
        {
            var d = new DimensionlessQuantity(2.0);
            Assert.Equal(2.0, d.Value, 12);
            Assert.True(d.Dimension.IsDimensionless);
        }

        [Fact]
        public void VelocityRatio_IsDimensionless()
        {
            var v1 = new Velocity(340, "m/s");
            var v2 = new Velocity(680, "m/s");
            var ratio = v2 / v1;
            Assert.IsType<DimensionlessQuantity>(ratio);
            Assert.Equal(2.0, ratio.Value, 9);
        }

        [Fact]
        public void LengthRatio_IsDimensionless()
        {
            var deflection = new Length(0.005, "m");
            var beamLength = new Length(2.0, "m");
            var strain = deflection / beamLength;
            Assert.IsType<DimensionlessQuantity>(strain);
            Assert.Equal(0.0025, strain.Value, 9);
        }

        [Fact]
        public void NoInterpretation_ShowsBareNumber()
        {
            var d = new DimensionlessQuantity(2.5);
            Assert.Equal("2.500", d.ToString());
            Assert.DoesNotContain("Ma", d.ToString());
        }

        [Fact]
        public void WithMachInterpretation_ShowsMaSymbol()
        {
            var d = new DimensionlessQuantity(2.0) { PreferredInterpretation = "Mach" };
            Assert.Contains("Ma", d.ToString());
        }

        [Fact]
        public void WithReynoldsInterpretation_ShowsReSymbol()
        {
            var d = new DimensionlessQuantity(4000) { PreferredInterpretation = "Reynolds" };
            Assert.Contains("Re", d.ToString());
        }

        [Theory]
        [InlineData("Reynolds", "Re")]
        [InlineData("Mach",     "Ma")]
        [InlineData("Froude",   "Fr")]
        [InlineData("Prandtl",  "Pr")]
        [InlineData("Nusselt",  "Nu")]
        [InlineData("Biot",     "Bi")]
        [InlineData("Euler",    "Eu")]
        [InlineData("Eckert",   "Ec")]
        [InlineData("Jakob",    "Ja")]
        public void KnownInterpretations_ResolveToExpectedSymbols(string name, string expectedSymbol)
        {
            Assert.True(DimensionlessQuantity.KnownInterpretations.ContainsKey(name));
            Assert.Equal(expectedSymbol, DimensionlessQuantity.KnownInterpretations[name]);
        }

        [Fact]
        public void UnknownInterpretation_FallsBackToBareNumber()
        {
            var d = new DimensionlessQuantity(2.0) { PreferredInterpretation = "NotAKnownNumber" };
            // Doesn't render the unknown name; just shows the number
            Assert.Equal("2.000", d.ToString());
        }

        [Fact]
        public void NoAutoInference()
        {
            // Library never auto-tags a velocity ratio as Mach
            var v1 = new Velocity(340, "m/s");
            var v2 = new Velocity(680, "m/s");
            var ratio = v2 / v1;
            Assert.Null(ratio.PreferredInterpretation);
        }

        [Fact]
        public void Parse_AcceptsBareNumber()
        {
            var d = DimensionlessQuantity.Parse("2.5");
            Assert.Equal(2.5, d.Value, 12);
        }

        [Fact]
        public void Parse_AcceptsNumberWithUnitOne()
        {
            var d = DimensionlessQuantity.Parse("2.5 1");
            Assert.Equal(2.5, d.Value, 12);
        }
    }

    public class IndustryUnitTests
    {
        [Fact]
        public void Dalton_IsCorrectMass()
        {
            // 1 Da ≈ 1.66053906660e-27 kg (CODATA 2018)
            var m = new Mass(1, "Da");
            Assert.Equal(1.66053906660e-27, m.CanonicalValue, 15);
        }

        [Fact]
        public void MMSCFD_DefaultIsInterstatePipelineConditions()
        {
            // 1 MMSCFD = 1e6 ft^3 / day in volumetric terms
            var q = new VolumetricFlowRate(1, "MMSCFD");
            var expected = 1e6 * (0.3048 * 0.3048 * 0.3048) / 86400.0;
            Assert.Equal(expected, q.CanonicalValue, 9);
        }

        [Fact]
        public void MMSCFD_AllVariants_HaveSameVolumetricScale()
        {
            // Volumetric conversion is condition-agnostic; the variants differ only
            // in the conditions they CARRY (for downstream mass-flow conversion).
            var a = new VolumetricFlowRate(1, "MMSCFD").CanonicalValue;
            var b = new VolumetricFlowRate(1, "MMSCFD_petro").CanonicalValue;
            var c = new VolumetricFlowRate(1, "MMSCFD_iupac").CanonicalValue;
            Assert.Equal(a, b, 12);
            Assert.Equal(a, c, 12);
        }

        [Fact]
        public void MMSCFD_PlusMMSCFD_PetroIsAllowedAndUsesLeftOperandUnit()
        {
            // Mixing variants does not throw; result is in left operand's unit
            var a = new VolumetricFlowRate(5, "MMSCFD");
            var b = new VolumetricFlowRate(2, "MMSCFD_petro");
            var sum = a + b;
            Assert.Equal("MMSCFD", sum.DisplayUnit.Symbol);
            Assert.Equal(7.0, sum.Value, 9);
        }

        [Fact]
        public void SCFM_KnownConversion()
        {
            // 1 SCFM (standard cubic foot per minute) = 1 ft^3 / 60 s in volumetric terms
            var q = new VolumetricFlowRate(1, "SCFM");
            var expected = (0.3048 * 0.3048 * 0.3048) / 60.0;
            Assert.Equal(expected, q.CanonicalValue, 9);
        }
    }
}
