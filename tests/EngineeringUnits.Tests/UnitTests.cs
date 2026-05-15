using System;
using Xunit;

namespace EngineeringUnits.Tests
{
    public class UnitTests
    {
        [Fact]
        public void Constructor_AcceptsLinearUnit()
        {
            var ft = new Unit("ft", "foot", DimensionSignature.Length, 0.3048);
            Assert.Equal("ft", ft.Symbol);
            Assert.Equal("foot", ft.LongName);
            Assert.Equal(0.0, ft.Offset);
        }

        [Fact]
        public void Constructor_AcceptsAffineUnit()
        {
            var degC = new Unit("degC", "degree Celsius", DimensionSignature.Temperature, 1.0, 273.15);
            Assert.Equal(273.15, degC.Offset);
        }

        [Fact]
        public void Constructor_RejectsEmptySymbol()
        {
            Assert.Throws<ArgumentException>(() => new Unit("", "blank", DimensionSignature.Length, 1.0));
        }

        [Fact]
        public void Constructor_RejectsNullLongName()
        {
            Assert.Throws<ArgumentNullException>(() => new Unit("x", null!, DimensionSignature.Length, 1.0));
        }

        [Fact]
        public void Constructor_RejectsZeroScale()
        {
            Assert.Throws<ArgumentException>(() => new Unit("x", "x", DimensionSignature.Length, 0.0));
        }

        [Fact]
        public void Constructor_RejectsInfiniteScale()
        {
            Assert.Throws<ArgumentException>(() => new Unit("x", "x", DimensionSignature.Length, double.PositiveInfinity));
        }

        [Fact]
        public void ToCanonical_AppliesScaleAndOffset()
        {
            var degC = new Unit("degC", "degree Celsius", DimensionSignature.Temperature, 1.0, 273.15);
            Assert.Equal(298.15, degC.ToCanonical(25.0), 12);
        }

        [Fact]
        public void FromCanonical_InvertsToCanonical()
        {
            var degF = new Unit("degF", "degree Fahrenheit", DimensionSignature.Temperature, 5.0 / 9.0, 459.67 * 5.0 / 9.0);
            var roundtrip = degF.FromCanonical(degF.ToCanonical(77.0));
            Assert.Equal(77.0, roundtrip, 9);
        }

        [Fact]
        public void Equality_IsBySymbol()
        {
            var a = new Unit("ft", "foot", DimensionSignature.Length, 0.3048);
            var b = new Unit("ft", "FOOT (different long name)", DimensionSignature.Length, 999.0);
            Assert.Equal(a, b);
        }
    }
}
