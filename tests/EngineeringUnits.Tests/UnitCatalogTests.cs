using System.Linq;
using Xunit;

namespace EngineeringUnits.Tests
{
    public class UnitCatalogTests
    {
        [Theory]
        [InlineData("m")]    [InlineData("ft")]   [InlineData("kg")]
        [InlineData("lb")]   [InlineData("s")]    [InlineData("K")]
        [InlineData("°C")]   [InlineData("°F")]   [InlineData("MMSCFD-not-seeded-yet")]
        public void TryGet_HandlesKnownAndUnknown(string symbol)
        {
            var ok = UnitCatalog.TryGet(symbol, out var u);
            if (symbol.StartsWith("MMSCFD"))
                Assert.False(ok);
            else
            {
                Assert.True(ok);
                Assert.Equal(symbol, u.Symbol);
            }
        }

        [Fact]
        public void Get_ThrowsOnUnknownSymbol()
        {
            var ex = Assert.Throws<UnknownUnitException>(() => UnitCatalog.Get("definitely-not-a-unit"));
            Assert.Equal("definitely-not-a-unit", ex.Symbol);
        }

        [Fact]
        public void IsRegistered_ReportsKnown()
        {
            Assert.True(UnitCatalog.IsRegistered("ft"));
            Assert.False(UnitCatalog.IsRegistered("xyzzy"));
        }

        [Fact]
        public void All_EnumeratesAllRegisteredUnits()
        {
            var units = UnitCatalog.All.ToList();
            Assert.NotEmpty(units);
            Assert.Contains(units, u => u.Symbol == "m");
            Assert.Contains(units, u => u.Symbol == "kg");
        }

        [Fact]
        public void CaseSensitive_M_IsDifferentFrom_m()
        {
            // M = megameter (length), m = meter (length).  Both should resolve.
            Assert.True(UnitCatalog.TryGet("m", out var m));
            Assert.True(UnitCatalog.TryGet("Mm", out var Mm));
            Assert.Equal(1.0, m.Scale);
            Assert.Equal(1e6, Mm.Scale);
        }
    }
}
