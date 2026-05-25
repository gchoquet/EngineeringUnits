using System;
using EngineeringUnits;
using Xunit;

namespace EngineeringUnits.Tests
{
    /// <summary>
    /// Tests for the molar-basis quantity classes added in Phase 4 (AGA8
    /// integration): AmountOfSubstance, MolarMass, MolarEnergy,
    /// MolarHeatCapacity, MolarDensity, plus the MixtureMolarConversions
    /// bridge that needs mixture MW.
    /// </summary>
    public class MolarQuantityTests
    {
        // ───── AmountOfSubstance ────────────────────────────────────

        [Theory]
        [InlineData( "mol",  1.0,        1.0)]
        [InlineData("kmol",  1.0,     1000.0)]
        [InlineData("lbmol", 1.0,  453.59237)]
        public void AmountOfSubstance_CanonicalConversion(string sym, double v, double expectedMol)
        {
            var n = new AmountOfSubstance(v, sym);
            Assert.Equal(expectedMol, n.CanonicalValue, 9);
            Assert.Equal(v, n.In(sym).Value, 9);
        }

        [Fact]
        public void AmountOfSubstance_CrossUnitConversion()
        {
            // 1 lbmol → 453.59237 mol → 0.45359237 kmol
            var n = new AmountOfSubstance(1.0, "lbmol");
            Assert.Equal(453.59237,  n.In("mol").Value,  6);
            Assert.Equal(0.45359237, n.In("kmol").Value, 9);
        }

        // ───── MolarMass ────────────────────────────────────────────

        [Fact]
        public void MolarMass_GramPerMole_EqualsKgPerKmol_EqualsLbPerLbmol()
        {
            // The "molecular weight" value is unit-independent across these three forms.
            var mwG  = new MolarMass(16.043, "g/mol");
            var mwKg = new MolarMass(16.043, "kg/kmol");
            var mwLb = new MolarMass(16.043, "lb/lbmol");
            Assert.Equal(mwG.CanonicalValue, mwKg.CanonicalValue, 12);
            Assert.Equal(mwG.CanonicalValue, mwLb.CanonicalValue, 12);
            // Canonical is kg/mol so 16.043 g/mol = 0.016043 kg/mol
            Assert.Equal(0.016043, mwG.CanonicalValue, 9);
        }

        [Fact]
        public void MolarMass_AirStandardConstant()
        {
            var air = MolarMass.AirStandard;
            Assert.Equal(28.9647, air.In("g/mol").Value, 4);
        }

        // ───── MolarEnergy ──────────────────────────────────────────

        [Fact]
        public void MolarEnergy_BTUperLbmol_RoundTrip()
        {
            // 1 BTU/lbmol = 1055.05585 J / 453.59237 mol ≈ 2.32600 J/mol
            var u = new MolarEnergy(1.0, "BTU/lbmol");
            Assert.Equal(2.32600, u.In("J/mol").Value, 4);
        }

        [Fact]
        public void MolarEnergy_KJperKmol_EqualsJperMol()
        {
            var a = new MolarEnergy(1000.0, "J/mol");
            var b = new MolarEnergy(1000.0, "kJ/kmol");
            Assert.Equal(a.CanonicalValue, b.CanonicalValue, 9);
        }

        // ───── MolarHeatCapacity ────────────────────────────────────

        [Fact]
        public void MolarHeatCapacity_BTUperLbmolR_NistConsistent()
        {
            // 1 BTU/(lbmol·R) → J/(mol·K).
            // factor = (1055.05585 J / 453.59237 mol) × (1.8 K/R)
            //        ≈ 4.18680 J/(mol·K)
            var cp = new MolarHeatCapacity(1.0, "BTU/(lbmol*R)");
            Assert.Equal(4.18680, cp.In("J/(mol*K)").Value, 4);
        }

        // ───── MolarDensity ─────────────────────────────────────────

