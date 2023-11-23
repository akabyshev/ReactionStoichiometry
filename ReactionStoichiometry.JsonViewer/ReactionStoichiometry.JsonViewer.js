const INTERPUNCT = "\u00B7";

function MakeJsonReadable(Equation, identifier) {
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

  const recordDiv = document.createElement("div");
  recordDiv.style.border = "1px solid black";
  recordDiv.style.padding = "10px";
  recordDiv.style.width = "100%";
  recordDiv.innerHTML = `<h3>${identifier}</h3>`;
  recordDiv.innerHTML += `
  <div class="multiple-columns-container">
    <div class="first-column">
      We begin by expressing the original input
      <p class="cre">${Equation.OriginalEquationString.replace('=', ' = ')}</p>
    </div>
    <div class="second-column">
      in its generalized form of
      <p class="cre">${constructGeneralizedEquation(Equation)}</p>
    </div>
  </div>`;

  var thirdMatrix;
  if (Equation.GeneralizedSolution.Success === false) {
    thirdMatrix = `That results in an identity matrix, indicating that <u>it's impossible to balance the equation</u>.`;
  } else {
    Equation.GeneralizedSolution.FreeVariableIndices.forEach((index) => Equation.Labels[index] = `<mark>${Equation.Labels[index]}</mark>`);
    thirdMatrix = `The RREF shows how all coefficients can be expressed via <u>${
      Equation.GeneralizedSolution.FreeVariableIndices.length > 1
        ? "free variables"
        : "the free variable"
    }</u>
    ${Equation.GeneralizedSolution.FreeVariableIndices.map(
      (index) => Equation.Labels[index]
    ).join(", ")}:
    ${
      createTable(
        Equation.GeneralizedSolution.AlgebraicExpressions.map((item) => item.includes('=')?[item.split("=")[1].trim()]:["<i>free</i>"]),
        (index) => Equation.Labels[index] + ' =',
        () => "Expression"
      ).outerHTML
    }
    `;
  }

  recordDiv.innerHTML += `
  <div class="multiple-columns-container">
    <div class="first-column">
      Following this, a chemical composition matrix (CCM) is constructed: ${tableCCM.outerHTML}
    </div>
    <div class="second-column">
      Then we get CCM in reduced row echelon form (RREF) using <u>Gaussian elimination</u>: ${tableRREF.outerHTML}
    </div>
    <div class="third-column">
      ${thirdMatrix}
    </div>
  </div>`;

  if (Equation.GeneralizedSolution.Success === true) {
    if (Equation.GeneralizedSolution.FreeVariableIndices.length === 1) {
      recordDiv.innerHTML += `
      <div class="multiple-columns-container">
        <div class="first-column">
        Establishing the free variable as <u>the lowest common multiple among the denominators of non-zero expressions</u> produces: ${
        createTable(
          Equation.Labels.map((item, index) => [
            Equation.GeneralizedSolution.SimplestSolution[index],
          ]),
          (index) => Equation.Labels[index] + " = ",
          () => "Value"
        ).outerHTML
      }
        </div>
        <div class="second-column">
        So the final solution is <p class="cre">${AssembleEquationString(
          Equation.Substances,
          Equation.GeneralizedSolution.SimplestSolution
        )}
        </div>
      </div>
      `;
    } else {
      var firstResult, secondResult;
      if (Equation.InverseBasedSolution.Success) {
        tableInverseBasedReactions = createTable(
          Equation.InverseBasedSolution.IndependentSetsOfCoefficients,
          (index) => "0" + " = ",
          (index) => Equation.Substances[index]
        );
        firstResult = `Any balancing solution is a combination of the following 'independent reactions': ${tableInverseBasedReactions.outerHTML}
        ${Equation.InverseBasedSolution.IndependentSetsOfCoefficients.map((vector) =>
          `<p class="cre">${AssembleEquationString(Equation.Substances, vector)}`)}`;

        if (Equation.InverseBasedSolution.CombinationSample.Item2) {
          const tableCombination = createTable(
            Equation.Labels.map((item, index) => [
              Equation.InverseBasedSolution.CombinationSample.Item2[index],
            ]),
            (index) => Equation.Labels[index] + " = ",
            () => "Value"
          );
          secondResult = `For example, <u>(${Equation.InverseBasedSolution.CombinationSample.Item1}) 
          combination</u> of those yields a solution: ${tableCombination.outerHTML}
          <p class="cre">${AssembleEquationString(
            Equation.Substances,
            Equation.InverseBasedSolution.CombinationSample.Item2
          )}`;
        }
        else
        {
          secondResult = `Use 'Instantiation tool' of the GUI`
        }
      }
      recordDiv.innerHTML += `
      <div class="multiple-columns-container">
        <div class="first-column">
          ${firstResult}
        </div>
        <div class="second-column">
          ${secondResult}
        </div>
      </div>
      `;
    }
  }

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

    let token = coefs[i] == 1 || coefs[i] == -1 ? "" : Math.abs(coefs[i]);
    if (token !== "") {
      token += INTERPUNCT;
    }

    (coefs[i] > 0 ? rhs : lhs).push(token + substances[i]);
  }

  return lhs.join(" + ") + " = " + rhs.join(" + ");
}
