# EngineeringUnits

A .NET class library for representing physical quantities (length, mass, time, pressure, volumetric flow rate, etc.) as first-class typed values, with units carried explicitly and preserved through arithmetic.

**Status**: 0.1.0-alpha — Phases 1–4 of the specification implemented (Phase 5 in progress).

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Why

Engineering calculations regularly mix units across the same dimension (`5 ft + 6 in`), produce derived units through multiplication (`55 lbf · 5 ft / 0.5 s = 1 hp`), and use industry-specific composites (`MMSCFD`, `BTU/(hr·ft²·°F)`). Tracking these by hand is error-prone. This library makes the units part of the type, so the compiler and runtime carry the bookkeeping.

## Quick start

```csharp
using EngineeringUnits;

// Same-dimension arithmetic with mixed units
var totalLength = new Length(5, "ft") + new Length(6, "in");
Console.WriteLine(totalLength);              // "5.500 ft" — left operand's unit wins

// Cross-type arithmetic propagates dimensions automatically
var area     = new Length(3, "m") * new Length(4, "m");        // Area
var velocity = new Length(100, "m") / new Time(10, "s");       // Velocity
var force    = new Mass(10, "kg") * new Acceleration(9.81, "m/s^2");  // Force (N)
var pressure = force / new Area(0.1, "m^2");                   // Pressure (Pa)
var energy   = new Length(2, "m") * new Force(10, "N");        // Energy (J)  — length-first
var torque   = new Force(10, "N") * new Length(2, "m");        // Torque (N·m) — force-first
var power    = energy / new Time(1, "s");                      // Power (W)

// Convert between units
var ft = new Length(1, "m");
Console.WriteLine(ft.As("ft"));              // 3.28084
Console.WriteLine(ft.ToString("U"));         // US-customary form: "3.281 ft"
Console.WriteLine(ft.ToString("D"));         // Dual: "1.000 m (3.281 ft)"

// Industry-specific units (oil & gas)
var flow = new VolumetricFlowRate(10, "MMSCFD");  // interstate-pipeline standard (14.73 psia / 60 °F)
Console.WriteLine(flow.As("m^3/s"));          // 3.277...

// Preferred units (per-dimension, with named profiles)
UnitPreferences.Default = UnitPreferences.OilAndGas;
Console.WriteLine(new Pressure(1, "atm").ToString("P"));   // displays in psia
Console.WriteLine(new Volume(1, "m^3").ToString("P"));     // displays in bbl

// Dimensionless quantities with interpretation hints
var ratio = new Velocity(680, "m/s") / new Velocity(340, "m/s");
ratio.PreferredInterpretation = "Mach";
Console.WriteLine(ratio);                    // "2.000 Ma"
```

## Design

- **Class hierarchy.** `EngineeringUnit` is the abstract base. Concrete subclasses (`Length`, `Mass`, `Time`, `Temperature`, `Area`, `Volume`, `Velocity`, `Acceleration`, `Force`, `Pressure`, `Energy`, `Torque`, `Power`, `Density`, `MassFlowRate`, `VolumetricFlowRate`, `Frequency`, `AngularVelocity`, `PlaneAngle`, `DimensionlessQuantity`) represent specific dimensions.
- **Strongly-typed operators.** `Length + Length → Length` compiles; `Length + Time` does not. Cross-type products and quotients propagate dimensions: `Mass * Acceleration → Force`, `Length / Time → Velocity`, etc.
- **Left-operand-wins for mixed-unit addition.** `5 ft + 6 in` yields `5.5 ft`, not `66 in` and not "some canonical SI thing".
- **Internal canonical SI.** Every quantity is stored in SI base units internally; display unit is kept alongside for formatting and result-unit selection.
- **Industry-specific aliases.** `MMSCFD` defaults to 14.73 psia / 60 °F (interstate-pipeline standard per GPSA / AGA Report No. 3); `MMSCFD_petro` and `MMSCFD_iupac` cover other standards.
- **Torque vs Energy disambiguation.** Same fundamental dimension, different physical meaning. `Force * Length → Torque` (force-first display: `N·m`, `lbf·ft`); `Length * Force → Energy` (length-first display: `J`, `ft·lbf`).
- **Angle as 9th pseudo-dimension.** Radians tracked so frequency (`Hz`) and angular velocity (`rad/s`) stay distinguishable through arithmetic.

## Units catalog

Currently registered (a sample — see `UnitCatalog.cs` for the full list):

| Dimension | Units |
|---|---|
| Length | m, km, cm, mm, μm/um, nm, Mm, in, ft, yd, mi, nmi, fathom, furlong |
| Mass | kg, g, mg, t, lb, lbm, oz, slug, ton, lt, Da, u |
| Time | s, ms, μs/us, ns, min, h, hr, day, week, year, yr |
| Temperature | K, °C/degC, °F/degF, °R/degR |
| Angle | rad, deg/°, rev, grad |
| Area | m², cm², mm², km², ha, in², ft², yd², mi², acre |
| Volume | m³, cm³, mm³, L, mL, μL, in³, ft³, yd³, gal, gal_imp, qt, pt, cup, floz, bbl |
| Velocity | m/s, cm/s, mm/s, km/h, kph, ft/s, ft/min, mph, knot |
| Acceleration | m/s², ft/s², g_n, Gal |
| Force | N, kN, MN, mN, lbf, kgf, dyn, ozf |
| Pressure | Pa, kPa, MPa, GPa, hPa, mPa, bar, mbar, atm, psi, psia, psig, ksi, Torr, mmHg, inHg, inH2O |
| Energy | J, kJ, MJ, GJ, mJ, Wh, kWh, MWh, BTU, kBTU, MMBTU, cal, kcal, erg, ft*lbf |
| Power | W, kW, MW, GW, mW, hp, hp_e, hp_m, BTU/hr, BTU/s, ft*lbf/s, MMBTU/hr |
| Torque | N*m, lbf*ft, lbf*in, kgf*m |
| Density | kg/m³, g/cm³, g/mL, kg/L, lb/ft³, lb/in³, lb/gal |
| Mass flow rate | kg/s, kg/min, kg/h, g/s, lb/s, lb/min, lb/hr, t/h |
| Volumetric flow rate | m³/s, m³/h, L/s, L/min, L/h, ft³/s, cfs, ft³/min, cfm, gpm, bbl/day, bpd, MMSCFD, MMSCFD_petro, MMSCFD_iupac, MSCFD, SCFD, SCFM |
| Frequency | Hz, kHz, MHz, GHz, 1/s, 1/min |
| Angular velocity | rad/s, rad/min, deg/s, rpm, rps |

## Specification

The full design specification — locked decisions, open questions, rationale — is in [specification.md](specification.md).

## Testing

```bash
dotnet test
```

Currently 227 tests across:
- Unit tests for every subclass
- Property-based tests (FsCheck) for arithmetic invariants
- Conversion-correctness tests driven by CSV reference data (NIST SP 811)
- Cross-type operator tests
- Format code tests
- Torque/Energy disambiguation tests
- Industry unit tests (MMSCFD variants, Dalton, SCFM)

## Target frameworks

- Core library: **.NET Standard 2.0** (consumable from .NET Framework 4.7.2+ and modern .NET 6+)
- Tests: .NET 9.0

## License

MIT — see [LICENSE](LICENSE). Downstream integration assemblies (Excel UDFs etc.) are out of scope of this repo and may carry their own licenses.
