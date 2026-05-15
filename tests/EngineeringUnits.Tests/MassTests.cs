using Xunit;

namespace EngineeringUnits.Tests
{
    public class MassTests
    {
        [Theory]
        [InlineData(1.0,    "lb",  0.45359237, "kg")]
        [InlineData(1.0,    "kg",  2.2046226218, "lb")]
        [InlineData(1.0,    "oz",  0.028349523125, "kg")]
        [InlineData(1.0,    "t",   1000.0,    "kg")]
        [InlineData(1.0,    "ton", 907.18474, "kg")]
        public void As_ConvertsCorrectly(double v, string fromUnit, double expected, string toUnit)
        {
            var m = new Mass(v, fromUnit);
            Assert.Equal(expected, m.As(toUnit), 9);
        }

        [Fact]
        public void Constructor_RejectsWrongDimension()
        {
            Assert.Throws<DimensionMismatchException>(() => new Mass(5, "ft"));
        }

        [Fact]
        public void Addition_PreservesLeftOperandUnit()
        {
            var sum = new Mass(5, "kg") + new Mass(500, "g");
            Assert.Equal("kg", sum.DisplayUnit.Symbol);
            Assert.Equal(5.5, sum.Value, 9);
        }

        [Fact]
        public void Parse_AcceptsValueWithUnit()
        {
            var m = Mass.Parse("2.5 lb");
            Assert.Equal(2.5, m.Value, 9);
            Assert.Equal("lb", m.DisplayUnit.Symbol);
        }

        [Fact]
        public void Equality_IsByCanonicalValue()
        {
            var kg = new Mass(1, "kg");
            var g = new Mass(1000, "g");
            Assert.Equal(kg, g);
        }
    }
}
