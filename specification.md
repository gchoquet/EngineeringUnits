# Engineering Units Conversion and Calculation Library — Specification

**Author**: Gary Choquette
**Initial draft**: May 11, 2026
**Last updated**: May 15, 2026
**Status**: Draft for review

---

## 1. Overview

A .NET class library for representing physical quantities (length, mass, time, derived quantities such as velocity, force, volumetric flow rate, etc.) as first-class typed values, with units carried explicitly and preserved through arithmetic.

The library is intended to eliminate a recurring class of engineering-calculation bugs caused by ad-hoc unit handling: developers tracking units in variable names, manually inserting conversion factors, forgetting kerf-equivalent conversions when crossing US-customary / SI boundaries, or producing "standardized" formulas that silently assume one of several plausible base units.

Primary consumption contexts (priority order):

1. **Excel via ExcelDNA** — exposing the library as Excel UDFs so analysts can write `=EU.Convert(A1, "m")` instead of remembering conversion factors. This is the immediate target and drives several decisions (Section 11).
2. **Server-side .NET 8 web apps** — engineering calculators served via ASP.NET Core, including REST endpoints that accept value+unit pairs.
3. **Direct library consumption** — referenced from any .NET project that wants typed quantities.

Conceptually adjacent to F#'s units of measure, Python's `pint`, and C++'s `boost::units`, but with a class-hierarchy design rather than compile-time generic templates, chosen for compatibility with VBA-origin code and ExcelDNA's reflection-based UDF exposure.

---

## 2. Goals and non-goals

### Goals

