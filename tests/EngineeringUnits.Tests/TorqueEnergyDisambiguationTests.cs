using Xunit;

namespace EngineeringUnits.Tests
{
    /// <summary>
    /// Decision 14.14: Torque and Energy share dimension M·L²·T⁻² but are
    /// disambiguated by operand order. <c>Force * Length → Torque</c>,
    /// <c>Length * Force → Energy</c>.
    /// </summary>
    public class TorqueEnergyDisambiguationTests
    {
        [Fact]
        public void ForceTimesLength_IsTorque()
        {
            var result = new Force(10, "N") * new Length(2, "m");
            Assert.IsType<Torque>(result);
            Assert.Equal(20.0, result.CanonicalValue, 9);
        }

        [Fact]
        public void LengthTimesForce_IsEnergy()
        {
            var result = new Length(2, "m") * new Force(10, "N");
            Assert.IsType<Energy>(result);
            Assert.Equal(20.0, result.CanonicalValue, 9);
        }

        [Fact]
        public void Torque_AsEnergy_PreservesCanonicalValue()
        {
            var torque = new Torque(50, "N*m");
            var energy = torque.AsEnergy();
            Assert.Equal(torque.CanonicalValue, energy.CanonicalValue);
        }

        [Fact]
        public void Energy_AsTorque_PreservesCanonicalValue()
        {
            var energy = new Energy(100, "J");
            var torque = energy.AsTorque();
            Assert.Equal(energy.CanonicalValue, torque.CanonicalValue);
        }

        [Fact]
        public void TorqueAndEnergy_HaveSameDimension()
        {
            Assert.Equal(new Torque(1, "N*m").Dimension, new Energy(1, "J").Dimension);
        }

        [Fact]
        public void EnergyDisplayUnit_IsLengthFirstByDefault()
        {
            var e = new Length(2, "m") * new Force(10, "N");
            Assert.Equal("J", e.DisplayUnit.Symbol);
            // ft*lbf is also length-first
            var e2 = new Energy(1, "ft*lbf");
            Assert.Equal("ft*lbf", e2.DisplayUnit.Symbol);
        }

        [Fact]
        public void TorqueDisplayUnit_IsForceFirstByDefault()
        {
            var t = new Force(10, "N") * new Length(2, "m");
            Assert.Equal("N*m", t.DisplayUnit.Symbol);
            // lbf*ft is force-first
            var t2 = new Torque(1, "lbf*ft");
            Assert.Equal("lbf*ft", t2.DisplayUnit.Symbol);
        }
    }
}
