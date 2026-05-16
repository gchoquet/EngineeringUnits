using System;
using System.Collections.Generic;

namespace EngineeringUnits
{
    /// <summary>Notation style for compound units in formatted output.</summary>
    public enum NotationStyle
    {
        /// <summary>ASCII caret form: <c>m/s^2</c>, <c>kg*m/s^2</c>. Default.</summary>
        Caret,
        /// <summary>Unicode superscript form: <c>m·s⁻²</c>, <c>kg·m·s⁻²</c>. Matches ISO 80000 print convention.</summary>
        Unicode
    }

    /// <summary>
    /// Per-dimension display-unit preferences plus notation style. Used by the
    /// <c>"P"</c> format code and by <see cref="EngineeringUnit.ToDualString"/>.
    /// </summary>
    /// <remarks>
    /// Preferences are a per-dimension map (Decision 14.17). There is no binary
    /// SI-vs-US toggle because even within SI the conventionally-preferred unit
    /// depends on the dimension and the application — meter for length, kilogram
    /// for mass, but kilometer for travel and millimeter for machining.
    /// <para>
    /// Built-in profiles (<see cref="SIScientific"/>, <see cref="SIEveryday"/>,
    /// <see cref="UsCustomary"/>, <see cref="UsEngineering"/>, <see cref="OilAndGas"/>,
    /// <see cref="Machining"/>) cover common contexts. <see cref="Clone"/> any of
    /// them to customize.
    /// </para>
    /// </remarks>
    public sealed class UnitPreferences
    {
        private readonly Dictionary<DimensionSignature, Unit> _map = new Dictionary<DimensionSignature, Unit>();

        /// <summary>Notation style for compound units.</summary>
        public NotationStyle Notation { get; set; } = NotationStyle.Caret;

        /// <summary>Sets the preferred display unit for whatever dimension this symbol carries.</summary>
        public void Prefer(string unitSymbol)
        {
            var u = UnitCatalog.Get(unitSymbol);
            _map[u.Dimension] = u;
        }

        /// <summary>Returns the preferred unit for the given dimension, or null if no preference is set.</summary>
        public Unit? GetPreferred(DimensionSignature dim) =>
            _map.TryGetValue(dim, out var u) ? u : (Unit?)null;

        /// <summary>Creates a deep copy of this preferences object so callers can customize without affecting the source.</summary>
        public UnitPreferences Clone()
        {
            var copy = new UnitPreferences { Notation = this.Notation };
            foreach (var kvp in _map) copy._map[kvp.Key] = kvp.Value;
            return copy;
        }

        // ── Built-in profiles ──────────────────────────────────────

        private static UnitPreferences Build(NotationStyle notation, params (DimensionSignature dim, string symbol)[] entries)
        {
            var p = new UnitPreferences { Notation = notation };
            foreach (var (_, sym) in entries) p.Prefer(sym);
            return p;
        }

        /// <summary>SI base units with Unicode notation (m, kg, s, K, Pa, J, W, ...).</summary>
        public static UnitPreferences SIScientific => Build(NotationStyle.Unicode,
            (DimensionSignature.Length, "m"),
            (DimensionSignature.Mass, "kg"),
            (DimensionSignature.Time, "s"),
            (DimensionSignature.Temperature, "K"),
            (DimensionSignature.PlaneAngle, "rad"),
            (Area.Dim, "m^2"),
            (Volume.Dim, "m^3"),
            (Velocity.Dim, "m/s"),
            (Acceleration.Dim, "m/s^2"),
            (Force.Dim, "N"),
            (Pressure.Dim, "Pa"),
            (Energy.Dim, "J"),
            (Power.Dim, "W"),
            (Density.Dim, "kg/m^3"),
            (MassFlowRate.Dim, "kg/s"),
            (VolumetricFlowRate.Dim, "m^3/s"),
            (Frequency.Dim, "Hz"),
            (AngularVelocity.Dim, "rad/s"));