- **Type-safe arithmetic for quantities of the same dimension.** Length + Length compiles. Length + Time does not (or throws at runtime when going through the base class).
- **Automatic unit propagation through multiplication and division.** Length × Length → Area. Mass × Acceleration → Force. Volume ÷ Time → VolumetricFlowRate.
- **Mixed-unit arithmetic within a dimension.** 5 ft + 6 in → 5.5 ft (left operand's unit governs the result).
- **Parsing of unit strings** in common engineering notation (`ft/s`, `kg*m/s^2`, `lb-ft`, `MMSCFD`).
- **Configurable display** — preferred unit, significant figures, short vs long unit names, US-customary-and-SI dual display for reports.
- **First-class support for industry-specific composite units** that aren't pure SI derivatives (MMSCFD, BTU/hr/ft², stokes, etc.).
- **Dimensionless quantities** — Reynolds, Mach, Froude, Prandtl, etc., recognized when arithmetic results in a dimensionless ratio with a known physical interpretation.
- **Approachable XML documentation** for every public member, suitable for DocFX or Sandcastle help generation.
- **Round-trippable serialization** — a quantity persists to a single string (e.g. `"5.5 ft"`) that fully reconstructs the value, unit, and precision metadata.

### Non-goals (v1)

- **Compile-time dimensional checking via generics.** F#-style `Length<Meter>` is more rigorous but incompatible with VBA conversion patterns and ExcelDNA's `object`-typed UDF signatures.
- **Uncertainty propagation.** Significant figures govern display only, not error propagation. A future v2 may add `±` propagation.
- **Symbolic computation.** Values are concrete `double` quantities, not symbolic expressions.
- **Currency or non-physical units.** Out of scope.
- **Calendar/date arithmetic.** Use `DateTime` / `TimeSpan` for clock time; the library's `Time` quantity is for durations in physics contexts.

---

## 3. Use cases

### 3.1 Excel UDF — basic conversion

```
=EU.Convert("5 ft", "m")        → 1.524
=EU.Convert(A1, "psi")          → if A1 = "2 bar", result = 29.0075
=EU.Add("5 ft", "6 in")         → "5.5 ft"
=EU.Subtract("5 ft", "0.5 m")   → "3.36 ft"  (preserves left operand's unit)
=EU.Multiply("55 lbf", "5 ft")  → "275 ft*lbf"
```

### 3.2 C# — direct library use

```csharp
using EngineeringUnits;

var distance = new Length(5, "ft");
var time     = new Time(2, "s");
var velocity = distance / time;            // Velocity in ft/s
Console.WriteLine(velocity);               // "2.5 ft/s"
Console.WriteLine(velocity.As("m/s"));     // "0.762 m/s"

var totalDistance = new Length(5, "ft") + new Length(6, "in");
Console.WriteLine(totalDistance);          // "5.5 ft" — left operand's unit governs

// Mismatched dimensions
var bad = distance + time;                 // Compile error (different subclasses)
EngineeringUnit a = distance, b = time;
var bad2 = a + b;                          // DimensionMismatchException at runtime
```

### 3.3 C# — dimensionless results

```csharp
var v1 = new Velocity(340, "m/s");
var v2 = new Velocity(680, "m/s");
var ratio = v2 / v1;                       // DimensionlessQuantity, value 2.0
Console.WriteLine(ratio);                  // "2.0"  (no unit, since dimensionless)

// User preference: interpret velocity ratios as Mach number
ratio.PreferredInterpretation = "Mach";
Console.WriteLine(ratio);                  // "2.0 Ma"
```

### 3.4 C# — dual-unit display for reports

```csharp
var dia = new Length(0.5, "m");
Console.WriteLine(dia.ToDualString());     // "0.5 m (1.640 ft)"
Console.WriteLine(dia.ToDualString(format: "[SI] ([US])"));
                                           // "[0.5 m] ([1.640 ft])"
```

### 3.5 ASP.NET Core — REST input

```csharp
[HttpPost("calculate")]
public IActionResult Calculate([FromBody] CalcRequest req)
{
    var pressure = Pressure.Parse(req.Pressure);     // "150 psi"
    var area     = Area.Parse(req.Area);             // "2 in^2"
    var force    = pressure * area;                  // Force in psi*in^2 → lbf
    return Ok(new { force = force.ToString() });
}
```

---

## 4. Architecture

### 4.1 Class hierarchy

```
EngineeringUnit (abstract)
├── DimensionlessQuantity        Mach, Reynolds, Froude, Prandtl, etc.
│
├── Length                       m, ft, in, mi, nmi, furlong, ...
├── Mass                         kg, lb, slug, oz, t, ...
├── Time                         s, min, h, day, year, ...
├── ElectricCurrent              A, mA, kA, ...
├── Temperature                  K, °C, °F, °R, ...
├── AmountOfSubstance            mol, kmol, lbmol, ...
├── LuminousIntensity            cd, lm/sr, ...
├── PlaneAngle                   rad, deg, rev, grad, ...
│
├── Area                         m^2, ft^2, acre, ha, ...
├── Volume                       m^3, ft^3, gal, L, bbl, ...
├── Velocity                     m/s, ft/s, mph, knot, ...
├── Acceleration                 m/s^2, ft/s^2, g_n, ...
├── Force                        N, lbf, kgf, dyne, ...
├── Pressure                     Pa, psi, bar, atm, inHg, ...
├── Energy                       J, BTU, ft*lbf, cal, kWh, ... — length-first display
├── Torque                       N*m, lbf*ft, ... — force-first display (same dimension as Energy)
├── Power                        W, hp, BTU/hr, ft*lbf/s, ...
├── AngularVelocity              rad/s, rpm, deg/s, ...
├── Frequency                    Hz, 1/s, ...
├── VolumetricFlowRate           m^3/s, gpm, cfm, MMSCFD, ...
├── MassFlowRate                 kg/s, lb/s, lb/hr, ...
├── Density                      kg/m^3, lb/ft^3, lb/gal, ...
├── DynamicViscosity             Pa*s, cP, lb/(ft*s), ...
├── KinematicViscosity           m^2/s, St, cSt, ft^2/s, ...
└── ... (and other derived types as the catalog grows)

DerivedUnit                       (fallback when no subclass matches)
```

`EngineeringUnit` is the abstract base. Concrete subclasses represent quantities of a single dimension. `DerivedUnit` is the runtime escape hatch when multiplication/division produces a combination not represented by any subclass (e.g. `length * mass / time^3` — unusual enough to not warrant a class but should still work).

Every subclass is **immutable** (a value type semantically, though implemented as a sealed class for inheritance). Arithmetic always returns a new instance.

### 4.2 Internal representation

Every quantity stores:

1. **Canonical value** — the magnitude expressed in SI base units (a `double`).
2. **Display unit** — the unit the user originally provided or last set as preferred (a `Unit` struct carrying short name, long name, conversion factor to/from canonical, dimension signature).
3. **Precision** — significant figures for display (a `byte`, default 4).
4. **Dimension signature** — a struct `[L, M, T, I, Θ, N, J, Angle]` of `sbyte` exponents identifying the quantity's dimension. Used by `DerivedUnit` and by the operator-overload subtype resolver.

The canonical SI internal representation is critical: it makes addition/subtraction across mixed units trivial (convert both to canonical, add, then convert back to the left operand's display unit), and makes equality / comparison meaningful regardless of how the operands were spelled.

Conversions are **affine** for temperature (degF, degC, K, R require offset + scale) and **linear** for all others (scale only). The `Unit` struct carries both `Scale` and `Offset` fields, but `Offset` is non-zero only for absolute temperature scales.

### 4.3 Unit catalog

Units are defined in a static catalog, keyed by short name. Each entry knows its dimension, its scale and offset to SI base, its long name, and aliases. The catalog is initialized at module load and frozen thereafter.

Catalog initialization happens via a static class `UnitCatalog` that contains:

```csharp
internal static class UnitCatalog
{
    public static readonly Dictionary<string, Unit> Units = new(StringComparer.Ordinal);

    static UnitCatalog()
    {
        // Length
        Add("m",  "meter",       Dimension.Length,  scale: 1.0);
        Add("ft", "foot",        Dimension.Length,  scale: 0.3048);
        Add("in", "inch",        Dimension.Length,  scale: 0.0254);
        Add("mi", "mile",        Dimension.Length,  scale: 1609.344);
        // ...
    }
}
```

Lookup is **case-sensitive by default** to avoid `M` (mega prefix) / `m` (meter) ambiguity, but a case-insensitive `TryParseLoose` is offered for end-user input. SI prefixes (k, M, G, m, μ, n, ...) are programmatically combined with the base units rather than enumerated.

---

## 5. Unit notation grammar

The library accepts unit strings using a simple grammar designed to be both human-readable and unambiguous to parse.

### 5.1 Grammar

```
UnitExpr   = Term (('*' | ' ' | '/') Term)*
Term       = UnitName Exponent?
Exponent   = ('^' Number) | DigitSuffix
DigitSuffix = unsigned integer immediately following UnitName (e.g. "ft3")
UnitName   = letters and optional SI prefix
Number     = optionally signed integer (exponent)
```

### 5.2 Examples

| Notation | Meaning |
|---|---|
| `m` | meter |
| `m/s` | meter per second |
| `m/s^2` | meter per second squared |
| `m^2` | square meter |
| `m2` | square meter (shorthand) |
| `ft*lbf` | foot-pound (force) — torque or energy |
| `ft lbf` | same as above (space as multiplication) |
| `kg*m/s^2` | newton, expanded |
| `lb/(ft*s)` | parentheses for grouping |
| `1/s` | per second (frequency) |
| `MMSCFD` | million standard cubic feet per day (single-token composite, special-cased) |

### 5.3 Notation choice rationale

The docx draft used `ft-s-1` (negative exponents trailing). The library accepts that form as an alternate parse (`ft-s-1` interpreted as `ft·s⁻¹` = `ft/s`) for backward compatibility with the existing VBA conventions, but the primary notation uses `*`, `/`, `^` because:

- More universally recognized in print and online sources
- Easier to disambiguate in regex parsing
- Matches the notation in `pint`, `boost::units`, ISO 80000

Both forms round-trip: `Length.Parse("ft-s-1").ToString()` produces `"ft/s"`.

### 5.4 Industry-specific composite units

Some industry-standard units are not cleanly expressible as products of SI bases. They are registered as single-token aliases mapped to canonical expressions:

| Token | Canonical expansion | Domain |
|---|---|---|
| `MMSCFD` | `1.0e6 * ft^3 / day` at **14.73 psia, 60 °F** (interstate pipeline default) | Natural gas |
| `MMSCFD_petro` | same volume basis but at **14.696 psia, 60 °F** | Petroleum / refining |
| `MMSCFD_iupac` | same volume basis but at **100 kPa, 0 °C** | Chemistry / IUPAC |
| `MMSCFD@<p>` | parametric override, e.g. `MMSCFD@14.65` | One-off |
| `MMBTU` | `1.0e6 * BTU` | Gas heating value |
| `bbl` | 42 US gallons | Petroleum |
| `Da` (Dalton) | mass unit, 1.66053907e-27 kg | Chemistry |
| `cP` | mPa*s | Viscosity |
| `cSt` | mm^2/s | Kinematic viscosity |

These behave as ordinary units after parsing — `5 MMSCFD + 2 MMSCFD = 7 MMSCFD`, but `5 MMSCFD + 0.1 m^3/s` is allowed and produces a result in the left-operand's unit (MMSCFD).

**MMSCFD note**: standard conditions vary by industry. The library defaults `MMSCFD` to 14.73 psia / 60 °F (GPSA Engineering Data Book; AGA Report No. 3 measurement basis used on US interstate pipelines). Users in petroleum/refining contexts (14.696 psia base) or non-US contexts should use the explicit `MMSCFD_petro` / `MMSCFD_iupac` aliases or the parametric `MMSCFD@<p>` form. Mixing MMSCFD variants is treated like mixing any two compatible units — both convert to canonical m³/s and back to the left operand's unit, with no warning about the silent condition conversion. (A future v2 may add a strict-mode option that requires explicit acknowledgment when mixing volumetric flow units with different reference conditions.)

---

## 6. Arithmetic semantics

### 6.1 Addition and subtraction

- **Same subclass, same display unit**: arithmetic on canonical values; result keeps the display unit. Trivial.
- **Same subclass, different display units** (e.g. `Length` in `ft` + `Length` in `m`): both convert to canonical SI, add, then express the result in **the left operand's display unit**. The left-operand-wins rule is taken directly from the docx draft.
- **Different subclasses**: compile error.
- **Going through `EngineeringUnit` base** with different dimensions: `DimensionMismatchException` at runtime.

Precision metadata in the result is the **minimum** of the two operands' precision (the least-precise input governs).

### 6.2 Multiplication and division

The result's dimension is determined by adding (multiplication) or subtracting (division) the exponent vectors of the operands. The runtime then attempts to resolve a concrete subclass:

1. If the resulting dimension matches a registered subclass (e.g. `Length × Length` → `Area`), return an instance of that subclass.
2. If the resulting dimension is **all zeros** (every exponent zero), return a `DimensionlessQuantity`.
3. Otherwise, return a `DerivedUnit` carrying the dimension signature and a generated display unit (e.g. `kg*m^2/s^3`).

Subclass resolution is performed by a static registry mapping `DimensionSignature → System.Type`. The registry is populated at module load by reflecting over all `EngineeringUnit` subclasses and reading a `[DimensionAttribute]` declared on each.

### 6.3 Powers and roots

```csharp
var area   = length.Pow(2);    // Length → Area
var side   = area.Pow(0.5);    // Area → Length (square root)
var volume = length.Pow(3);    // Length → Volume
```

`Pow` accepts a `double` exponent. Non-integer exponents are supported as long as the resulting dimension exponents remain integral (typical case: square roots and cube roots).

### 6.4 Operator overloading

`EngineeringUnit` declares operator overloads for `+`, `-`, `*`, `/`, `==`, `!=`, `<`, `>`, `<=`, `>=`. Subclasses inherit them but also declare strongly-typed overloads when sensible (e.g. `Length * Length → Area` is declared explicitly so the compiler chooses the strong return type).

For mixed-subclass multiplication where the result *should* be a known subclass (e.g. `Mass * Acceleration → Force`), the strong-typed overload is declared on the **left** operand's type by convention. Where no strong overload exists, the operation falls through to the `EngineeringUnit`-base overload and returns `DerivedUnit`.

---

## 7. Dimensionless quantities

When an arithmetic operation produces a quantity with all-zero dimensions, the result is a `DimensionlessQuantity`. Examples: velocity ratio (`v1 / v2`), Reynolds number (`ρ·v·L / μ`), strain.

`DimensionlessQuantity` has an optional **interpretation hint** identifying its physical meaning:

```csharp
public sealed class DimensionlessQuantity : EngineeringUnit
{
    /// <summary>Optional physical interpretation, e.g. "Reynolds", "Mach".</summary>
    public string? PreferredInterpretation { get; set; }
}
```

Default display is just the numeric value. With an interpretation set, the display includes the conventional symbol (`Re`, `Ma`, `Fr`, `Pr`, etc.).

The interpretation is **never inferred automatically** — many velocity ratios are not Mach numbers, many length ratios are not strain. The library refuses to guess; the user opts in.

Built-in interpretations recognized for display purposes only:

| Symbol | Name | Typical formula |
|---|---|---|
| Re | Reynolds | ρvL/μ |
| Ma | Mach | v/c |
| Fr | Froude | v/√(gL) |
| Pr | Prandtl | cpμ/k |
| Nu | Nusselt | hL/k |
| Bi | Biot | hL/k_solid |
| Pe | Péclet | Re·Pr |
| Eu | Euler | Δp/(ρv²) |
| Ec | Eckert | v²/(cpΔT) |
| Ja | Jakob | cpΔT/h_lv |

---

## 8. Precision and formatting

### 8.1 Significant-figures model

Every quantity carries a `Precision` (significant figures, default 4). Arithmetic results take the **minimum** precision of their inputs. The `Precision` is for display only — internal arithmetic is full `double` precision.

```csharp
var a = new Length(5.0, "ft") { Precision = 2 };   // displays "5.0 ft"
var b = new Length(6.0, "in") { Precision = 4 };
var c = a + b;                                      // Precision = 2
Console.WriteLine(c);                              // "5.5 ft" (2 sig figs)
```

### 8.2 ToString and format strings

```csharp
public override string ToString();                          // default: "5.5 ft"
public string ToString(string format);                      // custom
public string ToString(IFormatProvider? provider);          // culture-aware
public string ToString(string format, IFormatProvider? p);
```

Format string codes:

| Code | Effect |
|---|---|
| `G` | General (default) — sig figs, short unit name |
| `L` | Long unit name — `5.5 feet` |
| `D` | Dual display — `5.5 ft (1.676 m)` |
| `S` | SI canonical — `1.676 m` |
| `U` | US-customary canonical — `5.5 ft` |
| `P` | Preferred — consults `DefaultPreferences.GetPreferred(dimension)` (see Decision 13.17) |
| `E` | Engineering scientific — `5.500e0 ft` |
| `N{n}` | Force `n` significant figures — `N6` → `5.50000 ft` |
| `{unit}` | Force a specific unit — `{m}` → `1.676 m` |

`{unit}` may be combined with `L`: `L{m}` → `1.676 meters`.

### 8.3 Culture awareness

Decimal separator follows the supplied `IFormatProvider` (or `CultureInfo.CurrentCulture` if none). Unit names follow `CultureInfo.CurrentUICulture` for long format (a localized `feet` / `pied` / `Fuß` resource table is added in v1.1).

---

## 9. Public API surface

A representative slice. Full XML doc comments shown on the most important members; abbreviated elsewhere.

### 9.1 EngineeringUnit (abstract base)

```csharp
/// <summary>
/// Abstract base class for all engineering quantities. Carries a value
/// expressed in canonical SI base units, a display unit, a dimension
/// signature, and display-precision metadata.
/// </summary>
/// <remarks>
/// Subclasses represent specific physical dimensions (Length, Mass, Time,
/// and derived quantities such as Velocity, Force, Pressure). Arithmetic
/// operators are overloaded on the base class; strongly-typed overloads
/// on subclasses preserve compile-time type safety where applicable.
///
/// Instances are immutable; arithmetic returns new instances.
/// </remarks>
public abstract class EngineeringUnit : IFormattable, IComparable<EngineeringUnit>, IEquatable<EngineeringUnit>
{
    /// <summary>The value expressed in canonical SI base units.</summary>
    public double CanonicalValue { get; }

    /// <summary>The unit used for display and as the result-unit when this is the left operand of arithmetic.</summary>
    public Unit DisplayUnit { get; }

    /// <summary>The dimension signature, identifying this quantity's physical dimension.</summary>
    public DimensionSignature Dimension { get; }

    /// <summary>Significant figures used for formatted display. Default is 4.</summary>
    public byte Precision { get; init; } = 4;

    /// <summary>Returns the value expressed in the given unit.</summary>
    /// <param name="unit">Target unit, e.g. "ft", "m/s", "psi".</param>
    /// <returns>The numeric value in the target unit.</returns>
    /// <exception cref="DimensionMismatchException">If <paramref name="unit"/> is not of the same dimension.</exception>
    public double As(string unit);

    /// <summary>Returns a new instance carrying the same value but a different display unit.</summary>
    public EngineeringUnit In(string unit);

    public static EngineeringUnit operator +(EngineeringUnit a, EngineeringUnit b);
    public static EngineeringUnit operator -(EngineeringUnit a, EngineeringUnit b);
    public static EngineeringUnit operator *(EngineeringUnit a, EngineeringUnit b);
    public static EngineeringUnit operator /(EngineeringUnit a, EngineeringUnit b);
    public static EngineeringUnit operator *(EngineeringUnit a, double scalar);
    public static EngineeringUnit operator /(EngineeringUnit a, double scalar);
    public static EngineeringUnit operator -(EngineeringUnit a);

    public EngineeringUnit Pow(double exponent);
    public EngineeringUnit Abs();

    public override string ToString();
    public string ToString(string format);
    public string ToString(string format, IFormatProvider? provider);
    public string ToDualString();
}
```

### 9.2 Concrete subclass example

```csharp
/// <summary>A length quantity.</summary>
/// <remarks>
/// SI base unit: meter. Supported units include m, cm, mm, μm, nm, km,
/// in, ft, yd, mi, nmi, furlong, fathom, league, and SI-prefixed variants.
/// </remarks>
[Dimension(L: 1)]
public sealed class Length : EngineeringUnit
{
    /// <summary>Creates a length quantity.</summary>
    /// <param name="value">The numeric value.</param>
    /// <param name="unit">The unit symbol, e.g. "ft".</param>
    /// <exception cref="UnknownUnitException">If <paramref name="unit"/> is not a recognized length unit.</exception>
    public Length(double value, string unit);

    /// <summary>Parses a length from a string like "5 ft" or "1.5 m".</summary>
    public static Length Parse(string s);
    public static bool TryParse(string s, out Length? result);

    public static Length operator +(Length a, Length b);
    public static Length operator -(Length a, Length b);
    public static Area   operator *(Length a, Length b);
    public static Volume operator *(Length a, Area b);
    public static Velocity operator /(Length a, Time b);
    public static Length operator *(Length a, double scalar);
    public static Length operator *(double scalar, Length a);
}
```

### 9.3 DerivedUnit (fallback)

```csharp
/// <summary>
/// A quantity whose dimension has no registered subclass. Created
/// automatically when arithmetic produces an unusual combination, or
/// explicitly by parsing a complex unit string.
/// </summary>
public sealed class DerivedUnit : EngineeringUnit
{
    public DerivedUnit(double value, string unit);

    public static DerivedUnit Parse(string s);

    /// <summary>If the dimension matches a known subclass, returns an instance of that subclass. Otherwise returns this.</summary>
    public EngineeringUnit Simplify();
}
```

### 9.4 Exceptions

```csharp
public class EngineeringUnitException : Exception { ... }
public sealed class DimensionMismatchException : EngineeringUnitException { ... }
public sealed class UnknownUnitException : EngineeringUnitException { ... }
public sealed class UnitParseException : EngineeringUnitException { ... }
```

---

## 10. Error handling

| Situation | Behavior |
|---|---|
| Add two quantities of different dimensions | `DimensionMismatchException` (or compile error if statically typed) |
| Parse an unknown unit symbol | `UnknownUnitException` with the offending token in the message |
| Malformed unit notation | `UnitParseException` with the position of the parse failure |
| `As("...")` to a wrong-dimension unit | `DimensionMismatchException` |
| Divide by zero on the scalar path | `DivideByZeroException` (standard) — no special handling |
| Overflow / NaN | Propagates as `double.PositiveInfinity` / `NaN`; never throws |
| Negative `Precision` | `ArgumentOutOfRangeException` at the property setter |

`TryParse` variants are provided wherever `Parse` is, returning `bool` and never throwing for malformed input.

---

## 11. Design considerations for downstream consumers

The `EngineeringUnits` library is the **core**. Specific integration assemblies — ExcelDNA UDFs, ASP.NET Core controllers, gRPC adapters — are **separate downstream projects** under their own licenses and lifecycles, each of which depends on `EngineeringUnits` as a NuGet reference.

This section documents the core-library API requirements that those downstream assemblies need. It is **not** a spec of the downstream assemblies themselves — those will get their own specs in their own repositories.

### 11.1 Required API surface for downstream use

To enable downstream integration assemblies (especially the ExcelDNA one, which is the most demanding), the core library MUST expose:

- **Value-and-unit-separable construction**: `EngineeringUnit.Create(double value, string unit)` factory, and instance properties `Value` (in display unit) and `DisplayUnit` (with `.Symbol` and `.LongName` accessors). Downstream code should never need to round-trip through a combined string just to read value and unit independently.
- **Public dimension query**: `EngineeringUnit.Dimension` returns the `DimensionSignature` struct, including a human-readable `ToString()` (e.g. `"L*M/T^2"` for force).
- **Static parsing helpers**: `EngineeringUnit.TryParse(string, out EngineeringUnit?)` that resolves to the appropriate subclass based on dimension. Downstream code receives untyped strings and shouldn't have to choose the subclass itself.
- **Exception-free hot paths**: a `TryParse` for every `Parse` and for unit-conversion calls. Downstream UDFs cannot afford to catch exceptions per row in a large spreadsheet.
- **No static mutable state**: the unit catalog is built once at module load and frozen. Downstream multi-threaded hosts (Excel calc threads, ASP.NET Core requests) must be able to use the library without locks.
- **Trim-friendly**: avoid reflection in hot paths so AOT / trimming scenarios remain viable.

### 11.2 Example downstream signature (informational only)

What the ExcelDNA assembly might do — sketched here to show the core-library API is sufficient, not to specify the UDF assembly itself:

```csharp
// In EngineeringUnits.ExcelDNA — a SEPARATE assembly, separate license
[ExcelFunction(Name = "EU.Add", Description = "Adds two engineering quantities. Returns {value, unit}.")]
public static object[,] EuAdd(double aValue, string aUnit, double bValue, string bUnit)
{
    if (!EngineeringUnit.TryParse($"{aValue} {aUnit}", out var a) ||
        !EngineeringUnit.TryParse($"{bValue} {bUnit}", out var b))
        return new object[,] { { ExcelError.ExcelErrorValue, "" } };
    var result = a + b;
    return new object[,] { { result.Value, result.DisplayUnit.Symbol } };
}
```

The downstream consumer reads value and unit separately, calls a single arithmetic operator, returns two values. The core library provides everything that needs.

### 11.3 Excel value representation

**Cells store numbers, not annotated strings.** A quantity in a worksheet is represented by **two adjacent cells**:

| A | B |
|---|---|
| `5.5` | `ft` |

The numeric value lives in column A; the unit symbol (a plain string) lives in column B. This convention:

- Keeps Excel's numeric features working — sum, chart, conditional format, etc. all operate on the value cell normally.
- Lets a user override the displayed unit by editing column B alone (the value re-converts automatically through UDFs that read both cells).
- Allows array-formula returns from arithmetic UDFs: `=EU.Add(A1:B1, A2:B2)` spills into two cells, value and unit.

The `EngineeringUnit` class must therefore expose value and unit as **independently settable / readable properties**, decoupled from a single combined string form. Round-trip through a string is still supported (`Parse` / `ToString`) but is not the primary Excel storage form.

UDF signature pattern:

```csharp
[ExcelFunction(Name = "EU.Add", Description = "Adds two engineering quantities. Returns {value, unit}.")]
public static object[,] EuAdd(
    [ExcelArgument(Description = "Value of operand A")]    double aValue,
    [ExcelArgument(Description = "Unit of operand A")]     string aUnit,
    [ExcelArgument(Description = "Value of operand B")]    double bValue,
    [ExcelArgument(Description = "Unit of operand B")]     string bUnit)
{
    var a = EngineeringUnit.Create(aValue, aUnit);
    var b = EngineeringUnit.Create(bValue, bUnit);
    var result = a + b;
    return new object[,] { { result.Value, result.DisplayUnit.Symbol } };
}
```

Users enter `=EU.Add(A1,B1,A2,B2)` selected over a 1×2 range and press Ctrl+Shift+Enter (or just Enter in modern Excel with dynamic arrays). Result spills to value-and-unit pair.

### 11.3 Performance budget (set by the hardest downstream consumer)

The most demanding downstream consumer is ExcelDNA UDFs called over large ranges. Setting the core-library performance budget against that:

- **Catalog initialization**: amortized to zero (module load only).
- **Unit-string parse**: ≤ 1 µs in steady state.
- **Arithmetic between two parsed instances**: ≤ 0.5 µs.
- **Allocation per operation**: ≤ 1 small object (the result). Avoid intermediate boxing.
- **End-to-end goal**: a 100k-row sheet computing `=EU.Add(A1:B1, A2:B2)` recalcs in under a second on a typical workstation.

Achieving this drives several design choices: the `DimensionSignature` is a `struct`, not a class; the subclass dispatcher uses a `Dictionary<DimensionSignature, Func<...>>` lookup (O(1) amortized) rather than reflection; format strings are parsed once and cached.

---

## 12. Implementation phases

Loose ordering, not commitments.

### Phase 1 — Core (MVP)
- `EngineeringUnit` base, `DimensionSignature`, `Unit` struct
- Length, Mass, Time, Temperature subclasses with SI + US customary + common SI-prefixed units
- Addition, subtraction, scalar mul/div, equality, comparison
- `Parse`, `TryParse`, `ToString` (default format)
- Basic exceptions

### Phase 2 — Derived quantities
- Area, Volume, Velocity, Acceleration, Force, Pressure, Energy, Power, MassFlowRate, VolumetricFlowRate, Density
- Cross-type multiplication and division operator overloads
- `DerivedUnit` fallback
- `Pow`, `Abs`

### Phase 3 — Display polish
- Format string codes (`G`, `L`, `D`, `S`, `U`, `E`, `N{n}`, `{unit}`)
- `ToDualString`
- Culture-aware decimal separator
- Long-format unit names

### Phase 4 — ExcelDNA integration
- `EngineeringUnits.ExcelDNA.xll`
- All UDFs from Section 11.2
- Function wizard help text

### Phase 5 — Dimensionless and industry units
- `DimensionlessQuantity` with interpretation hints
- MMSCFD, bbl, cP, cSt, MMBTU, Da and other industry composites
- Reynolds/Mach/Froude/etc. display symbols

### Phase 6 — Documentation and packaging
- XML doc comments on every public member
- DocFX or Sandcastle output → static help site
- NuGet packaging
- Sample workbook with ExcelDNA UDFs in action

### Future
- Localized unit names (resource files per culture)
- Uncertainty propagation (`±` model)
- Visual Studio analyzer warning on suspicious patterns (e.g. unitless `double` passed where a quantity is expected)
- gRPC-friendly proto definitions for quantity serialization

---

## 13. Design decisions

Decisions made deliberately. Revisit any of these only with intent.

### 13.1 Target framework
**Decision**: `.NET Standard 2.0` for the core library; `.NET 8` for the ASP.NET Core sample and the ExcelDNA loader.

- .NET Standard 2.0 is consumable from both .NET Framework 4.7.2+ and modern .NET 6+. ExcelDNA's modern loader supports both runtimes; the library doesn't pick.
- Where modern .NET features would be nice (records, `init`, nullable reference types), we use language version 10+ with `LangVersion` set in the .csproj — the language features compile to .NET Standard 2.0 IL.

### 13.2 Class hierarchy vs. generics
**Decision**: Class hierarchy (subclasses per dimension), not F#-style generics.

- VBA conversion patterns produce flat classes, not generic types
- ExcelDNA UDFs accept `object`; generic types add no value at that boundary
- Operator overloads are simpler to declare and discover on concrete classes
- The cost — `DerivedUnit` exists for arbitrary combinations — is mild

### 13.3 Internal representation
**Decision**: canonical SI value (`double`) + display unit reference + dimension signature.

- Always storing canonical SI makes equality, comparison, and arithmetic trivial
- `double` is the right precision for engineering (15–17 sig figs; sig-fig display caps below that)
- Carrying the display unit lets arithmetic preserve the left-operand convention

### 13.4 Mixed-unit addition rule
**Decision**: result uses the left operand's display unit.

- Taken from the docx draft
- Convention matches `pint` and `boost::units`
- Predictable for the user; no "smart" guess required

### 13.5 Notation grammar
**Decision**: `*`, `/`, `^` as primary; trailing exponent-without-caret (`ft3` for `ft^3`) and dash-style (`ft-s-1` for `ft/s`) as accepted alternates.

- Primary form matches ISO 80000 and most engineering print
- Alternates accommodate VBA-origin code and the original docx convention

### 13.6 Case sensitivity
**Decision**: case-sensitive by default. `TryParseLoose` available for end-user input.

- `M` (mega) vs `m` (meter) is the canonical example
- Strict parsing in code; lenient parsing at user-input boundaries

### 13.7 Operator overloading on the base class
**Decision**: declared on `EngineeringUnit` as a fallback, overridden on subclasses where strongly-typed return is possible.

- `Length + Length → Length` (typed; declared on `Length`)
- `Length + Time` (statically typed) → compile error
- `EngineeringUnit a + EngineeringUnit b` (dynamically typed) → runtime `DimensionMismatchException` if dimensions mismatch

### 13.8 Significant figures vs uncertainty
**Decision**: significant figures only in v1. Uncertainty (`±`) deferred to v2 if demand exists.

- Sig figs cover 95% of report-formatting needs
- Full uncertainty propagation is a significantly larger design space

### 13.9 Temperature
**Decision**: temperatures carry an `Offset` in the `Unit` struct, applied during conversion. `°C` and `°F` and `°R` and `K` all share dimension `[Θ]` but differ in offset+scale.

- Addition of temperature **deltas** uses linear conversion only (60°C + 5°C delta = 65°C). Care taken in the API to avoid the classic "add two absolute temperatures" trap by exposing `TemperatureDelta` as a distinct typed wrapper for differences.

### 13.10 Dimensionless interpretation
**Decision**: never auto-inferred. User opts in via `PreferredInterpretation`.

- Many velocity ratios are not Mach numbers
- Many length-to-length ratios are not strain
- Heuristics here would produce wrong-looking output more often than helpful output

### 13.11 Documentation tooling
**Decision**: XML doc comments on every public member, generated docs via DocFX.

- DocFX is current and actively maintained (vs. Sandcastle which is largely unmaintained)
- Sandcastle output is still supported if you'd rather stay on it — XML comments are the same input either way

### 13.12 MMSCFD condition defaults
**Decision**: bare `MMSCFD` defaults to 14.73 psia / 60 °F (GPSA / AGA interstate-pipeline standard). Other conditions are named-alias variants (`MMSCFD_petro` for 14.696 psia, `MMSCFD_iupac` for 100 kPa/0 °C) or parametric (`MMSCFD@<psia>`).

- Most users in scope (interstate gas pipeline operators) expect 14.73 psia
- Explicit alternates avoid silent surprises for petroleum / non-US contexts
- The parametric form handles edge cases (e.g. some Canadian operators use 14.65 psia) without requiring catalog additions

### 13.13 Value and unit are separable
**Decision**: the library exposes `Value` and `DisplayUnit` as independent public properties. The string form (`"5.5 ft"`) is round-trippable but is not the only access pattern.

- Required to enable the Excel adjacent-cells convention without ugly string parsing in hot paths
- Useful for any downstream that wants to serialize value and unit separately (JSON `{"value": 5.5, "unit": "ft"}`, gRPC message fields, database columns)

### 13.14 Torque vs energy disambiguation
**Decision**: `Torque` and `Energy` are distinct subclasses. Both have fundamental dimension `M·L²/T²`. Resolution rules:

- **Parsing** uses display-unit order convention: `lbf*ft` → `Torque`, `ft*lbf` → `Energy`. The first token is the "primary" dimension.
- **Multiplication** uses operand-type order: `Force * Length` → `Torque`, `Length * Force` → `Energy`. Operator overloads on each subclass specify which.
- **Explicit conversion**: `.AsTorque()` and `.AsEnergy()` instance methods reinterpret a quantity if the user knows what they meant.
- **Display** preserves the convention: a `Torque` instance always displays force-first (`lbf*ft`); an `Energy` instance always displays length-first (`ft*lbf`).

This is a notation-driven distinction (the dimensions truly are identical), so the library defers to the user's expressed intent and never auto-converts between the two.

### 13.15 Angle as a tracked pseudo-dimension
**Decision**: angle (radians) is tracked as a 9th slot in `DimensionSignature`, separate from the 8 SI base dimensions plus the seven SI-base + temperature.

Wait — re-stating to avoid the off-by-one:

- 7 SI base dimensions (Length, Mass, Time, Current, Temperature, Amount of substance, Luminous intensity)
- Plus 1 pseudo-dimension for **plane angle** (radians)
- Total: 8 slots in `DimensionSignature`

Tracking angle as a pseudo-dimension lets the library distinguish frequency (`1/s`) from angular velocity (`rad/s`), and linear power (`W` from `F·v`) from rotational power (`W` from `τ·ω`). SI nominally treats radians as dimensionless, but most engineering computation benefits from keeping them tracked — exactly the same convention used by `pint` and `boost::units`.

A future v2 may add solid angle (steradian) as a 9th slot if demand exists; v1 ignores it.

### 13.17 Preferred units are per-dimension, not per-system

**Decision**: a `UnitPreferences` map associates each dimension with the user's preferred unit. There is no binary "SI vs US customary" toggle.

Rationale: even within SI, the conventionally-preferred unit depends on the dimension and the application. SI base for length is `m`, but travel distance is `km`, machining tolerance is `mm`, atomic scale is `nm`. A petroleum engineer wants `bbl` for volume and `psi` for pressure but `°F` for temperature and `lb` for mass. The library expresses this as a flat map:

```csharp
public sealed class UnitPreferences
{
    /// <summary>Sets the preferred display unit for whatever dimension this unit symbol carries.</summary>
    public void Prefer(string unit);  // e.g. Prefer("km") sets Length's preference to km

    /// <summary>Returns the preferred unit for the given dimension, or null if none set.</summary>
    public Unit? GetPreferred(DimensionSignature dim);

    /// <summary>Notation style for compound units. Caret (m/s^2) or Unicode (m·s⁻²).</summary>
    public NotationStyle Notation { get; set; } = NotationStyle.Caret;

    /// <summary>Built-in profiles. User can also build custom ones from scratch.</summary>
    public static UnitPreferences SIScientific  { get; }  // m, kg, s, K, Pa, J, W, ... (Unicode notation)
    public static UnitPreferences SIEveryday    { get; }  // km, kg, h, °C, kPa, kJ, kW, ...
    public static UnitPreferences UsCustomary   { get; }  // ft, lb, s, °F, psi, BTU, hp, ... (Caret notation)
    public static UnitPreferences UsEngineering { get; }  // ft, lbm, s, °F, psia, BTU/hr, hp, ...
    public static UnitPreferences OilAndGas     { get; }  // bbl, MMSCFD, psia, °F, lbm, ...
    public static UnitPreferences Machining     { get; }  // mm, kg, s, °C, MPa, ...
}

public enum NotationStyle
{
    /// <summary>ASCII caret form: m/s^2, kg*m/s^2.</summary>
    Caret,
    /// <summary>Unicode superscript form: m·s⁻², kg·m·s⁻².</summary>
    Unicode
}
```

Preferences only affect **display when explicitly requested** — they never override the left-operand-wins rule (Decision 13.4) for arithmetic results' implicit unit. To use preferences:

```csharp
EngineeringUnit.DefaultPreferences = UnitPreferences.OilAndGas;
var p = new Pressure(2_000_000, "Pa");
Console.WriteLine(p);            // "2000000 Pa"  (operand's unit; preferences not yet invoked)
Console.WriteLine(p.ToString("P"));  // "290.075 psia"  ("P" = preferred-unit format code)
Console.WriteLine(p.In("psia"));     // "290.075 psia"  (explicit override, same result here)
```

The `"P"` format code (added to Section 8.2) consults `DefaultPreferences` for this quantity's dimension and renders in that unit. If no preference is registered for the dimension, it falls back to the operand's display unit.

Profiles can be cloned and customized:

```csharp
var myPrefs = UnitPreferences.OilAndGas.Clone();
myPrefs.Prefer("mm");   // override length to mm even though OilAndGas had it as ft
```

Per-call overrides via `.In("km")` always win over preferences — the user can force a specific unit at the call site without touching the global preference state.

`DefaultPreferences` is `[ThreadStatic]`-attributed: in ASP.NET Core or multithreaded hosts, each thread gets its own preferences, and one request setting a profile doesn't bleed into another. (Library default is `UnitPreferences.SIScientific`.)

### 13.18 Recognize compound-unit aliases when possible

**Decision**: when arithmetic produces a quantity whose dimension matches a registered single-symbol unit, prefer the single-symbol form for display.

Examples:
- `Mass * Acceleration` (`kg * m/s²`) → display as `N`
- `Energy / Time` (`J / s`) → display as `W`
- `Force / Area` (`N / m²`) → display as `Pa`

Recognition is **dimensional**, subject to the disambiguations already locked in:
- Same-dimension subclasses stay distinct (Decision 13.14: Torque vs Energy never collapse into each other).
- The Caret/Unicode notation style (Decision 13.17) applies to the recognized form.
- The result inherits its display-unit *system* from the left operand: `Mass(kg) * Acceleration(m/s²)` → `N`, but `Mass(lbm) * Acceleration(ft/s²)` → `lbf` (the US-customary force unit), not `N`.

If no registered alias matches the dimension, the library falls back to the expanded compound expression (`kg·m·s⁻²`) using the current notation style. Recognition is best-effort, not contractual — a user who needs a specific display form should call `.In("...")` explicitly.

### 13.16 Open core / dual-licensing
**Decision**: the `EngineeringUnits` core library is **MIT-licensed**. Integration assemblies (ExcelDNA UDFs, etc.) are **separate projects under separate (proprietary) licenses** that consume `EngineeringUnits` as a NuGet dependency.

- Core library is in its own GitHub repository with `LICENSE` (MIT)
- Each integration assembly is in its own repository (private or public per its own license)
- Integration assemblies reference the core via NuGet, not source-level inclusion — keeps the license boundary clean
- This is the canonical open-core model (GitLab CE/EE, Elastic, Confluent, etc.); see [TermsFeed: Dual Licensing vs. Open Core](https://www.termsfeed.com/blog/dual-licensing-vs-open-core/) and [Wikipedia: Open-core model](https://en.wikipedia.org/wiki/Open-core_model) for the broader pattern

---

## 14. Open questions

Things still unclear that need decisions before or during implementation. Best to surface these now.

1. ~~**MMSCFD definition.**~~ **Resolved** in Decision 13.12 — default 14.73 psia / 60 °F, named alternates for other industries.

2. **VBA function inventory.** You mentioned having a VBA library that isn't critical at this point. If/when you want Phase 1 coverage to mirror what's already in your VBA codebase, share the `.bas` (or workbook) and I'll prioritize the unit-catalog additions accordingly.

3. **Decimal vs double.** Default assumption is `double` (15–17 sig figs). Switching to `decimal` would cost performance and lose `Pow` support (decimal doesn't have it natively). Engineering use almost always wants `double`; flag this only if you have a specific decimal scenario.

4. ~~**Per-instance preferred-unit override.**~~ **Resolved** in Decision 13.17 — preferences are a per-dimension map, not a system flag. Built-in profiles (`SIScientific`, `OilAndGas`, etc.) plus user customization, opt-in via `"P"` format code. Arithmetic still follows left-operand-wins regardless of preferences.

5. ~~**Excel cell format.**~~ **Resolved** in Decision 13.13 — value and unit are separable; Excel cells store the numeric value in one cell with the unit in an adjacent cell.

6. ~~**Negative-exponent display.**~~ **Resolved** in Decision 13.17 — `UnitPreferences.Notation` toggles between `NotationStyle.Caret` (`m/s^2`, the default) and `NotationStyle.Unicode` (`m·s⁻²`). Default profile uses Caret; `SIScientific` profile uses Unicode (matches ISO 80000 print convention); user can override on any profile.

7. ~~**Compound display unit recognition.**~~ **Resolved** in Decision 13.18 — yes, recognize known aliases when the dimension matches, with the same disambiguation rules as Decision 13.14 (torque/energy stay distinct) and respect for the left-operand's unit system.

8. **Solid angle (steradian).** Not in v1. Add later if needed?

9. **Operator chaining allocation pressure.** A chain like `(a + b - c) * d / e` allocates 4 intermediate instances. Worth a struct-based "QuantityBuilder" for hot paths in v1, or accept the GC pressure since the performance budget already factors in 1 allocation per op?

10. ~~**Public release.**~~ **Resolved** in Decision 13.16 — core library MIT, integration assemblies separate.

11. **Repository layout.** Going with single-repo (`src/`, `tests/`, `samples/`, `docs/`) unless you flag otherwise — confirmed by the move-to-its-own-repo step.

12. ~~**SemVer policy.**~~ **Resolved** — standard SemVer. New unit = minor bump; rename/remove = major; bug fix = patch.

13. ~~**CI / build.**~~ **Resolved** — GitHub Actions: build + test on every push/PR; DocFX deploy to GitHub Pages on push to main; NuGet publish on tagged release. Set up when going public.

---

## 15. References

### Primary sources

- Original docx draft — `EngineeringUnits/Engineering Units Specification.docx`
- ISO 80000 — quantities and units standard (the formal one)
- GPSA Engineering Data Book — basis for 14.73 psia / 60 °F MMSCFD default
- AGA Report No. 3 — orifice metering standard for natural gas (consistent with 14.73 psia base)

### Reference designs (other unit-aware libraries)

- `pint` (Python) — https://pint.readthedocs.io/ — closest design philosophy match; uses dimension vectors with optional angle tracking
- `boost::units` (C++) — https://www.boost.org/doc/libs/release/libs/units/ — compile-time generic approach (not chosen here, but useful for comparison)
- F# Units of Measure — https://learn.microsoft.com/dotnet/fsharp/language-reference/units-of-measure — what we deliberately did *not* do, since it requires generic templates
- UnitsNet — https://github.com/angularsen/UnitsNet — popular .NET library; subclass-per-quantity, similar to what we propose. Worth reviewing for API conventions and unit catalog coverage before committing to Phase 1 details.

### Industry data

- Engineering units reference workbook — https://www.optimizedtechnicalsolutions.com/ReferenceDocuments/EngineeringUnits.xls
- Dimensionless numbers reference — https://www.iist.ac.in/sites/default/files/2025-06/dimensionless.pdf
- General unit converter — https://www.unitconverters.net/
- MMSCFD interstate-pipeline conditions guide — https://midstreamcalculator.com/engineering/compressors/compressor-flow-conversion-fundamentals.html
- AGA Report No. 3 in pipeline practice — https://www.jmcampbell.com/tip-of-the-month/2022/06/nord-stream-long-distance-gas-pipeline-part-3-application-of-basic-and-aga-equations-for-estimating-maximum-gas-flow-in-a-long%E2%80%90distance-pipeline/

### Tooling and downstream

- ExcelDNA — https://excel-dna.net/ — runtime for the proprietary Excel integration library (separate project)
- DocFX — https://dotnet.github.io/docfx/ — documentation generator
- ChooseALicense — https://choosealicense.com/ — license decision guide

### Open-core / dual licensing background

- TermsFeed: Dual Licensing vs Open Core — https://www.termsfeed.com/blog/dual-licensing-vs-open-core/
- Wikipedia: Open-core model — https://en.wikipedia.org/wiki/Open-core_model
- Open Core Ventures handbook — https://handbook.opencoreventures.com/

---

## 16. License and distribution

### Core library (this project)

`EngineeringUnits` is licensed under the **MIT License**. The license text is in `LICENSE` at the repo root. Anyone may use, modify, fork, and redistribute the library — including in proprietary or commercial products — provided the copyright notice is preserved.

The repository is intended to be **public on GitHub**, distributed via **NuGet**, with **DocFX-generated documentation** published to GitHub Pages.

### Downstream integration libraries (separate projects)

Specific integration libraries (ExcelDNA UDF assembly, future ASP.NET Core helpers if any, etc.) are **out of scope of this spec and this repository**. They live in their own repositories, with their own licenses (potentially proprietary), and consume `EngineeringUnits` as an external NuGet package — never as source-level inclusion. This separation is the canonical open-core pattern and keeps the license boundary clean for both the open community and any commercial offerings.

The MIT license on `EngineeringUnits` explicitly permits this: a proprietary downstream consumer can ship a closed product that depends on the MIT-licensed library, as long as the MIT notice travels with the binary.
