using System;
using Xunit;

namespace EngineeringUnits.Tests
{
    public class TemperatureTests
    {
        [Theory]
        [InlineData(0.0,    "degC",  273.15, "K")]
        [InlineData(100.0,  "degC",  373.15, "K")]
        [InlineData(32.0,   "degF",  273.15, "K")]
        [InlineData(212.0,  "degF",  373.15, "K")]
        [InlineData(0.0,    "degF",  255.3722222222, "K")]
        [InlineData(32.0,   "degF",  0.0,    "degC")]
        [InlineData(212.0,  "degF",  100.0,  "degC")]
        [InlineData(491.67, "degR",  273.15, "K")]
        public void As_HandlesAffineConversion(double v, string fromUnit, double expected, string toUnit)
        {
            var t = new Temperature(v, fromUnit);
            Assert.Equal(expected, t.As(toUnit), 6);
        }

        [Fact]
        public void Constructor_AcceptsAllScales()
        {
            // Just make sure each constructs without throwing
            _ = new Temperature(25, "K");
            _ = new Temperature(25, "degC");
            _ = new Temperature(25, "°C");
            _ = new Temperature(77, "degF");
            _ = new Temperature(77, "°F");
            _ = new Temperature(491.67, "degR");
            _ = new Temperature(491.67, "°R");
        }

        [Fact]
        public void Constructor_RejectsWrongDimension()
        {
            Assert.Throws<DimensionMismatchException>(() => new Temperature(25, "ft"));
        }

        [Fact]
        public void Parse_AcceptsValueWithUnit()
        {
            var t = Temperature.Parse("25 degC");
            Assert.Equal(25.0, t.Value, 9);
            Assert.Equal("degC", t.DisplayUnit.Symbol);
        }

        [Fact]
        public void Addition_ThrowsAsPhysicallyMeaningless()
        {
            var a = new Temperature(20, "degC");
            var b = new Temperature(30, "degC");
            Assert.Throws<InvalidOperationException>(() => { var _ = a + b; });
        }

        [Fact]
        public void Subtraction_ReturnsTemperatureDelta_InLeftOperandUnit()
        {
            // 30°C - 20°C = 10°C delta (display in left operand's unit, no offset)
            var diff = new Temperature(30, "degC") - new Temperature(20, "degC");
            Assert.IsType<TemperatureDelta>(diff);
            Assert.Equal("degC", diff.DisplayUnit.Symbol);
            Assert.Equal(10.0, diff.Value, 9);
        }

        [Fact]
        public void Subtraction_AcrossUnitsYieldsCorrectDelta()
        {
            // 100°F - 32°F = 68°F delta (linear; no offset applied)
            var diff = new Temperature(100, "degF") - new Temperature(32, "degF");
            Assert.IsType<TemperatureDelta>(diff);
            Assert.Equal("degF", diff.DisplayUnit.Symbol);
            Assert.Equal(68.0, diff.Value, 9);
            // Verify the delta is also correct in K
            Assert.Equal(68.0 * 5.0 / 9.0, diff.As("K"), 9);
        }

        [Fact]
        public void Equality_IsByCanonicalValue()
        {
            var c = new Temperature(0, "degC");
            var k = new Temperature(273.15, "K");
            Assert.Equal(c, k);
        }
    }
}
