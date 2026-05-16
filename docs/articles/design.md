# Design Overview

## Class hierarchy

```
EngineeringUnit (abstract base)
├── DimensionlessQuantity            — pure numbers, optional Reynolds/Mach/etc. tag
├── Length, Mass, Time, Temperature  — 4 of 7 SI base dimensions
├── ElectricCurrent, PlaneAngle      — base + pseudo-base
├── TemperatureDelta                 — differences, linear-only conversion
├── Area, Volume                     — geometric
├── Velocity, Acceleration, AngularVelocity, Frequency  — kinematic
├── Force, Pressure, Energy, Torque, Power              — dynamic
├── Density, Momentum, MassFlowRate, VolumetricFlowRate — flow
├── DynamicViscosity, KinematicViscosity                — viscosity
├── ThermalConductivity, HeatCapacity, HeatFluxDensity  — thermal
├── SpecificHeatCapacity, SpecificEnergy                — per-mass
├── SpecificVolume, AreaDensity, EnergyDensity, SurfaceTension
├── Voltage, ElectricCharge, ElectricResistance         — electrical
└── ElectricalConductance, ElectricCapacitance, Inductance
DerivedUnit                          — fallback for arbitrary dimension combinations
```

## Internal representation

Every quantity stores:

1. **Canonical value** — magnitude in SI base units (a `double`).
2. **Display unit** — the `Unit` struct the user provided or last set as preferred.
3. **Precision** — significant figures for display (`byte`, default 4).
4. **Dimension signature** — 8-slot `[L, M, T, I, Θ, N, J, A]` of `sbyte` exponents.

The canonical SI representation makes equality, comparison, and arithmetic trivial. Display unit preserves the left-operand-wins convention without adding state.

## Left-operand-wins for mixed units

When two quantities of the same dimension but different units are added or subtracted, the result takes the **left operand's** display unit:

```csharp
new Length(5, "ft") + new Length(6, "in")    // 5.5 ft
new Length(6, "in") + new Length(5, "ft")    // 66 in
```

The choice is predictable and matches `pint`/`boost::units`. Multiplication produces a derived unit via dimension addition; division via subtraction.

## Torque vs Energy disambiguation

Both `Torque` and `Energy` have the fundamental dimension M·L²·T⁻². They're distinct subclasses chosen by operand order:

```csharp
new Force(10, "N")  * new Length(2, "m")   // Torque   (force-first: N·m, lbf·ft)
new Length(2, "m") * new Force(10, "N")   // Energy   (length-first: J, ft·lbf)
```

Explicit conversions (`AsTorque()`, `AsEnergy()`) reinterpret without changing the canonical value when the user knows what they meant. Parsing follows the same convention: `"5 ft*lbf"` → Energy, `"5 lbf*ft"` → Torque.

## Angle as a 9th pseudo-dimension

Radians are dimensionless in standard SI but are tracked as a separate slot here so:

- **Frequency** (`Hz` = 1/s, no angle) and **AngularVelocity** (`rad/s`, has angle) stay distinguishable
- **Linear power** (Force × Velocity, no angle) and **Rotational power** (Torque × AngularVelocity, has angle) — both watts numerically, but separable through the angle slot

The angle slot has no effect on simple kinematic quantities — radians cancel out in standard derivations. Its purpose is to prevent accidental conflation when an operation should yield a different quantity.

## Industry-specific units

Some units don't fit the SI prefix pattern cleanly. The catalog registers them as first-class entries:

- **MMSCFD** defaults to interstate-pipeline conditions (14.73 psia / 60 °F per GPSA / AGA Report No. 3). Named alternates `MMSCFD_petro` (14.696 psia / 60 °F) and `MMSCFD_iupac` (100 kPa / 0 °C) cover other industries.
- **bbl** (petroleum barrel = 42 US gallons), **MMBTU**, **cP**, **cSt**, **Da**.

## Open-core licensing

The core `EngineeringUnits` library is **MIT-licensed**. Specific integration assemblies (ExcelDNA UDFs, ASP.NET Core helpers) live in **separate repositories** and may carry their own licenses while consuming this library as a NuGet dependency. The MIT license explicitly permits closed downstream consumers.
