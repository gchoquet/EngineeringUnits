using Xunit;

namespace EngineeringUnits.Tests
{
    public class TierCElectricalTests
    {
        [Fact]
        public void OhmsLaw_VoltageOverCurrent_IsResistance()
        {
            var v = new Voltage(12, "V");
            var i = new ElectricCurrent(2, "A");
            var r = v / i;
            Assert.IsType<ElectricResistance>(r);
            Assert.Equal(6.0, r.CanonicalValue, 9);
        }

        [Fact]
        public void OhmsLaw_VoltageOverResistance_IsCurrent()
        {
            var v = new Voltage(120, "V");
            var r = new ElectricResistance(60, "Ω");
            var i = v / r;
            Assert.IsType<ElectricCurrent>(i);
            Assert.Equal(2.0, i.CanonicalValue, 9);
        }

        [Fact]
        public void OhmsLaw_ResistanceTimesCurrent_IsVoltage()
        {
            var r = new ElectricResistance(10, "Ω");
            var i = new ElectricCurrent(0.5, "A");
            var v = r * i;
            Assert.IsType<Voltage>(v);
            Assert.Equal(5.0, v.CanonicalValue, 9);
        }

        [Fact]
        public void Power_FromVoltageTimesCurrent()
        {
            var v = new Voltage(120, "V");
            var i = new ElectricCurrent(10, "A");
            var p = v * i;
            Assert.IsType<Power>(p);
            Assert.Equal(1200.0, p.CanonicalValue, 9);
        }

        [Fact]
        public void Power_FromCurrentTimesVoltage_AlsoWorks()
        {
            var i = new ElectricCurrent(2, "A");
            var v = new Voltage(50, "V");
            var p = i * v;
            Assert.IsType<Power>(p);
            Assert.Equal(100.0, p.CanonicalValue, 9);
        }

        [Fact]
        public void Charge_FromCurrentTimesTime()
        {
            var i = new ElectricCurrent(0.5, "A");
            var t = new Time(2, "s");
            var q = i * t;
            Assert.IsType<ElectricCharge>(q);
            Assert.Equal(1.0, q.CanonicalValue, 9);
        }

        [Fact]
        public void Current_FromChargeOverTime()
        {
            var q = new ElectricCharge(3600, "C");
            var t = new Time(1, "h");
            var i = q / t;
            Assert.IsType<ElectricCurrent>(i);
            Assert.Equal(1.0, i.CanonicalValue, 9);
        }

        [Fact]
        public void Energy_FromChargeTimesVoltage()
        {
            var q = new ElectricCharge(1, "C");
            var v = new Voltage(1, "V");
            var e = q * v;
            Assert.IsType<Energy>(e);
            Assert.Equal(1.0, e.CanonicalValue, 9);   // 1 C × 1 V = 1 J
        }

        [Fact]
        public void Charge_FromCapacitanceTimesVoltage()
        {
            var c = new ElectricCapacitance(1, "μF");
            var v = new Voltage(5, "V");
            var q = c * v;
            Assert.IsType<ElectricCharge>(q);
            Assert.Equal(5e-6, q.CanonicalValue, 12);
        }

        [Fact]
        public void AmpereHour_IsRecognized()
        {
            var q = new ElectricCharge(1, "Ah");
            Assert.Equal(3600.0, q.As("C"), 9);
        }

        [Fact]
        public void Prefixes_kV_mV_uV()
        {
            Assert.Equal(1000.0, new Voltage(1, "kV").As("V"), 9);
            Assert.Equal(1e-3,   new Voltage(1, "mV").As("V"), 9);
            Assert.Equal(1e-6,   new Voltage(1, "μV").As("V"), 9);
            Assert.Equal(1e-6,   new Voltage(1, "uV").As("V"), 9);
        }
    }
}
