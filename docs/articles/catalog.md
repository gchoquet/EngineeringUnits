# Catalog Reference

The unit catalog is populated at module load and frozen. Lookup is case-sensitive (so `m` and `M` are distinct). For end-user input, prefer the case-sensitive symbols listed below.

| Dimension | Symbol → SI base | Common units |
|---|---|---|
| Length | → m | m, km, cm, mm, μm/um, nm, Mm, in, ft, yd, mi, nmi, fathom, furlong |
| Mass | → kg | kg, g, mg, μg/ug, t, lb, lbm, oz, slug, ton, lt, Da, u |
| Time | → s | s, ms, μs/us, ns, min, h, hr, day, week, year, yr |
| Temperature | → K | K, °C/degC, °F/degF, °R/degR |
| Electric Current | → A | A, mA, μA, nA, kA |
| PlaneAngle | → rad | rad, deg/°, rev, grad |
| Area | → m² | m², cm², mm², km², ha, in², ft², yd², mi², acre |
| Volume | → m³ | m³, cm³, mm³, L, mL, μL, in³, ft³, yd³, gal, gal_imp, qt, pt, cup, floz, bbl |
| Velocity | → m/s | m/s, cm/s, mm/s, km/h, kph, ft/s, ft/min, mph, knot/kn |
| Acceleration | → m/s² | m/s², ft/s², g_n, Gal |
| AngularVelocity | → rad/s | rad/s, rad/min, deg/s, rpm, rps |
| Frequency | → Hz | Hz, kHz, MHz, GHz, 1/s, 1/min |
| Force | → N | N, kN, MN, mN, lbf, kgf, dyn, ozf |
| Pressure | → Pa | Pa, kPa, MPa, GPa, hPa, mPa, bar, mbar, atm, psi, psia, psig, ksi, Torr, mmHg, inHg, inH2O |
| Energy | → J | J, kJ, MJ, GJ, mJ, Wh, kWh, MWh, BTU, kBTU, MMBTU, cal, kcal, erg, ft*lbf |
| Torque | → N·m | N*m, lbf*ft, lbf*in, kgf*m |
| Power | → W | W, kW, MW, GW, mW, hp, hp_e, hp_m, BTU/hr, BTU/s, ft*lbf/s, MMBTU/hr |
| Density | → kg/m³ | kg/m³, g/cm³, g/mL, kg/L, lb/ft³, lb/in³, lb/gal |
| Momentum | → kg·m/s | kg*m/s, N*s, lb*ft/s, slug*ft/s, lbf*s |
| MassFlowRate | → kg/s | kg/s, kg/min, kg/h, g/s, lb/s, lb/min, lb/h, t/h |
| VolumetricFlowRate | → m³/s | m³/s, m³/h, L/s, L/min, L/h, ft³/s, cfs, cfm, gpm, bbl/day, bpd, MMSCFD, MMSCFD_petro, MMSCFD_iupac, MSCFD, SCFD, SCFM |
| DynamicViscosity | → Pa·s | Pa*s, mPa*s, P, cP, lb/(ft*s), lb/(ft*hr) |
| KinematicViscosity | → m²/s | m²/s, mm²/s, St, cSt, ft²/s |
| ThermalConductivity | → W/(m·K) | W/(m*K), kW/(m*K), BTU/(hr*ft*°F), cal/(s*cm*°C) |
| HeatCapacity | → J/K | J/K, kJ/K, BTU/°F, cal/K |
| SpecificHeatCapacity | → J/(kg·K) | J/(kg*K), kJ/(kg*K), cal/(g*°C), BTU/(lb*°F) |
| HeatFluxDensity | → W/m² | W/m², kW/m², BTU/(hr*ft²), cal/(s*cm²) |
| SpecificEnergy | → J/kg | J/kg, kJ/kg, MJ/kg, BTU/lb, cal/g, kcal/kg, Wh/kg, kWh/kg |
| EnergyDensity | → J/m³ | J/m³, kJ/m³, MJ/m³, BTU/ft³, Wh/L, kWh/L |
| SpecificVolume | → m³/kg | m³/kg, L/kg, ft³/lb, cm³/g |
| AreaDensity | → kg/m² | kg/m², g/m², lb/ft², oz/yd² |
| SurfaceTension | → N/m | N/m, mN/m, dyn/cm, lbf/ft, lbf/in |
| ElectricCharge | → C | C, mC, μC, nC, pC, Ah, mAh |
| Voltage | → V | V, mV, μV, kV, MV |
| ElectricResistance | → Ω | Ω, ohm, mΩ, kΩ, MΩ, GΩ |
| ElectricalConductance | → S | S, mS, μS |
| ElectricCapacitance | → F | F, mF, μF, nF, pF |
| Inductance | → H | H, mH, μH, nH, pH |
| Dimensionless | → 1 | 1 (and the bare-number parser) |

## MMSCFD variants

`MMSCFD` defaults to GPSA / AGA Report No. 3 standard conditions (14.73 psia, 60 °F), the convention used on US interstate gas pipelines. Variants:

| Symbol | Conditions | Notes |
|---|---|---|
| `MMSCFD` | 14.73 psia, 60 °F | Default. GPSA / AGA. |
| `MMSCFD_petro` | 14.696 psia, 60 °F | Petroleum / refining. |
| `MMSCFD_iupac` | 100 kPa, 0 °C | IUPAC standard. |

The volumetric conversion is the same for all three (geometric `ft³ → m³`); the variants differ only in the conditions they *carry*. If a future release adds mass-flow-from-volumetric-flow conversion via the ideal gas law, the conditions become load-bearing.

## How to register additional units

Out of scope for v1 — the catalog is frozen at module load to make multithreaded reads lock-free. If your domain needs a unit not listed, file an issue against the repository and we'll evaluate whether to add it to the catalog or whether you should use the parametric form (planned for v1.x).
