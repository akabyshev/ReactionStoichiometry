const INTERPUNCT = "\u00B7";

function MakeJsonReadable(Equation, identifier) {
  const recordDiv = document.createElement("div");

  Equation.Substances = Equation.Substances.map((substance) =>
    substance.replace(/(\d+(\.\d+)?)/g, "<sub>$1</sub>")
  );

  Equation.Labels = Equation.Labels.map((label) =>
    label.replace(/(\d+(\.\d+)?)/g, "<sub>$1</sub>")
  );

  if (Equation.GeneralizedSolution.AlgebraicExpressions) {
    Equation.GeneralizedSolution.AlgebraicExpressions =
      Equation.GeneralizedSolution.AlgebraicExpressions.map((expression) =>
        expression.replace(/(?<=x)\d{2}/g, "<sub>$&</sub>")
      );
  }

  const tableCCM = createTable(
    Equation.CCM,
    (index) => Equation.Elements[index],
    (index) => Equation.Labels[index],
    (index) => Equation.Substances[index]
  );

  const tableRREF = createTable(
    Equation.RREF,
    (index) => "0" + " = ",
    (index) => Equation.Labels[index]
  );

  recordDiv.innerHTML = `
    <h3>${identifier}</h3>
    <p>
    We begin by expressing the original input
    <p class="cre">${Equation.OriginalEquationString}</p>
    in generalized form of
    <p class="cre">${constructGeneralizedEquation(Equation)}</p>
    </p>
    <p>Following this, a chemical composition matrix (CCM) is constructed: ${
      tableCCM.outerHTML
    }</p>
    <p>The CCM is transformed into its reduced row echelon form (RREF) using Gaussian elimination: ${
      tableRREF.outerHTML
    }</p>`;

  if (Equation.GeneralizedSolution.Success === false) {
    recordDiv.innerHTML += `
      <p>That results in an identity matrix, indicating that it's impossible to balance the provided equation.</p>
      `;
  } else {
    const tableExpressions = createTable(
      Equation.GeneralizedSolution.AlgebraicExpressions.filter((item) =>
        item.includes(" = ")
      ).map((item) => [item.split("=")[1].trim()]),
      (index) => Equation.Labels[index] + " = ",
      () => "Expression"
    );

    recordDiv.innerHTML += `
      <p>
      The RREF demonstrates how all coefficients can be expressed as linear functions of <u>${
        Equation.GeneralizedSolution.FreeVariableIndices.length > 1
          ? "free variables"
          : "the free variable"
      }</u><b>
        ${Equation.GeneralizedSolution.FreeVariableIndices.map(
          (index) => Equation.Labels[index]
        ).join(", ")}</b>:
        ${tableExpressions.outerHTML}
      </p>`;

    if (Equation.GeneralizedSolution.FreeVariableIndices.length === 1) {
      const tableFoundSolution = createTable(
        Equation.Labels.map((item, index) => [
          Equation.GeneralizedSolution.SimplestSolution[index],
        ]),
        (index) => Equation.Labels[index] + " = ",
        () => "Value"
      );

      recordDiv.innerHTML += `<p>The next step is determining a value of the free variable that yields integer coefficients. Setting 
      <b>${
        Equation.Labels[Equation.GeneralizedSolution.FreeVariableIndices[0]]
      }</b> equal to <u>the least common multiple of all the denominators</u> results in: ${
        tableFoundSolution.outerHTML
      }</p>
      <p>So the final solution is <p class="cre">${AssembleEquationString(
        Equation.Substances,
        Equation.GeneralizedSolution.SimplestSolution
      )}</p></p>
      `;
    } else {
      if (Equation.InverseBasedSolution.Success) {
        const tableSubreactions = createTable(
          Equation.InverseBasedSolution.IndependentReactions,
          (index) => "0" + " = ",
          (index) => Equation.Substances[index]
        );
        recordDiv.innerHTML += `<p>Any integer solution will be a combination of these 'subreactions': ${tableSubreactions.outerHTML} </p>`;

        if (Equation.InverseBasedSolution.CombinationSample.Item2) {
          const tableCombination = createTable(
            Equation.Labels.map((item, index) => [
              Equation.InverseBasedSolution.CombinationSample.Item2[index],
            ]),
            (index) => Equation.Labels[index] + " = ",
            () => "Value"
          );
          recordDiv.innerHTML += `</p>For example, ${
            "(" +
            Equation.InverseBasedSolution.CombinationSample.Item1.join(", ") +
            ")"
          } combination of those yields ${tableCombination.outerHTML}</p>
          <p class="cre">${AssembleEquationString(
            Equation.Substances,
            Equation.InverseBasedSolution.CombinationSample.Item2
          )}</p>`;
        }
      } else {
        recordDiv.innerHTML += `<p>Discover a solution instance by utilizing a calculator, Excel, or the 'Instantiation' feature in our GUI. </p>`;
      }
    }
  }

  recordDiv.style.border = "1px solid black";
  recordDiv.style.padding = "10px";
  recordDiv.style.width = "100%";
  document.body.appendChild(recordDiv);
}

