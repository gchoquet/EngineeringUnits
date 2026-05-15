using System;
using FsCheck;
using FsCheck.Xunit;

namespace EngineeringUnits.Tests
{
    // Non-static so FsCheck's Arb.Register<T>() (with `where T : class, new()` semantics) accepts it.
    public class Generators
    {
        private static readonly string[] LengthUnits = { "m", "km", "cm", "mm", "in", "ft", "yd", "mi" };
        private static readonly string[] MassUnits   = { "kg", "g", "mg", "lb", "oz", "slug" };
        private static readonly string[] TimeUnits   = { "s", "ms", "min", "h", "day" };

        public static Arbitrary<Length> Length()
        {
            var gen = from nf in Arb.Default.NormalFloat().Generator
                      from u in Gen.Elements(LengthUnits)
                      select new Length(nf.Get, u);
            return Arb.From(gen);
        }

        public static Arbitrary<Mass> Mass()
        {
            var gen = from nf in Arb.Default.NormalFloat().Generator
                      from u in Gen.Elements(MassUnits)
                      select new Mass(nf.Get, u);
            return Arb.From(gen);
        }

        public static Arbitrary<Time> Time()
        {
            var gen = from nf in Arb.Default.NormalFloat().Generator
                      from u in Gen.Elements(TimeUnits)
                      select new Time(nf.Get, u);
            return Arb.From(gen);
        }
    }

    public class PropertyTests
    {
        public PropertyTests()
        {
            Arb.Register<Generators>();
        }

        // ── Length invariants ─────────────────────────────────────

        [Property] public bool Length_AdditionIsCommutative(Length a, Length b)
        {
            var lhs = (a + b).CanonicalValue;
            var rhs = (b + a).CanonicalValue;
            return Math.Abs(lhs - rhs) <= 1e-9 * Math.Max(Math.Abs(lhs), Math.Abs(rhs));
        }

        [Property] public bool Length_AdditionIsAssociative(Length a, Length b, Length c)
        {
            var lhs = ((a + b) + c).CanonicalValue;
            var rhs = (a + (b + c)).CanonicalValue;
            var scale = Math.Max(Math.Abs(lhs), Math.Abs(rhs));
            return Math.Abs(lhs - rhs) <= 1e-9 * Math.Max(scale, 1.0);
        }

        [Property] public bool Length_SubtractInverse(Length q)
        {
            var zero = q - q;
            return Math.Abs(zero.CanonicalValue) <= 1e-9 * Math.Max(Math.Abs(q.CanonicalValue), 1.0);
        }

        [Property] public bool Length_LeftOperandWinsForDisplayUnit(Length a, Length b)
        {
            return (a + b).DisplayUnit.Symbol == a.DisplayUnit.Symbol;
        }

        [Property] public bool Length_ConversionRoundtrip(Length q)
        {
            var originalUnit = q.DisplayUnit.Symbol;
            var roundtripped = q.In("mm").In(originalUnit);
            var scale = Math.Max(Math.Abs(q.Value), 1.0);
            return Math.Abs(roundtripped.Value - q.Value) <= 1e-9 * scale;
        }

        [Property] public bool Length_ParseFormatRoundtrip(Length q)
        {
            var v = Math.Abs(q.Value);
            if (v != 0 && (v < 1e-3 || v >= 1e6)) return true;  // skip extreme magnitudes
            var s = q.ToString();
            if (!Length.TryParse(s, out var parsed)) return false;
            return q.Equals(parsed!, 1e-3);  // ToString rounds to 4 sig figs
        }

        // ── Mass invariants ───────────────────────────────────────

        [Property] public bool Mass_AdditionIsCommutative(Mass a, Mass b)
        {
            var lhs = (a + b).CanonicalValue;
            var rhs = (b + a).CanonicalValue;
            return Math.Abs(lhs - rhs) <= 1e-9 * Math.Max(Math.Abs(lhs), Math.Abs(rhs));
        }

        [Property] public bool Mass_SubtractInverse(Mass q)
        {
            var zero = q - q;
            return Math.Abs(zero.CanonicalValue) <= 1e-9 * Math.Max(Math.Abs(q.CanonicalValue), 1.0);
        }

        // ── Time invariants ───────────────────────────────────────

        [Property] public bool Time_AdditionIsCommutative(Time a, Time b)
        {
            var lhs = (a + b).CanonicalValue;
            var rhs = (b + a).CanonicalValue;
            return Math.Abs(lhs - rhs) <= 1e-9 * Math.Max(Math.Abs(lhs), Math.Abs(rhs));
        }

        [Property] public bool Time_SubtractInverse(Time q)
        {
            var zero = q - q;
            return Math.Abs(zero.CanonicalValue) <= 1e-9 * Math.Max(Math.Abs(q.CanonicalValue), 1.0);
        }
    }
}
