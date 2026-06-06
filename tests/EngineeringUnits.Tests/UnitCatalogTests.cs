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

        // ── Compositional units (composed from base units on demand, not seeded) ──

        [Fact]
        public void Compose_DensityFromMassAndVolume_NotSeeded()
        {
            // slug/in^3 is not seeded; it must compose from slug (mass) and in (length, cubed).
            Assert.True(UnitCatalog.TryGet("slug/in^3", out var u));
            Assert.Equal(14.59390294 / (0.0254 * 0.0254 * 0.0254), u.ToCanonical(1.0), 3);
        }

        [Fact]
        public void Compose_GramsPerLitre_EqualsKgPerM3()
        {
            // g/L is not seeded; composes to exactly 1 kg/m^3.
            Assert.True(UnitCatalog.TryGet("g/L", out var u));
            Assert.Equal(1.0, u.ToCanonical(1.0), 9);
        }

        [Fact]
        public void Compose_HandlesUnicodeSuperscriptExponents()
        {
            Assert.True(UnitCatalog.TryGet("m/s^2", out var a));
            Assert.True(UnitCatalog.TryGet("m/s²", out var b));
            Assert.Equal(a.ToCanonical(1.0), b.ToCanonical(1.0), 12);
        }

        [Fact]
        public void Compose_ProductAndQuotient()
        {
            // 1 lbf*s/ft^2 = 47.880259 Pa*s (dynamic viscosity).
            Assert.True(UnitCatalog.TryGet("lbf*s/ft^2", out var u));
            Assert.Equal(47.880259, u.ToCanonical(1.0), 4);
        }

        [Fact]
        public void Compose_RejectsAffineComponent()
        {
            // °C is affine (non-zero offset); it cannot form a compound unit.
            Assert.False(UnitCatalog.TryGet("°C/m", out _));
        }

        [Fact]
        public void Compose_RejectsUnknownToken()
        {
            Assert.False(UnitCatalog.TryGet("zorch/ft^3", out _));
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