        /// <summary>SI-flavored but practical units (km, kg, h, °C, kPa, kJ, kW, ...).</summary>
        public static UnitPreferences SIEveryday => Build(NotationStyle.Caret,
            (DimensionSignature.Length, "km"),
            (DimensionSignature.Mass, "kg"),
            (DimensionSignature.Time, "h"),
            (DimensionSignature.Temperature, "degC"),
            (DimensionSignature.PlaneAngle, "deg"),
            (Area.Dim, "m^2"),
            (Volume.Dim, "L"),
            (Velocity.Dim, "km/h"),
            (Force.Dim, "N"),
            (Pressure.Dim, "kPa"),
            (Energy.Dim, "kJ"),
            (Power.Dim, "kW"),
            (Density.Dim, "kg/m^3"),
            (Frequency.Dim, "Hz"));

        /// <summary>US customary units (ft, lb, s, °F, psi, BTU, hp, ...).</summary>
        public static UnitPreferences UsCustomary => Build(NotationStyle.Caret,
            (DimensionSignature.Length, "ft"),
            (DimensionSignature.Mass, "lb"),
            (DimensionSignature.Time, "s"),
            (DimensionSignature.Temperature, "degF"),
            (DimensionSignature.PlaneAngle, "deg"),
            (Area.Dim, "ft^2"),
            (Volume.Dim, "ft^3"),
            (Velocity.Dim, "ft/s"),
            (Acceleration.Dim, "ft/s^2"),
            (Force.Dim, "lbf"),
            (Pressure.Dim, "psi"),
            (Energy.Dim, "BTU"),
            (Power.Dim, "hp"),
            (Density.Dim, "lb/ft^3"),
            (Frequency.Dim, "Hz"));

        /// <summary>US engineering practice (similar to UsCustomary but with absolute-pressure psi-a and lbm for mass).</summary>
        public static UnitPreferences UsEngineering => Build(NotationStyle.Caret,
            (DimensionSignature.Length, "ft"),
            (DimensionSignature.Mass, "lbm"),
            (DimensionSignature.Time, "s"),
            (DimensionSignature.Temperature, "degF"),
            (Force.Dim, "lbf"),
            (Pressure.Dim, "psia"),
            (Energy.Dim, "BTU"),
            (Power.Dim, "BTU/hr"),
            (Density.Dim, "lb/ft^3"));

        /// <summary>Petroleum / natural-gas industry (bbl, MMSCFD-deferred, psia, °F).</summary>
        public static UnitPreferences OilAndGas => Build(NotationStyle.Caret,
            (DimensionSignature.Length, "ft"),
            (DimensionSignature.Mass, "lbm"),
            (DimensionSignature.Time, "h"),
            (DimensionSignature.Temperature, "degF"),
            (Volume.Dim, "bbl"),
            (Force.Dim, "lbf"),
            (Pressure.Dim, "psia"),
            (Energy.Dim, "MMBTU"),
            (Power.Dim, "MMBTU/hr"),
            (Density.Dim, "lb/ft^3"),
            (VolumetricFlowRate.Dim, "bbl/day"));

        /// <summary>Precision-machining context (mm for length, MPa for pressure).</summary>
        public static UnitPreferences Machining => Build(NotationStyle.Caret,
            (DimensionSignature.Length, "mm"),
            (DimensionSignature.Mass, "kg"),
            (DimensionSignature.Time, "s"),
            (DimensionSignature.Temperature, "degC"),
            (Area.Dim, "mm^2"),
            (Force.Dim, "N"),
            (Pressure.Dim, "MPa"),
            (Energy.Dim, "J"),
            (Power.Dim, "kW"));

        // ── Default (thread-local) ─────────────────────────────────

        [ThreadStatic]
        private static UnitPreferences? _default;

        /// <summary>
        /// The ambient preferences used by the <c>"P"</c> format code.
        /// <see cref="ThreadStaticAttribute"/>-scoped so multithreaded hosts (Excel
        /// calc threads, ASP.NET Core requests) get independent values. Default
        /// (when unset) is <see cref="SIScientific"/>.
        /// </summary>
        public static UnitPreferences Default
        {
            get => _default ??= SIScientific;
            set => _default = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
