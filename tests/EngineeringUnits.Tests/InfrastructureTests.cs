using System;
using Xunit;

namespace EngineeringUnits.Tests
{
    public class PowTests
    {
        [Fact]
        public void LengthPow2_IsArea()
        {
            var l = new Length(3, "m");
            var a = l.Pow(2);
            Assert.IsType<Area>(a);
            Assert.Equal(9.0, a.CanonicalValue, 9);
        }

        [Fact]
        public void LengthPow3_IsVolume()
        {
            var l = new Length(2, "m");
            var v = l.Pow(3);
            Assert.IsType<Volume>(v);
            Assert.Equal(8.0, v.CanonicalValue, 9);
        }

        [Fact]
        public void Pow0_IsDimensionless()
        {
            var l = new Length(5, "ft");
            var p = l.Pow(0);
            Assert.IsType<DimensionlessQuantity>(p);
            Assert.Equal(1.0, p.Value, 9);
        }

        [Fact]
        public void Pow1_IsIdentity()
        {
            var l = new Length(5, "ft");
            var p = l.Pow(1);
            Assert.IsType<Length>(p);
            Assert.Equal(l.CanonicalValue, p.CanonicalValue, 12);
        }

        [Fact]
        public void AreaPowHalf_IsLength()
        {
            var a = new Area(9, "m^2");
            var l = a.Pow(0.5);
            Assert.IsType<Length>(l);
            Assert.Equal(3.0, l.CanonicalValue, 9);
        }

        [Fact]
        public void VolumePowOneThird_IsLength()
        {
            var v = new Volume(27, "m^3");
            var l = v.Pow(1.0 / 3.0);
            Assert.IsType<Length>(l);
            Assert.Equal(3.0, l.CanonicalValue, 9);
        }

        [Fact]
        public void NonIntegerDimension_Throws()
        {
            // Length.Pow(0.5) would yield L^0.5 which is not a registered dimension
            var l = new Length(5, "m");
            Assert.Throws<InvalidOperationException>(() => l.Pow(0.5));
        }
    }

    public class AbsTests
    {
        [Fact]
        public void Abs_PreservesSubclassType()
        {
            var l = new Length(-5, "ft");
            var a = l.Abs();
            Assert.IsType<Length>(a);
            Assert.Equal(5.0, ((Length)a).Value, 9);
        }

        [Fact]
        public void Abs_PreservesDisplayUnit()
        {
            var l = new Length(-5, "ft");
            var a = l.Abs();
            Assert.Equal("ft", a.DisplayUnit.Symbol);
        }

        [Fact]
        public void Abs_OnPositiveValueIsIdentity()
        {
            var l = new Length(5, "ft");
            Assert.Equal(l.CanonicalValue, l.Abs().CanonicalValue, 12);
        }
    }

    public class TemperatureDeltaTests
    {
        [Fact]
        public void Construct_FromKelvinDelta()
        {
            var d = new TemperatureDelta(10, "K");
            Assert.Equal(10.0, d.Value, 9);
            Assert.Equal(10.0, d.CanonicalValue, 9);  // K is already canonical
        }

        [Fact]
        public void Construct_FromCelsiusDelta_IsSameAsKelvinDelta()
        {
            // 1 °C delta = 1 K delta (linear; offset ignored)
            var d = new TemperatureDelta(10, "degC");
            Assert.Equal(10.0, d.CanonicalValue, 9);
        }

        [Fact]
        public void Construct_FromFahrenheitDelta_ScalesByFiveNinths()
        {
            // 18 °F delta = 10 K delta
            var d = new TemperatureDelta(18, "degF");
            Assert.Equal(10.0, d.CanonicalValue, 9);
        }

        [Fact]
        public void TemperatureMinusTemperature_IsTemperatureDelta()
        {
            var diff = new Temperature(30, "degC") - new Temperature(20, "degC");
            Assert.IsType<TemperatureDelta>(diff);
            Assert.Equal(10.0, diff.Value, 9);  // value in °C-delta
        }

        [Fact]
        public void TemperaturePlusDelta_IsTemperature()
        {
            var t0 = new Temperature(20, "degC");
            var d = new TemperatureDelta(5, "degC");
            var t1 = t0 + d;
            Assert.IsType<Temperature>(t1);
            Assert.Equal("degC", t1.DisplayUnit.Symbol);
            Assert.Equal(25.0, t1.Value, 9);
        }

        [Fact]
        public void TemperatureMinusDelta_IsTemperature()
        {
            var t0 = new Temperature(100, "degF");
            var d = new TemperatureDelta(50, "degF");
            var t1 = t0 - d;
            Assert.IsType<Temperature>(t1);
            Assert.Equal(50.0, t1.Value, 9);
        }

        [Fact]
        public void DeltaPlusDelta_IsDelta()
        {
            var d1 = new TemperatureDelta(5, "K");
            var d2 = new TemperatureDelta(3, "K");
            var sum = d1 + d2;
            Assert.IsType<TemperatureDelta>(sum);
            Assert.Equal(8.0, sum.Value, 9);
        }

        [Fact]
        public void As_AcrossUnits_AppliesLinearConversionOnly()
        {
            var d = new TemperatureDelta(10, "K");
            Assert.Equal(10.0, d.As("degC"), 9);           // 1 K = 1 °C delta
            Assert.Equal(18.0, d.As("degF"), 9);           // 10 K = 18 °F delta
        }

        [Fact]
        public void ToString_UsesDeltaSymbol()
        {
            var d = new TemperatureDelta(10, "K");
            Assert.Contains("Δ", d.ToString());
        }
    }
}