        [Theory]
        [InlineData("mol/m^3",  1000.0,    1000.0)]
        [InlineData("mol/L",    1.0,       1000.0)]
        [InlineData("kmol/m^3", 1.0,       1000.0)]
        public void MolarDensity_CanonicalConversion(string sym, double v, double expected_molPerM3)
        {
            var d = new MolarDensity(v, sym);
            Assert.Equal(expected_molPerM3, d.CanonicalValue, 6);
        }

        [Fact]
        public void MolarDensity_LbmolPerFt3_RoundTrip()
        {
            // 1 lbmol/ft³ = 453.59237 mol / 0.02831685 m³ ≈ 16018.46 mol/m³
            var d = new MolarDensity(1.0, "lbmol/ft^3");
            Assert.Equal(16018.46, d.In("mol/m^3").Value, 1);
        }

        // ───── MixtureMolarConversions bridge ───────────────────────

        [Fact]
        public void Bridge_MolarEnergy_ToSpecificEnergy_Methane()
        {
            // 1164.7 J/mol of methane (MW = 16.043 g/mol):
            //   J/mol ÷ kg/mol  =  J/kg
            //   1164.7 / 0.016043 ≈ 72598.6 J/kg
            var H_molar = new MolarEnergy(1164.7, "J/mol");
            var MW      = new MolarMass(16.043, "g/mol");
            var H_mass  = MixtureMolarConversions.ToMassBasis(H_molar, MW);
            Assert.Equal(72598.6, H_mass.In("J/kg").Value, 1);
        }

        [Fact]
        public void Bridge_MolarHeatCapacity_ToSpecificHeatCapacity()
        {
            // Cp = 58.546 J/(mol·K), MW = 20.5433 g/mol → ~2849.88 J/(kg·K)
            var Cp_molar = new MolarHeatCapacity(58.546, "J/(mol*K)");
            var MW       = new MolarMass(20.5433, "g/mol");
            var Cp_mass  = MixtureMolarConversions.ToMassBasis(Cp_molar, MW);
            Assert.Equal(2849.88, Cp_mass.In("J/(kg*K)").Value, 1);
        }

        [Fact]
        public void Bridge_MolarDensity_ToMassDensity()
        {
            // 12.808 mol/L of mixture with MW=20.5433 g/mol:
            //   ρ_mass = 12.808 mol/L × 20.5433 g/mol = 263.12 g/L = 263.12 kg/m³
            var D_molar = new MolarDensity(12.808, "mol/L");
            var MW      = new MolarMass(20.5433, "g/mol");
            var D_mass  = MixtureMolarConversions.ToMassBasis(D_molar, MW);
            Assert.Equal(263.12, D_mass.In("kg/m^3").Value, 1);
        }

        [Fact]
        public void Bridge_RoundTrip_MolarToMassAndBack()
        {
            var orig = new MolarEnergy(1234.5, "J/mol");
            var MW   = new MolarMass(20.5433, "g/mol");
            var mass = MixtureMolarConversions.ToMassBasis(orig, MW);
            var back = MixtureMolarConversions.ToMolarBasis(mass, MW);
            Assert.Equal(orig.CanonicalValue, back.CanonicalValue, 9);
        }

        [Fact]
        public void Bridge_ZeroMW_Throws()
        {
            var H_molar = new MolarEnergy(1000.0, "J/mol");
            var MW0     = new MolarMass(0.0, "g/mol");
            Assert.Throws<ArgumentException>(() =>
                MixtureMolarConversions.ToMassBasis(H_molar, MW0));
        }

        // ───── Dimension safety ─────────────────────────────────────

        [Fact]
        public void MolarEnergy_WrongUnit_Throws()
        {
            Assert.Throws<DimensionMismatchException>(() => new MolarEnergy(1.0, "J/kg"));
        }

        [Fact]
        public void MolarDensity_WrongUnit_Throws()
        {
            Assert.Throws<DimensionMismatchException>(() => new MolarDensity(1.0, "kg/m^3"));
        }
    }
}
