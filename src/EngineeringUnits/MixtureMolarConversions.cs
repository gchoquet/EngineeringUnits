using System;

namespace EngineeringUnits
{
    /// <summary>
    /// Bridges molar-basis quantities (e.g. <see cref="MolarEnergy"/> in J/mol)
    /// to mass-basis quantities (e.g. <see cref="SpecificEnergy"/> in J/kg) and
    /// back, using a mixture's <see cref="MolarMass"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Pure dimensional analysis can't perform this conversion: molar mass is a
    /// mixture property, not a unit. So this bridge takes the MW explicitly,
    /// keeping the call sites self-documenting:
    /// <code>
    /// var H_molar = new MolarEnergy(1164.7, "J/mol");
    /// var MW      = new MolarMass(20.5433, "g/mol");
    /// var H_mass  = MixtureMolarConversions.ToMassBasis(H_molar, MW);   // → J/kg
    /// </code>
    /// </para>
    /// <para>
    /// All conversions go through SI canonical units internally
    /// (kg/mol for MW, J/mol for energy, etc.), so the user can supply
    /// any input unit and request any output unit.
    /// </para>
    /// </remarks>
    public static class MixtureMolarConversions
    {
        /// <summary>
        /// Convert molar energy (J/mol) to specific (mass-basis) energy (J/kg):
        /// <c>e_specific = e_molar / MW</c>.
        /// </summary>
        public static SpecificEnergy ToMassBasis(MolarEnergy molar, MolarMass mw)
        {
            if (molar is null) throw new ArgumentNullException(nameof(molar));
            if (mw    is null) throw new ArgumentNullException(nameof(mw));
            if (mw.CanonicalValue <= 0)
                throw new ArgumentException("Molar mass must be positive.", nameof(mw));
            // canonical: J/mol ÷ kg/mol = J/kg
            return new SpecificEnergy(UnitCatalog.Get("J/kg"), molar.CanonicalValue / mw.CanonicalValue);
        }

        /// <summary>
        /// Convert molar heat capacity (J/(mol·K)) to specific heat capacity
        /// (J/(kg·K)): <c>cp_specific = cp_molar / MW</c>.
        /// </summary>
        public static SpecificHeatCapacity ToMassBasis(MolarHeatCapacity molar, MolarMass mw)
        {
            if (molar is null) throw new ArgumentNullException(nameof(molar));
            if (mw    is null) throw new ArgumentNullException(nameof(mw));
            if (mw.CanonicalValue <= 0)
                throw new ArgumentException("Molar mass must be positive.", nameof(mw));
            // J/(mol·K) ÷ kg/mol = J/(kg·K)
            return new SpecificHeatCapacity(UnitCatalog.Get("J/(kg*K)"), molar.CanonicalValue / mw.CanonicalValue);
        }

        /// <summary>
        /// Convert molar density (mol/m³) to mass density (kg/m³):
        /// <c>ρ_mass = ρ_molar * MW</c>.
        /// </summary>
        public static Density ToMassBasis(MolarDensity molar, MolarMass mw)
        {
            if (molar is null) throw new ArgumentNullException(nameof(molar));
            if (mw    is null) throw new ArgumentNullException(nameof(mw));
            // mol/m³ * kg/mol = kg/m³
            return new Density(UnitCatalog.Get("kg/m^3"), molar.CanonicalValue * mw.CanonicalValue);
        }

        // ── Reverse direction ─────────────────────────────────────────

        /// <summary>
        /// Convert specific energy (J/kg) to molar energy (J/mol):
        /// <c>e_molar = e_specific * MW</c>.
        /// </summary>
        public static MolarEnergy ToMolarBasis(SpecificEnergy mass, MolarMass mw)
        {
            if (mass is null) throw new ArgumentNullException(nameof(mass));
            if (mw   is null) throw new ArgumentNullException(nameof(mw));
            return new MolarEnergy(UnitCatalog.Get("J/mol"), mass.CanonicalValue * mw.CanonicalValue);
        }

        /// <summary>
        /// Convert specific heat capacity (J/(kg·K)) to molar heat capacity
        /// (J/(mol·K)): <c>cp_molar = cp_specific * MW</c>.
        /// </summary>
        public static MolarHeatCapacity ToMolarBasis(SpecificHeatCapacity mass, MolarMass mw)
        {
            if (mass is null) throw new ArgumentNullException(nameof(mass));
            if (mw   is null) throw new ArgumentNullException(nameof(mw));
            return new MolarHeatCapacity(UnitCatalog.Get("J/(mol*K)"), mass.CanonicalValue * mw.CanonicalValue);
        }

        /// <summary>
        /// Convert mass density (kg/m³) to molar density (mol/m³):
        /// <c>ρ_molar = ρ_mass / MW</c>.
        /// </summary>
        public static MolarDensity ToMolarBasis(Density mass, MolarMass mw)
        {
            if (mass is null) throw new ArgumentNullException(nameof(mass));
            if (mw   is null) throw new ArgumentNullException(nameof(mw));
            if (mw.CanonicalValue <= 0)
                throw new ArgumentException("Molar mass must be positive.", nameof(mw));
            return new MolarDensity(UnitCatalog.Get("mol/m^3"), mass.CanonicalValue / mw.CanonicalValue);
        }
    }
}
