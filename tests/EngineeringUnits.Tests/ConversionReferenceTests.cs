using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace EngineeringUnits.Tests
{
    /// <summary>
    /// Data-driven conversion-correctness tests. Each row in ReferenceData/*.csv
    /// asserts that 1 unit converts to the listed canonical-base value.
    /// </summary>
    public class ConversionReferenceTests
    {
        public static IEnumerable<object[]> LengthRows() => Load("length.csv");
        public static IEnumerable<object[]> MassRows() => Load("mass.csv");
        public static IEnumerable<object[]> TimeRows() => Load("time.csv");

        private static IEnumerable<object[]> Load(string fileName)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "ReferenceData", fileName);
            foreach (var line in File.ReadAllLines(path).Skip(1))   // skip header
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                var parts = line.Split(',');
                yield return new object[] {
                    parts[0].Trim(),
                    double.Parse(parts[1].Trim(), System.Globalization.CultureInfo.InvariantCulture)
                };
            }
        }

        [Theory]
        [MemberData(nameof(LengthRows))]
        public void Length_ConvertsToCanonicalMeters(string unit, double expectedMeters)
        {
            var l = new Length(1, unit);
            Assert.Equal(expectedMeters, l.CanonicalValue, 12);
        }

        [Theory]
        [MemberData(nameof(MassRows))]
        public void Mass_ConvertsToCanonicalKilograms(string unit, double expectedKg)
        {
            var m = new Mass(1, unit);
            Assert.Equal(expectedKg, m.CanonicalValue, 12);
        }

        [Theory]
        [MemberData(nameof(TimeRows))]
        public void Time_ConvertsToCanonicalSeconds(string unit, double expectedSeconds)
        {
            var t = new Time(1, unit);
            Assert.Equal(expectedSeconds, t.CanonicalValue, 12);
        }
    }
}
