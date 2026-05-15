# EngineeringUnits

A .NET class library for representing physical quantities (length, mass, time, pressure, volumetric flow rate, etc.) as first-class typed values, with units carried explicitly and preserved through arithmetic.

**Status**: pre-prototype. Specification only — no implementation yet.

## Why

Engineering calculations regularly mix units across the same dimension (`5 ft + 6 in`), produce derived units through multiplication (`55 lbf · 5 ft / 0.5 s = 1 hp`), and use industry-specific composites (`MMSCFD`, `BTU/(hr·ft²·°F)`). Tracking these by hand is error-prone. This library makes the units part of the type, so the compiler and runtime carry the bookkeeping for you.

## Design

- `EngineeringUnit` is the abstract base. Concrete subclasses (`Length`, `Mass`, `Time`, `Pressure`, `Power`, etc.) represent quantities of a specific dimension.
- Operator overloads carry units through arithmetic — `Force * Length → Torque`, `Length / Time → Velocity`, `Velocity / Velocity → DimensionlessQuantity`.
- Mixed-unit addition uses the left operand's display unit: `5 ft + 6 in = 5.5 ft`.
- A configurable `UnitPreferences` map (with named profiles: `SIScientific`, `UsCustomary`, `OilAndGas`, `Machining`, etc.) controls preferred display units per dimension.
- Industry-specific aliases (`MMSCFD`, `bbl`, `MMBTU`, `cP`, `cSt`) are first-class catalog entries. `MMSCFD` defaults to 14.73 psia / 60 °F (interstate-pipeline standard per GPSA / AGA Report No. 3); explicit alternates available.
- Designed to be consumed by downstream integration libraries (ExcelDNA UDFs, ASP.NET Core, gRPC, …) that live in their own repositories.

## Documentation

The full specification — design rationale, locked decisions, open questions — is in [specification.md](specification.md).

## License

MIT — see [LICENSE](LICENSE). Downstream integration assemblies (Excel UDFs etc.) are out of scope of this repo and may carry their own licenses.
