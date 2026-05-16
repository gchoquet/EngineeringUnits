using Xunit;

namespace EngineeringUnits.Tests
{
    /// <summary>
    /// Verifies that the UnitCatalog accepts long names and common plural aliases,
    /// not just short symbols. Symbol lookups remain case-sensitive (so 'm' and 'M'
    /// stay distinct). Long-name and alias lookups are case-insensitive.
    /// </summary>
    public class LongNameLookupTests
    {
        // ── Long-name lookups ─────────────────────────────────────

        [Theory]
        [InlineData("foot",     "ft")]
        [InlineData("meter",    "m")]
        [InlineData("inch",     "in")]
        [InlineData("kilogram", "kg")]
        [InlineData("pound",    "lb")]
        [InlineData("second",   "s")]
        [InlineData("minute",   "min")]
        [InlineData("pascal",   "Pa")]
        [InlineData("joule",    "J")]
        [InlineData("watt",     "W")]
        public void TryGet_AcceptsLongName(string longName, string expectedSymbol)
        {
            Assert.True(UnitCatalog.TryGet(longName, out var u));
            Assert.Equal(expectedSymbol, u.Symbol);
        }

        [Theory]
        [InlineData("Foot",     "ft")]   // capitalized
        [InlineData("METER",    "m")]    // all caps
        [InlineData("KiLoGrAm", "kg")]   // mixed
        public void TryGet_LongName_IsCaseInsensitive(string longName, string expectedSymbol)
        {
            Assert.True(UnitCatalog.TryGet(longName, out var u));
            Assert.Equal(expectedSymbol, u.Symbol);
        }

        // ── Plural / alias lookups ────────────────────────────────

        [Theory]
        [InlineData("feet",       "ft")]
        [InlineData("inches",     "in")]
        [InlineData("yards",      "yd")]
        [InlineData("miles",      "mi")]
        [InlineData("meters",     "m")]
        [InlineData("metres",     "m")]    // British spelling
        [InlineData("kilograms",  "kg")]
        [InlineData("pounds",     "lb")]
        [InlineData("ounces",     "oz")]
        [InlineData("seconds",    "s")]
        [InlineData("hours",      "h")]
        [InlineData("days",       "day")]
        [InlineData("celsius",    "degC")]
        [InlineData("fahrenheit", "degF")]
        [InlineData("kelvins",    "K")]
        [InlineData("gallons",    "gal")]
        [InlineData("liters",     "L")]
        [InlineData("barrels",    "bbl")]
        [InlineData("newtons",    "N")]
        [InlineData("pascals",    "Pa")]
        [InlineData("joules",     "J")]
        [InlineData("watts",      "W")]
        [InlineData("volts",      "V")]
        [InlineData("amperes",    "A")]
        [InlineData("amps",       "A")]
        [InlineData("ohms",       "Ω")]
        public void TryGet_AcceptsCommonPluralOrAlias(string alias, string expectedSymbol)
        {
            Assert.True(UnitCatalog.TryGet(alias, out var u));
            Assert.Equal(expectedSymbol, u.Symbol);
        }

        [Theory]
        [InlineData("Feet")]
        [InlineData("FEET")]
        [InlineData("FeEt")]
        public void TryGet_Alias_IsCaseInsensitive(string alias)
        {
            Assert.True(UnitCatalog.TryGet(alias, out var u));
            Assert.Equal("ft", u.Symbol);
        }

        // ── Invalid units still error ─────────────────────────────

        [Theory]
        [InlineData("foots")]          // bad pluralization of foot
        [InlineData("metr")]           // typo
        [InlineData("kilogrm")]        // typo
        [InlineData("xyzzy")]
        [InlineData("")]
        [InlineData("definitely-not-a-unit")]
        public void TryGet_InvalidName_ReturnsFalse(string bogus)
        {
            Assert.False(UnitCatalog.TryGet(bogus, out _));
        }

        [Fact]
        public void TryGet_Null_ReturnsFalse()
        {
            Assert.False(UnitCatalog.TryGet(null!, out _));
        }

        // ── Symbol lookups stay case-sensitive ────────────────────

        [Fact]
        public void TryGet_Symbol_M_vs_m_StaysCaseSensitive()
        {
            // 'm' is meter (scale 1.0); 'Mm' is megameter (scale 1e6).
            // We should NOT collapse capitalization on symbols.
            Assert.True(UnitCatalog.TryGet("m", out var meter));
            Assert.True(UnitCatalog.TryGet("Mm", out var megameter));
            Assert.Equal(1.0,  meter.Scale);
            Assert.Equal(1e6,  megameter.Scale);
        }

        // ── End-to-end through Length constructor ─────────────────

        [Fact]
        public void Length_AcceptsLongNameInput()
        {
            var l = new Length(5.0, "foot");
            Assert.Equal(1.524, l.CanonicalValue, 9);
        }

        [Fact]
        public void Length_AcceptsPluralInput()
        {
            var l = new Length(5.0, "feet");
            Assert.Equal(1.524, l.CanonicalValue, 9);
        }

        [Fact]
        public void Length_RejectsTypo()
        {
            Assert.Throws<UnknownUnitException>(() => new Length(5.0, "foots"));
        }
    }
}
