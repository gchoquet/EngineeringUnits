# Getting Started

## Install

```powershell
dotnet add package EngineeringUnits
```

## First quantity

```csharp
using EngineeringUnits;

var l = new Length(5, "ft");
Console.WriteLine(l);              // "5.000 ft"
Console.WriteLine(l.As("m"));      // 1.524
Console.WriteLine(l.In("m"));      // "1.524 m"
```

## Mixed-unit arithmetic

```csharp
var total = new Length(5, "ft") + new Length(6, "in");
Console.WriteLine(total);          // "5.500 ft" — left operand's unit governs
```

## Cross-type arithmetic

The compiler picks the right result type from operand types:

```csharp
var area = new Length(3, "m") * new Length(4, "m");         // Area
var v    = new Length(100, "m") / new Time(10, "s");        // Velocity
var F    = new Mass(10, "kg") * new Acceleration(9.81, "m/s^2");  // Force (N)
var P    = F / new Area(0.1, "m^2");                        // Pressure (Pa)
var V    = new Voltage(120, "V");
var I    = V / new ElectricResistance(60, "Ω");             // ElectricCurrent (Ohm's law)
var pwr  = V * I;                                            // Power (W)
```

## Industry-specific units

```csharp
var flow = new VolumetricFlowRate(10, "MMSCFD");   // 14.73 psia / 60 °F default
var visc = new DynamicViscosity(1.0, "cP");        // water at 20 °C
var energy = new Energy(1, "MMBTU");
```

## Format codes

```csharp
var dia = new Length(0.5, "m");
Console.WriteLine(dia.ToString("L"));    // "0.5000 meter"
Console.WriteLine(dia.ToString("U"));    // "1.640 ft"   (US-customary)
Console.WriteLine(dia.ToString("D"));    // "0.5000 m (1.640 ft)"   (dual)
Console.WriteLine(dia.ToString("N6"));   // "0.500000 m"  (force 6 sig figs)
Console.WriteLine(dia.ToString("{cm}")); // "50.00 cm"    (force unit)
```

## Preferred unit profiles

```csharp
UnitPreferences.Default = UnitPreferences.OilAndGas;
Console.WriteLine(new Pressure(1, "atm").ToString("P"));   // displays in psia
Console.WriteLine(new Volume(1, "m^3").ToString("P"));     // displays in bbl
```

Built-in profiles: `SIScientific`, `SIEveryday`, `UsCustomary`, `UsEngineering`, `OilAndGas`, `Machining`.

## Temperature arithmetic

Adding two absolute temperatures throws (physically meaningless). Subtraction produces a `TemperatureDelta`:

```csharp
var t1 = new Temperature(30, "degC");
var t2 = new Temperature(20, "degC");
var dt = t1 - t2;                                  // TemperatureDelta of 10 degC
var t3 = t1 + new TemperatureDelta(5, "degC");     // Temperature of 35 degC
```

## Pow / Abs

```csharp
var l = new Length(3, "m");
var a = l.Pow(2);                  // Area (9 m²)
var v = l.Pow(3);                  // Volume (27 m³)
var l2 = new Area(9, "m^2").Pow(0.5);  // back to Length (3 m)

var negL = new Length(-5, "ft");
var absL = negL.Abs();             // Length (5 ft) — preserves display unit
```
