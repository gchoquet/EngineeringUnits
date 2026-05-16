using System;
using System.Globalization;

namespace EngineeringUnits
{
    /// <summary>A length quantity. SI base unit: meter.</summary>
    /// <remarks>
    /// Supported units include m, km, cm, mm, μm (um), nm, Mm, in, ft, yd, mi, nmi,
    /// fathom, and furlong. See <see cref="UnitCatalog"/> for the full enumeration.
    /// </remarks>
    public sealed class Length : EngineeringUnit
    {
        /// <summary>Creates a length from a value and unit symbol.</summary>
        /// <param name="value">The numeric value in the given unit.</param>
        /// <param name="unit">The unit symbol, e.g. <c>"ft"</c>.</param>
        /// <exception cref="UnknownUnitException">If the symbol is not registered.</exception>
        /// <exception cref="DimensionMismatchException">If the symbol is registered but is not a length unit.</exception>
        public Length(double value, string unit) : base(value, RequireLengthUnit(unit)) { }

        internal Length(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        private static Unit RequireLengthUnit(string symbol)
        {
            var u = UnitCatalog.Get(symbol);
            if (u.Dimension != DimensionSignature.Length)
                throw new DimensionMismatchException(DimensionSignature.Length, u.Dimension);
            return u;
        }

        /// <summary>Returns a new <see cref="Length"/> with the same canonical value expressed in the named unit.</summary>
        public Length In(string unit) => new Length(RequireLengthUnit(unit), CanonicalValue) { Precision = Precision };

        /// <summary>Parses a length from a string of the form <c>"5 ft"</c>.</summary>
        public static Length Parse(string s)
        {
            if (!TryParse(s, out var result))
                throw new UnitParseException(s ?? "(null)", "expected '<value> <length-unit>'");
            return result!;
        }

        /// <summary>Attempts to parse a length from a string. Returns false on any failure.</summary>
        public static bool TryParse(string? s, out Length? result)
        {
            result = null;
            if (string.IsNullOrWhiteSpace(s)) return false;
            var trimmed = s!.Trim();
            var space = trimmed.IndexOf(' ');
            if (space < 0) return false;
            var numPart = trimmed.Substring(0, space).Trim();
            var unitPart = trimmed.Substring(space + 1).Trim();
            if (!double.TryParse(numPart, NumberStyles.Float, CultureInfo.InvariantCulture, out var value))
                return false;
            if (!UnitCatalog.TryGet(unitPart, out var unit)) return false;
            if (unit.Dimension != DimensionSignature.Length) return false;
            result = new Length(unit, unit.ToCanonical(value));
            return true;
        }

        // ── Operators ─────────────────────────────────────────────

        public static Length operator +(Length a, Length b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new Length(a.DisplayUnit, a.CanonicalValue + b.CanonicalValue)
            { Precision = Math.Min(a.Precision, b.Precision) };
        }

        public static Length operator -(Length a, Length b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new Length(a.DisplayUnit, a.CanonicalValue - b.CanonicalValue)
            { Precision = Math.Min(a.Precision, b.Precision) };
        }

        public static Length operator -(Length a)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new Length(a.DisplayUnit, -a.CanonicalValue) { Precision = a.Precision };
        }

        public static Length operator *(Length a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new Length(a.DisplayUnit, a.CanonicalValue * scalar) { Precision = a.Precision };
        }

        public static Length operator *(double scalar, Length a) => a * scalar;

        public static Length operator /(Length a, double scalar)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            return new Length(a.DisplayUnit, a.CanonicalValue / scalar) { Precision = a.Precision };
        }

        // ── Cross-type operators ──────────────────────────────────

        /// <summary>Length * Length → Area.</summary>
        public static Area operator *(Length a, Length b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new Area(UnitCatalog.Get("m^2"), a.CanonicalValue * b.CanonicalValue);
        }

        /// <summary>Length * Area → Volume.</summary>
        public static Volume operator *(Length a, Area A)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (A is null) throw new ArgumentNullException(nameof(A));
            return new Volume(UnitCatalog.Get("m^3"), a.CanonicalValue * A.CanonicalValue);
        }

        /// <summary>Length / Time → Velocity.</summary>
        public static Velocity operator /(Length a, Time t)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (t is null) throw new ArgumentNullException(nameof(t));
            return new Velocity(UnitCatalog.Get("m/s"), a.CanonicalValue / t.CanonicalValue);
        }

        /// <summary>Length * Force → Energy (length-first display convention, per Decision 14.14).</summary>
        public static Energy operator *(Length L, Force F)
        {
            if (L is null) throw new ArgumentNullException(nameof(L));
            if (F is null) throw new ArgumentNullException(nameof(F));
            return new Energy(UnitCatalog.Get("J"), L.CanonicalValue * F.CanonicalValue);
        }

        /// <summary>Length / Length → DimensionlessQuantity (e.g. strain candidate).</summary>
        public static DimensionlessQuantity operator /(Length a, Length b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new DimensionlessQuantity(a.CanonicalValue / b.CanonicalValue);
        }

        public static bool operator <(Length a, Length b) => a.CompareTo(b) < 0;
        public static bool operator >(Length a, Length b) => a.CompareTo(b) > 0;
        public static bool operator <=(Length a, Length b) => a.CompareTo(b) <= 0;
        public static bool operator >=(Length a, Length b) => a.CompareTo(b) >= 0;
        public static bool operator ==(Length? a, Length? b)
        {
            if (a is null) return b is null;
            return a.Equals(b);
        }
        public static bool operator !=(Length? a, Length? b) => !(a == b);

        public override bool Equals(object? obj) => base.Equals(obj as EngineeringUnit);
        public override int GetHashCode() => base.GetHashCode();
    }
}
