using Xunit;

namespace EngineeringUnits.Tests
{
    public class TimeTests
    {
        [Theory]
        [InlineData(1.0, "min", 60.0, "s")]
        [InlineData(1.0, "h",   3600.0, "s")]
        [InlineData(1.0, "day", 86400.0, "s")]
        [InlineData(60.0, "s",  1.0, "min")]
        public void As_ConvertsCorrectly(double v, string fromUnit, double expected, string toUnit)
        {
            var t = new Time(v, fromUnit);
            Assert.Equal(expected, t.As(toUnit), 9);
        }

        [Fact]
        public void Constructor_RejectsWrongDimension()
        {
            Assert.Throws<DimensionMismatchException>(() => new Time(5, "kg"));
        }

        [Fact]
        public void Addition_PreservesLeftOperandUnit()
        {
            var sum = new Time(1, "min") + new Time(30, "s");
            Assert.Equal("min", sum.DisplayUnit.Symbol);
            Assert.Equal(1.5, sum.Value, 9);
        }

        [Fact]
        public void Parse_AcceptsValueWithUnit()
        {
            var t = Time.Parse("2.5 h");
            Assert.Equal(2.5, t.Value, 9);
        }
    }
}
