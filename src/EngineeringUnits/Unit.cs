using System;

namespace EngineeringUnits
{
    /// <summary>
    /// A unit of measurement: a name, a dimension, and conversion factors to/from
    /// the canonical SI-base value.
    /// </summary>
    /// <remarks>
    /// Conversion is affine: <c>canonical = display * Scale + Offset</c>.
    /// Offset is non-zero only for absolute temperature scales (°C, °F).
    /// All other units are linear (offset = 0).
    /// </remarks>
    public readonly struct Unit : IEquatable<Unit>
    {
        /// <summary>Short symbol, e.g. <c>"ft"</c>, <c>"m/s"</c>, <c>"psi"</c>.</summary>
        public string Symbol { get; }

        /// <summary>Full name, e.g. <c>"foot"</c>, <c>"meter per second"</c>.</summary>
        public string LongName { get; }

        /// <summary>The physical dimension this unit measures.</summary>
        public DimensionSignature Dimension { get; }

        /// <summary>Multiplicative conversion factor from this unit to the SI canonical unit.</summary>
        public double Scale { get; }

        /// <summary>Additive offset applied after scaling (used by absolute temperature scales).</summary>
        public double Offset { get; }

        /// <summary>Creates a unit definition.</summary>
        public Unit(string symbol, string longName, DimensionSignature dimension, double scale, double offset = 0.0)
        {
            if (string.IsNullOrEmpty(symbol)) throw new ArgumentException("Symbol must be non-empty.", nameof(symbol));
            if (longName is null) throw new ArgumentNullException(nameof(longName));
            if (scale == 0.0 || double.IsNaN(scale) || double.IsInfinity(scale))
                throw new ArgumentException("Scale must be a finite non-zero number.", nameof(scale));
            Symbol = symbol;
            LongName = longName;
            Dimension = dimension;
            Scale = scale;
            Offset = offset;
        }

        /// <summary>Converts a value in this display unit to canonical SI-base units.</summary>
        public double ToCanonical(double displayValue) => displayValue * Scale + Offset;

        /// <summary>Converts a value in canonical SI-base units to this display unit.</summary>
        public double FromCanonical(double canonicalValue) => (canonicalValue - Offset) / Scale;

        /// <summary>Two units are equal when their symbols match exactly (case-sensitive).</summary>
        public bool Equals(Unit other) => string.Equals(Symbol, other.Symbol, StringComparison.Ordinal);

        public override bool Equals(object? obj) => obj is Unit other && Equals(other);
        public override int GetHashCode() => Symbol?.GetHashCode() ?? 0;
        public static bool operator ==(Unit a, Unit b) => a.Equals(b);
        public static bool operator !=(Unit a, Unit b) => !a.Equals(b);
        public override string ToString() => Symbol ?? "";
    }
}
