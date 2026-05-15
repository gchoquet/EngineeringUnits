using System;
using Xunit;

namespace EngineeringUnits.Tests
{
    public class LengthTests
    {
        [Fact]
        public void Constructor_AcceptsSimpleUnit()
        {
            var l = new Length(5, "ft");
            Assert.Equal("ft", l.DisplayUnit.Symbol);
            Assert.Equal(5.0, l.Value, 12);
        }

        [Fact]
        public void Constructor_StoresCanonicalInMeters()
        {
            var l = new Length(5, "ft");
            Assert.Equal(1.524, l.CanonicalValue, 12);
        }

        [Fact]
        public void Constructor_RejectsUnknownUnit()
        {
            Assert.Throws<UnknownUnitException>(() => new Length(5, "blarg"));
        }

        [Fact]
        public void Constructor_RejectsWrongDimensionUnit()
        {
            Assert.Throws<DimensionMismatchException>(() => new Length(5, "kg"));
        }

        [Theory]
        [InlineData(5.0,    "ft", 1.524,    "m")]
        [InlineData(1.0,    "in", 0.0254,   "m")]
        [InlineData(1.0,    "mi", 1609.344, "m")]
        [InlineData(1.0,    "km", 3280.839895013, "ft")]
        [InlineData(2.54,   "cm", 1.0,      "in")]
        public void As_ConvertsToTargetUnit(double v, string fromUnit, double expected, string toUnit)
        {
            var l = new Length(v, fromUnit);
            Assert.Equal(expected, l.As(toUnit), 9);
        }

        [Fact]
        public void As_RejectsWrongDimensionTarget()
        {
            var l = new Length(5, "ft");
            Assert.Throws<DimensionMismatchException>(() => l.As("s"));
        }

        [Fact]
        public void In_PreservesCanonicalValue()
        {
            var ft = new Length(5, "ft");
            var inches = ft.In("in");
            Assert.Equal(60.0, inches.Value, 9);
            Assert.Equal(ft.CanonicalValue, inches.CanonicalValue, 12);
        }

        [Fact]
        public void Parse_AcceptsValueWithUnit()
        {
            var l = Length.Parse("5 ft");
            Assert.Equal(5.0, l.Value, 12);
            Assert.Equal("ft", l.DisplayUnit.Symbol);
        }

        [Fact]
        public void Parse_ThrowsOnMalformed()
        {
            Assert.Throws<UnitParseException>(() => Length.Parse("5ft"));   // no space
            Assert.Throws<UnitParseException>(() => Length.Parse("ft"));     // no value
            Assert.Throws<UnitParseException>(() => Length.Parse("5 xyzzy"));// unknown unit
            Assert.Throws<UnitParseException>(() => Length.Parse("5 kg"));   // wrong dimension
        }

        [Fact]
        public void TryParse_ReturnsFalseOnFailure()
        {
            Assert.False(Length.TryParse(null, out _));
            Assert.False(Length.TryParse("", out _));
            Assert.False(Length.TryParse("not a number ft", out _));
            Assert.False(Length.TryParse("5 nonexistentunit", out _));
        }

        [Fact]
        public void Addition_PreservesLeftOperandUnit()
        {
            var sum = new Length(5, "ft") + new Length(6, "in");
            Assert.Equal("ft", sum.DisplayUnit.Symbol);
            Assert.Equal(5.5, sum.Value, 9);
        }

        [Fact]
        public void Subtraction_PreservesLeftOperandUnit()
        {
            var diff = new Length(5, "ft") - new Length(0.5, "m");
            Assert.Equal("ft", diff.DisplayUnit.Symbol);
            Assert.Equal(5.0 - (0.5 / 0.3048), diff.Value, 9);
        }

        [Fact]
        public void UnaryNegation_NegatesValue()
        {
            var n = -new Length(5, "ft");
            Assert.Equal(-5.0, n.Value, 12);
        }

        [Fact]
        public void ScalarMultiplication_ScalesValue()
        {
            var doubled = new Length(5, "ft") * 2.0;
            Assert.Equal(10.0, doubled.Value, 12);
            Assert.Equal("ft", doubled.DisplayUnit.Symbol);
        }

        [Fact]
        public void ScalarMultiplication_Commutes()
        {
            var a = new Length(5, "ft") * 2.0;
            var b = 2.0 * new Length(5, "ft");
            Assert.Equal(a.Value, b.Value, 12);
        }

        [Fact]
        public void ScalarDivision_ScalesValue()
        {
            var half = new Length(5, "ft") / 2.0;
            Assert.Equal(2.5, half.Value, 12);
        }

        [Theory]
        [InlineData(1.0, "m", 1.0, "m", true)]
        [InlineData(1.0, "m", 100.0, "cm", true)]
        [InlineData(1.0, "m", 1.001, "m", false)]
        public void Equality_IsByCanonicalValue(double v1, string u1, double v2, string u2, bool equal)
        {
            var a = new Length(v1, u1);
            var b = new Length(v2, u2);
            Assert.Equal(equal, a.Equals(b));
            Assert.Equal(equal, a == b);
            Assert.Equal(!equal, a != b);
        }

        [Fact]
        public void Comparison_OrdersByCanonicalValue()
        {
            var foot = new Length(1, "ft");
            var meter = new Length(1, "m");
            Assert.True(foot < meter);
            Assert.True(meter > foot);
            Assert.True(foot <= meter);
            Assert.True(meter >= foot);
        }

        [Fact]
        public void ToString_DefaultUsesDisplayUnit()
        {
            var l = new Length(5, "ft") { Precision = 4 };
            Assert.Equal("5.000 ft", l.ToString());
        }

        [Fact]
        public void ToString_HandlesZero()
        {
            var l = new Length(0, "ft");
            Assert.Contains("0", l.ToString());
            Assert.Contains("ft", l.ToString());
        }

        [Fact]
        public void Precision_DefaultIs4()
        {
            var l = new Length(5, "ft");
            Assert.Equal(4, l.Precision);
        }

        [Fact]
        public void Arithmetic_TakesMinimumPrecision()
        {
            var a = new Length(5, "ft") { Precision = 3 };
            var b = new Length(6, "in") { Precision = 5 };
            var sum = a + b;
            Assert.Equal(3, sum.Precision);
        }
    }
}