function constructGeneralizedEquation(record) {
  let result = "";
  result = Array.from(
    { length: record.Substances.length },
    (_, index) => record.Labels[index] + INTERPUNCT + record.Substances[index]
  ).join(" + ");
  return result + " = 0";
}

function createTable(
  data,
  rowLabelFunc,
  labelColumnHeader,
  labelColumnFooter = null
) {
  const table = document.createElement("table");
  table.classList.add("matrix");

  const tableHead = document.createElement("thead");
  const tableRowOfColumnHeaders = document.createElement("tr");
  tableRowOfColumnHeaders.appendChild(document.createElement("th"));
  for (let index = 0; index < data[0].length; index++) {
    const currentHeader = document.createElement("th");
    currentHeader.innerHTML = labelColumnHeader(index);
    tableRowOfColumnHeaders.appendChild(currentHeader);
  }
  tableHead.appendChild(tableRowOfColumnHeaders);
  table.appendChild(tableHead);

  const tableBody = document.createElement("tbody");
  data.forEach((dataRow, dataRowIndex) => {
    const currentRow = document.createElement("tr");
    const currentRowHeaderCell = document.createElement("td");
    currentRowHeaderCell.innerHTML = rowLabelFunc(dataRowIndex);
    currentRow.appendChild(currentRowHeaderCell);
    dataRow.forEach((entry) => {
      const currentCell = document.createElement("td");
      currentCell.innerHTML = entry;
      if (entry === "0") {
        currentCell.style.color = "lightgrey";
      }
      currentCell.style.border = "1px dotted grey";
      currentRow.appendChild(currentCell);
    });
    tableBody.appendChild(currentRow);
  });
  table.appendChild(tableBody);

  if (labelColumnFooter != null) {
    const tableFoot = document.createElement("tfoot");
    const tableRowOfColumnFooters = document.createElement("tr");
    tableRowOfColumnFooters.appendChild(document.createElement("th"));
    for (let index = 0; index < data[0].length; index++) {
      const cell = document.createElement("th");
      cell.innerHTML = labelColumnFooter(index);
      tableRowOfColumnFooters.appendChild(cell);
    }
    tableFoot.appendChild(tableRowOfColumnFooters);
    table.appendChild(tableFoot);
  }

  return table;
}

function AssembleEquationString(substances, coefs) {
  const lhs = [];
  const rhs = [];

  for (let i = 0; i < coefs.length; i++) {
    if (coefs[i] == 0) {
      continue;
    }

    console.debug(coefs[i]);
    let token = coefs[i] == 1 || coefs[i] == -1? "" : Math.abs(coefs[i]);
    if (token !== "") {
      token += INTERPUNCT;
    }

    ((coefs[i] > 0) ? rhs : lhs).push(token + substances[i]);
  }

  return lhs.join(" + ") + " = " + rhs.join(" + ");
}
