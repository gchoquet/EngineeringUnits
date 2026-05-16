using Xunit;

namespace EngineeringUnits.Tests
{
    public class CrossTypeOperatorTests
    {
        [Fact]
        public void Length_TimesLength_IsArea()
        {
            var product = new Length(3, "m") * new Length(4, "m");
            Assert.IsType<Area>(product);
            Assert.Equal(12.0, product.CanonicalValue, 9);
        }

        [Fact]
        public void Length_TimesArea_IsVolume()
        {
            var product = new Length(2, "m") * new Area(5, "m^2");
            Assert.IsType<Volume>(product);
            Assert.Equal(10.0, product.CanonicalValue, 9);
        }

        [Fact]
        public void Length_DividedByTime_IsVelocity()
        {
            var v = new Length(100, "m") / new Time(10, "s");
            Assert.IsType<Velocity>(v);
            Assert.Equal(10.0, v.CanonicalValue, 9);
        }

        [Fact]
        public void Velocity_DividedByTime_IsAcceleration()
        {
            var a = new Velocity(10, "m/s") / new Time(2, "s");
            Assert.IsType<Acceleration>(a);
            Assert.Equal(5.0, a.CanonicalValue, 9);
        }

        [Fact]
        public void Mass_TimesAcceleration_IsForce_NewtonsSecondLaw()
        {
            var F = new Mass(10, "kg") * new Acceleration(9.81, "m/s^2");
            Assert.IsType<Force>(F);
            Assert.Equal(98.1, F.CanonicalValue, 9);
        }

        [Fact]
        public void Force_DividedByArea_IsPressure()
        {
            var p = new Force(1000, "N") / new Area(0.5, "m^2");
            Assert.IsType<Pressure>(p);
            Assert.Equal(2000.0, p.CanonicalValue, 9);
        }

        [Fact]
        public void Pressure_TimesArea_IsForce()
        {
            var f = new Pressure(100, "kPa") * new Area(0.1, "m^2");
            Assert.IsType<Force>(f);
            Assert.Equal(10000.0, f.CanonicalValue, 9);
        }

        [Fact]
        public void Energy_DividedByTime_IsPower()
        {
            var p = new Energy(3600, "J") / new Time(1, "h");
            Assert.IsType<Power>(p);
            Assert.Equal(1.0, p.CanonicalValue, 9);
        }

        [Fact]
        public void Power_TimesTime_IsEnergy()
        {
            var e = new Power(1, "kW") * new Time(1, "h");
            Assert.IsType<Energy>(e);
            Assert.Equal(3.6e6, e.CanonicalValue, 9);
        }

        [Fact]
        public void Mass_DividedByVolume_IsDensity()
        {
            var d = new Mass(1000, "kg") / new Volume(1, "m^3");
            Assert.IsType<Density>(d);
            Assert.Equal(1000.0, d.CanonicalValue, 9);
        }

        [Fact]
        public void Density_TimesVolume_IsMass()
        {
            var m = new Density(1000, "kg/m^3") * new Volume(0.5, "m^3");
            Assert.IsType<Mass>(m);
            Assert.Equal(500.0, m.CanonicalValue, 9);
        }

        [Fact]
        public void OneOverTime_IsFrequency()
        {
            var f = 1.0 / new Time(0.5, "s");
            Assert.IsType<Frequency>(f);
            Assert.Equal(2.0, f.CanonicalValue, 9);
        }

        [Fact]
        public void Angle_DividedByTime_IsAngularVelocity()
        {
            var av = new PlaneAngle(2 * System.Math.PI, "rad") / new Time(1, "s");
            Assert.IsType<AngularVelocity>(av);
            Assert.Equal(2 * System.Math.PI, av.CanonicalValue, 9);
        }
    }
}
