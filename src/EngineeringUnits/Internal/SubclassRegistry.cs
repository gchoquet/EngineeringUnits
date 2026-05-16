using System;
using System.Collections.Generic;

namespace EngineeringUnits.Internal
{
    /// <summary>
    /// Maps a <see cref="DimensionSignature"/> to a factory that produces the
    /// corresponding <see cref="EngineeringUnit"/> subclass. Falls back to
    /// <see cref="DerivedUnit"/> if no subclass matches.
    /// </summary>
    /// <remarks>
    /// Populated at module load by <see cref="UnitCatalog"/>'s static constructor.
    /// Reads are lock-free after that point.
    /// </remarks>
    internal static class SubclassRegistry
    {
        private static readonly Dictionary<DimensionSignature, Func<Unit, double, EngineeringUnit>> _factories
            = new Dictionary<DimensionSignature, Func<Unit, double, EngineeringUnit>>();

        private static readonly Dictionary<DimensionSignature, string> _canonicalSymbols
            = new Dictionary<DimensionSignature, string>();

        /// <summary>Registers a subclass factory for the given dimension.</summary>
        /// <param name="dim">The dimension this factory produces.</param>
        /// <param name="canonicalSymbol">The SI canonical unit symbol for this dimension (e.g. "m" for Length, "N" for Force).</param>
        /// <param name="factory">Constructs the subclass instance from a display unit + canonical value.</param>
        public static void Register(DimensionSignature dim, string canonicalSymbol, Func<Unit, double, EngineeringUnit> factory)
        {
            _factories[dim] = factory;
            _canonicalSymbols[dim] = canonicalSymbol;
        }

        /// <summary>
        /// Creates an <see cref="EngineeringUnit"/> instance for the given display unit
        /// and canonical value. Falls back to <see cref="DerivedUnit"/> if no subclass
        /// is registered for the unit's dimension.
        /// </summary>
        public static EngineeringUnit Create(Unit displayUnit, double canonicalValue)
        {
            if (_factories.TryGetValue(displayUnit.Dimension, out var f))
                return f(displayUnit, canonicalValue);
            return new DerivedUnit(displayUnit, canonicalValue);
        }

        /// <summary>
        /// Creates an <see cref="EngineeringUnit"/> for the given dimension using its
        /// canonical SI unit. Used by Pow / Abs and similar derivations.
        /// </summary>
        public static EngineeringUnit CreateCanonical(DimensionSignature dim, double canonicalValue)
        {
            if (_canonicalSymbols.TryGetValue(dim, out var symbol) && UnitCatalog.TryGet(symbol, out var unit))
                return Create(unit, canonicalValue);
            // No canonical-symbol registered — compose a synthetic display unit
            var synthetic = new Unit(dim.ToString(), dim.ToString(), dim, 1.0);
            return new DerivedUnit(synthetic, canonicalValue);
        }
    }
}
