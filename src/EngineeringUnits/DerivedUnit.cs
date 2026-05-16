using System;

namespace EngineeringUnits
{
    /// <summary>
    /// A quantity whose dimension does not match any registered subclass. Returned by
    /// arithmetic when the result combination has no dedicated type, e.g.
    /// <c>Mass * Length / Time^3</c>.
    /// </summary>
    /// <remarks>
    /// <see cref="DerivedUnit"/> instances arise from arithmetic; in v1 there is no
    /// dynamic-dispatch path that creates them on construction. Their display unit
    /// is composed by the operation that produced them (e.g. <c>kg*m/s^3</c>).
    /// </remarks>
    public sealed class DerivedUnit : EngineeringUnit
    {
        internal DerivedUnit(Unit displayUnit, double canonicalValue) : base(displayUnit, canonicalValue) { }

        /// <summary>
        /// Constructs an ad-hoc derived unit from a composed symbol and a dimension.
        /// Used by arithmetic operators to build display units like <c>kg*m/s^3</c>.
        /// </summary>
        internal static Unit ComposedUnit(string symbol, DimensionSignature dim, double scale)
        {
            return new Unit(symbol, symbol, dim, scale);
        }
    }
}
