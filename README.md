# ReactionStoichiometry

A chemical reaction equation balancer written in C# (.NET 8). Given a skeletal equation, it finds the balancing coefficients using exact rational-number linear algebra — and unlike most balancers, it returns the **general solution**, not just one set of numbers.

```
TiO2 + C + Cl2 = TiCl4 + CO + CO2
```
has no unique answer. This tool tells you so, and gives you the full answer space:

```
x01·TiO2 + x02·C + x03·Cl2 + x04·TiCl4 + x05·CO + x06·CO2 = 0
with coefficients {(-x05 - 2·x06)/2, -x05 - x06, -x05 - 2·x06, (x05 + 2·x06)/2, x05, x06}
```

Pick any values for the free variables `x05` and `x06`, and you get a valid balanced equation.

---

## Features

- **Exact arithmetic** — no floating point anywhere; built on [Rationals](https://github.com/tompazourek/Rationals) and `BigInteger`, so it handles equations with 20+ substances and coefficients in the billions without precision loss.
- **General solutions** — equations with multiple independent solutions (nullity > 1) are solved fully, two different ways (see below).
- **Honest failure** — equations that cannot be balanced (e.g. `FeS2 + HNO3 = Fe2(SO4)3 + NO + H2SO4`) are reported as unsolvable with a reason, not force-fitted.
- **No chemistry table inside** — element symbols are just tokens, so placeholder symbols (`Ln` for a lanthanoid, `Ph` for phenyl) work fine.
- **Ion charges, berthollides, nested parentheses** — all supported by the input syntax.
- **JSON export** of the entire computation: substances, composition matrix, RREF, and both solutions.

---

## How It Works

1. The equation is parsed into a **chemical composition matrix (CCM)**: one row per chemical element, one column per substance, entries are element counts (rational, to allow decimal indices).
2. The matrix is reduced to **Reduced Row Echelon Form (RREF)** using exact rational arithmetic.
3. Balancing coefficients are exactly the **null space** of the CCM. Its dimension (nullity) tells you how many independent solutions exist:
   - **nullity = 0** — the equation cannot be balanced.
   - **nullity = 1** — the familiar textbook case: one solution up to scaling.
   - **nullity ≥ 2** — infinitely many structurally different solutions.

All coefficients are reported in the *signed* convention: the equation is rewritten as `x01·A + x02·B + x03·C = 0`, so reactants get negative coefficients and products positive. For `H2 + O2 = H2O` the solution is `{-2, -1, 2}`, i.e. **2**H2 + **1**O2 → **2**H2O.

### Two solution strategies

The null space is described in two complementary ways, both available on every equation:

**Rows-based** (`RowsBasedSolution`) — parametric form. Dependent coefficients are expressed as algebraic formulas of the free ones. For `H2 + O2 + Na = H2O`:

```
x01 = -x04
x02 = -x04/2
x03 = 0        ← Na can't participate; the math says so
x04             ← free
```

Call `Instantiate(values)` to plug in free-variable values and get integer coefficients back.

**Columns-based** (`ColumnsBasedSolution`) — basis form. A set of independent integer coefficient vectors spanning the null space. For `C6H5C2H5 + O2 = C6H5OH + CO2 + H2O` the basis is:

```
{ 6, -7, -10, 12, 0}
{-6, -7,   8,  0, 6}
```

Every balanced version of that equation is a linear combination of these two vectors. Call `CombineIndependents(4, 5)` to build one, or `FindCombination(coefficients)` to decompose a known solution back into basis weights.

---

## Input Syntax

- Spaces are ignored; substances separated by `+`, sides separated by `=`.
- Element symbols are one capital letter optionally followed by one lowercase letter. Any such token works — no periodic table is consulted.
- Parentheses, including nested: `(Ru(C10H8N2)3)Cl2(H2O)6`.
- Decimal indices for berthollides (non-stoichiometric compounds): `Fe0.996(H2PO4)2H2O`.
- **Ion charges** via two reserved symbols: `Qn` (negative) and `Qp` (positive).
  - `IQn3` means I³⁻
  - `HQp` means H⁺ (a proton)
  - Example: `IO4Qn + IQn = IO3Qn + I3Qn + H2O + OHQn`

### Sample equations, simple to extreme

```
N2O4 = NO2
O2 = O3
Ni(CO)4 = Ni + CO
CaO + P2O5 = Ca3(PO4)2
NH3 + H2SO4 = (NH4)2SO4
Ca3(PO4)2 + H3PO4 = Ca(H2PO4)2
NaHCO3 + Ca(H2PO4)2 = Na2HPO4 + CaHPO4 + CO2 + H2O
(CH3)2N2H2 + N2H4 + N2O4 = CO2 + H2O + N2
IO4Qn + IQn = IO3Qn + I3Qn + H2O + OHQn
Fe2(SO4)3 + PrTlTe3 + H3PO4 = Fe0.996(H2PO4)2H2O + Tl1.987(SO3)3 + Pr1.998(SO4)3 + Te2O3 + P2O5 + H2S
C2952H4664N812O832S8Fe4 + Na2C4H3O4SAu + Fe(SCN)2 + Fe(NH4)2(SO4)2(H2O)6 + C4H8Cl2S + C8H12MgN2O8
    = C55H72MgN4 + Na3.99Fe(CN)6 + Au0.987SC6H11O5 + HClO4 + H2S
```

…and a 23-reactant, 17-product stress test (see `ColumnsBasedSolutionTests.FindCombination_Complex`) whose smallest integer coefficients run into the tens of billions — solved exactly.

---

## API Usage

```csharp
using System.Numerics;
using ReactionStoichiometry;

if (!ChemicalReactionEquation.IsValidString("H2 + O2 = H2O")) return;

var equation = new ChemicalReactionEquation("H2 + O2 = H2O");

// --- Rows-based: parametric solution ---
var rbs = equation.RowsBasedSolution;          // lazy, computed on first access
if (rbs.Success)
{
    // "x01·H2 + x02·O2 + x03·H2O = 0 with coefficients {-2, -1, 2}"
    Console.WriteLine(rbs.ToString(OutputFormat.Simple));

    // Formulas, one per substance: "x01 = -x03", ...
    foreach (var expr in rbs.AlgebraicExpressions!) Console.WriteLine(expr);

    // One ready-made integer solution (all free variables set to 1, scaled)
    BigInteger[] sample = rbs.InstanceSample!.ToArray();

    // Or choose your own free-variable values
    BigInteger[] mine = rbs.Instantiate(2);    // one value per free coefficient

    // Verify and pretty-print
    if (equation.Validate(mine))
        Console.WriteLine(equation.EquationWithIntegerCoefficients(mine));
}
else
{
    Console.WriteLine(rbs.ToString(OutputFormat.Simple)); // failure mark
}

// --- Columns-based: null-space basis ---
var cbs = equation.ColumnsBasedSolution;
if (cbs.Success)
{
    foreach (var vector in cbs.IndependentSetsOfCoefficients!)
        Console.WriteLine(equation.EquationWithIntegerCoefficients(vector));

    // Linear combinations (only meaningful when nullity >= 2)
    // BigInteger[] combined = cbs.CombineIndependents(4, 5);
    // int[]? weights = cbs.FindCombination(someKnownCoefficients);
}

// --- Full computation as JSON ---
Console.WriteLine(equation.ToJson());
```

### Output formats

`Solution.ToString(OutputFormat)` accepts:

| Format | Contents |
|---|---|
| `Simple` | One line: generalized equation + coefficients |
| `Multiline` | Coefficient formulas one per line, plus free-variable list |
| `DetailedMultiline` | Everything: skeletal equation, CCM with element/substance headers, RREF, rank & nullity, then the solution |

### Key types

| Type / member | Purpose |
|---|---|
| `ChemicalReactionEquation(string)` | Parses the equation, builds CCM and RREF |
| `ChemicalReactionEquation.IsValidString(string)` | Static syntax pre-check (use before constructing) |
| `.Substances`, `.InGeneralForm` | Parsed substances; `x01·A + x02·B … = 0` form |
| `.Validate(BigInteger[])` | True if the coefficients balance every element |
| `.EquationWithIntegerCoefficients(BigInteger[])` | Human-readable balanced equation |
| `.ToJson()` | Full serialization (substances, CCM, RREF, both solutions) |
| `SolutionRowsBased.AlgebraicExpressions` | Per-substance formulas in free variables |
| `SolutionRowsBased.Instantiate(params BigInteger[])` | Free-variable values → integer coefficients (throws on non-integer results) |
| `SolutionColumnsBased.IndependentSetsOfCoefficients` | Null-space basis vectors |
| `SolutionColumnsBased.CombineIndependents(params int[])` | Weighted sum of basis vectors, reduced |
| `SolutionColumnsBased.FindCombination(params BigInteger[])` | Decompose a solution into basis weights (null if impossible) |
| `Solution.Success` / failure message in output | Whether balancing succeeded |

---

## Repository Layout

| Project | Description |
|---|---|
| `ReactionStoichiometry` | Core library (the only thing you need as a dependency) |
| `ReactionStoichiometry.CLI` | Console app: batch-processes equations, prompts interactively |
| `ReactionStoichiometry.GUI` | Windows Forms UI with live instantiation/combination grids and an HTML report |
| `ReactionStoichiometry.Tests` | xUnit suite, incl. a 70-equation textbook batch (`70_from_the_book.txt`) |
| `ReactionStoichiometry.JsonViewer` | HTML/JS viewer for the exported JSON |
| `ReactionStoichiometry.ValidationTool` | VB.NET cross-check utility |
| `ReactionStoichiometry.PoC` | Original Python proof-of-concept (NumPy/SymPy) |

**Dependencies:** [Rationals](https://github.com/tompazourek/Rationals) (exact fractions), Newtonsoft.Json. Tests use xUnit.

---

## Build & Test

Requires the [.NET 8.0 SDK](https://dotnet.microsoft.com/download). The core library and CLI are cross-platform; the GUI is Windows-only (WinForms + WebView2).

```bash
dotnet build
dotnet test
```

---

## License

See [LICENSE.txt](LICENSE.txt).
