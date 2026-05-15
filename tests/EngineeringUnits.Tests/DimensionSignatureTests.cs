using Xunit;

namespace EngineeringUnits.Tests
{
    public class DimensionSignatureTests
    {
        [Fact]
        public void Default_IsDimensionless()
        {
            var d = default(DimensionSignature);
            Assert.True(d.IsDimensionless);
        }

        [Fact]
        public void Length_HasLengthExponentOne()
        {
            var d = DimensionSignature.Length;
            Assert.Equal(1, d.L);
            Assert.Equal(0, d.M);
            Assert.False(d.IsDimensionless);
        }

        [Fact]
        public void Addition_ComposesElementWise()
        {
            // Length * Length = Area  ->  L^2
            var area = DimensionSignature.Length + DimensionSignature.Length;
            Assert.Equal(2, area.L);
        }

        [Fact]
        public void Subtraction_ComposesElementWise()
        {
            // Length / Time = Velocity -> L^1 T^-1
            var velocity = DimensionSignature.Length - DimensionSignature.Time;
            Assert.Equal(1, velocity.L);
            Assert.Equal(-1, velocity.T);
        }

        [Fact]
        public void IntMultiplication_RaisesToPower()
        {
            var cube = DimensionSignature.Length * 3;
            Assert.Equal(3, cube.L);
        }

        [Fact]
        public void Equality_IsValueBased()
        {
            var a = new DimensionSignature(L: 1, M: 1);
            var b = new DimensionSignature(L: 1, M: 1);
            var c = new DimensionSignature(L: 1, M: 2);
            Assert.Equal(a, b);
            Assert.True(a == b);
            Assert.NotEqual(a, c);
            Assert.True(a != c);
        }

        [Fact]
        public void HashCode_StableForEqualValues()
        {
            var a = new DimensionSignature(L: 1, T: -1);
            var b = new DimensionSignature(L: 1, T: -1);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void ToString_DimensionlessIs1()
        {
            Assert.Equal("1", DimensionSignature.Dimensionless.ToString());
        }

        [Fact]
        public void ToString_LengthIsL()
        {
            Assert.Equal("L", DimensionSignature.Length.ToString());
        }

        [Fact]
        public void ToString_FormatsCompoundDimensions()
        {
            // Force: M * L / T^2
            var force = DimensionSignature.Mass + DimensionSignature.Length - DimensionSignature.Time * 2;
            Assert.Equal("L*M/T^2", force.ToString());
        }
    }
}
