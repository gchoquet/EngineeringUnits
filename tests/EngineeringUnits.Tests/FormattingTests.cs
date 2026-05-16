using System.Globalization;
using Xunit;

namespace EngineeringUnits.Tests
{
    public class FormattingTests
    {
        [Fact]
        public void ToString_Default_UsesShortSymbol()
        {
            Assert.Equal("5.000 ft", new Length(5, "ft").ToString());
        }

        [Fact]
        public void Format_L_UsesLongName()
        {
            Assert.Equal("5.000 foot", new Length(5, "ft").ToString("L"));
        }

        [Fact]
        public void Format_S_ConvertsToSI()
        {
            // 5 ft in SI scientific (meters)
            var l = new Length(5, "ft");
            Assert.Contains("m", l.ToString("S"));
            Assert.DoesNotContain("ft", l.ToString("S"));
        }

        [Fact]
        public void Format_U_ConvertsToUsCustomary()
        {
            // 1 m in US customary (feet)
            var l = new Length(1, "m");
            Assert.Contains("ft", l.ToString("U"));
            Assert.DoesNotContain(" m", l.ToString("U"));
        }

        [Fact]
        public void Format_P_UsesDefaultPreferences()
        {
            // Default is SIScientific
            UnitPreferences.Default = UnitPreferences.SIScientific;
            var l = new Length(5, "ft");
            Assert.Contains("m", l.ToString("P"));
        }

        [Fact]
        public void Format_P_RespectsCustomPreferences()
        {
            UnitPreferences.Default = UnitPreferences.UsCustomary;
            var l = new Length(1, "m");
            Assert.Contains("ft", l.ToString("P"));
            UnitPreferences.Default = UnitPreferences.SIScientific;  // reset
        }

        [Fact]
        public void Format_D_DualStringShowsBothSystems()
        {
            var l = new Length(1, "m");
            var s = l.ToString("D");
            Assert.Contains("m", s);
            Assert.Contains("ft", s);
            Assert.Contains("(", s);
            Assert.Contains(")", s);
        }

        [Fact]
        public void Format_E_UsesScientificNotation()
        {
            var l = new Length(123, "m");
            var s = l.ToString("E");
            Assert.Contains("E+", s);
            Assert.Contains("m", s);
        }

        [Theory]
        [InlineData(2, "5.0 ft")]
        [InlineData(3, "5.00 ft")]
        [InlineData(6, "5.00000 ft")]
        public void Format_N_ForcesSignificantFigures(int sigFigs, string expected)
        {
            var l = new Length(5, "ft");
            Assert.Equal(expected, l.ToString($"N{sigFigs}"));
        }

        [Fact]
        public void Format_BraceUnit_ForcesSpecificUnit()
        {
            var l = new Length(5, "ft");
            var inMeters = l.ToString("{m}");
            Assert.Contains("m", inMeters);
            Assert.DoesNotContain("ft", inMeters);
        }

        [Fact]
        public void Format_BraceUnit_IgnoresWrongDimension()
        {
            var l = new Length(5, "ft");
            // Wrong-dimension unit should fall back to default behavior
            var s = l.ToString("{kg}");
            Assert.Contains("ft", s);
        }

        [Fact]
        public void Culture_UsesProvidedDecimalSeparator()
        {
            var l = new Length(1.5, "m");
            var deDE = CultureInfo.GetCultureInfo("de-DE");
            var formatted = l.ToString(null, deDE);
            Assert.Contains(",", formatted);
            Assert.DoesNotContain("1.5", formatted);
        }

        [Fact]
        public void ToDualString_LengthShowsBothUnits()
        {
            var dia = new Length(0.5, "m");
            var s = dia.ToDualString();
            // Should contain both m and ft
            Assert.Contains("m", s);
            Assert.Contains("ft", s);
        }
    }

    public class UnitPreferencesTests
    {
        [Fact]
        public void Prefer_SetsByDimension()
        {
            var p = new UnitPreferences();
            p.Prefer("km");
            var pref = p.GetPreferred(DimensionSignature.Length);
            Assert.True(pref.HasValue);
            Assert.Equal("km", pref!.Value.Symbol);
        }

        [Fact]
        public void Clone_IsIndependent()
        {
            var src = UnitPreferences.SIScientific.Clone();
            src.Prefer("km");
            var copy = src.Clone();
            copy.Prefer("ft");
            Assert.Equal("km", src.GetPreferred(DimensionSignature.Length)!.Value.Symbol);
            Assert.Equal("ft", copy.GetPreferred(DimensionSignature.Length)!.Value.Symbol);
        }

        [Fact]
        public void SIScientific_Profile_UsesUnicodeNotation()
        {
            Assert.Equal(NotationStyle.Unicode, UnitPreferences.SIScientific.Notation);
        }

        [Fact]
        public void UsCustomary_Profile_UsesCaretNotation()
        {
            Assert.Equal(NotationStyle.Caret, UnitPreferences.UsCustomary.Notation);
        }

        [Fact]
        public void OilAndGas_Profile_PrefersBarrelForVolume()
        {
            var pref = UnitPreferences.OilAndGas.GetPreferred(Volume.Dim);
            Assert.True(pref.HasValue);
            Assert.Equal("bbl", pref!.Value.Symbol);
        }

        [Fact]
        public void Machining_Profile_PrefersMillimeter()
        {
            var pref = UnitPreferences.Machining.GetPreferred(DimensionSignature.Length);
            Assert.True(pref.HasValue);
            Assert.Equal("mm", pref!.Value.Symbol);
        }
    }
}
