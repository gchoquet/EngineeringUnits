using Xunit;

namespace EngineeringUnits.Tests
{
    /// <summary>
    /// Confirms the design choice locked in Decision 14.3 / §15 Open Q3:
    /// the constructor takes <c>double</c>, and C# implicit numeric conversion
    /// makes <c>int</c>, <c>float</c>, <c>long</c>, etc. accepted at the call
    /// site without explicit casts. ExcelDNA passes cell values as <c>double</c>
    /// already; this test exists to detect any future regression that would
    /// change that boundary.
    /// </summary>
    public class NumericConversionTests
    {
        [Fact]
        public void Constructor_AcceptsInt()
        {
            var l = new Length(5, "ft");                    // int literal
            Assert.Equal(5.0, l.Value, 12);
            Assert.Equal(1.524, l.CanonicalValue, 12);
        }

        [Fact]
        public void Constructor_AcceptsLong()
        {
            var l = new Length(5L, "ft");                   // long literal
            Assert.Equal(5.0, l.Value, 12);
        }

        [Fact]
        public void Constructor_AcceptsFloat()
        {
            var l = new Length(5.0f, "ft");                 // float literal
            Assert.Equal(5.0, l.Value, 6);
        }

        [Fact]
        public void Constructor_AcceptsDecimalAfterCast()
        {
            // decimal does NOT implicitly convert to double in C#. Verify the
            // explicit cast path works, so callers with decimal inputs (rare
            // in engineering — typically financial code) can still use the library.
            decimal d = 5.0m;
            var l = new Length((double)d, "ft");
            Assert.Equal(5.0, l.Value, 12);
        }

        [Fact]
        public void Arithmetic_AcceptsScalarInt()
        {
            var l = new Length(5, "ft");
            var doubled = l * 2;                            // int literal as scalar
            Assert.Equal(10.0, doubled.Value, 12);
        }

        [Fact]
        public void Arithmetic_AcceptsScalarFloat()
        {
            var l = new Length(5, "ft");
            var halved = l * 0.5f;                          // float literal
            Assert.Equal(2.5, halved.Value, 6);
        }

        [Fact]
        public void DivisionByInt_ScalesCorrectly()
        {
            var l = new Length(10, "ft");
            var third = l / 3;                              // int literal in division
            Assert.Equal(10.0 / 3.0, third.Value, 12);
        }
    }
}
