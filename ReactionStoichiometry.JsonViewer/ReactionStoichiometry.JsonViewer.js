const INTERPUNCT = "\u00B7";
const EQUILIBRIUM = "\u21CC";

function MakeJsonReadable(Equation, identifier) {
  Equation.Substances = Equation.Substances.map((substance) =>
    substance.replace(/(\d+(\.\d+)?)/g, "<sub>$1</sub>")
  );

  Equation.Labels = Equation.Labels.map((label) =>
    label.replace(/(\d+(\.\d+)?)/g, "<sub>$1</sub>")
  );

  if (Equation.RowsBasedSolution.AlgebraicExpressions) {
    Equation.RowsBasedSolution.AlgebraicExpressions =
      Equation.RowsBasedSolution.AlgebraicExpressions.map((expression) =>
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
    (index) => Equation.Elements[index],
    (index) => Equation.Labels[index]
  );

  const recordDiv = document.createElement("div");
  recordDiv.style.border = "1px solid black";
  recordDiv.style.padding = "10px";
  recordDiv.style.width = "100%";
  recordDiv.innerHTML = `<h2>${identifier}</h3>`;
  recordDiv.innerHTML += `<h3>Generalization</h3>`;
  recordDiv.innerHTML += `
      We begin by expressing the original input "${
        Equation.OriginalEquationString
      }" in its generalized form:
      <p class="cre">${constructGeneralizedEquation(Equation)}</p>
  `;
  recordDiv.innerHTML += `<h3>Matrices</h3>`;
  recordDiv.innerHTML += `
  <div class="multiple-columns-container">
    <div class="first-column">
      Following this, a chemical composition matrix (CCM) is constructed: ${tableCCM.outerHTML}
    </div>
    <div class="second-column">
      Then we get CCM in reduced row echelon form (RREF) using <u>Gaussian elimination</u>: ${tableRREF.outerHTML}
    </div>
  </div>`;

  recordDiv.innerHTML += `<h3>Rows-based solution</h3>`;

  if (Equation.RowsBasedSolution.Success === false) {
    recordDiv.innerHTML += `<p>${Equation.RowsBasedSolution.FailureMessage}</p>`;
  } else
  {
    recordDiv.innerHTML += `<p>RREF interpretation: no new atoms are created, so each row sum <b>must be</b> equal to zero.</p>`;

    var content2A1, content2A2, content2A3;

    content2A1 = `This interpretation forms a system of linear equations:
    ${
      createTable(
        Equation.RREF,
        (index) => "0" + " = ",
        (index) => Equation.Labels[index]
      ).outerHTML
    }`;

    content2A2 = `We see how all coefficients can be expressed via ${
      Equation.RowsBasedSolution.FreeVariableIndices.length
    } ${
      Equation.RowsBasedSolution.FreeVariableIndices.length > 1
        ? "free variables"
        : "free variable"
    }:
    ${
      createTable(
        Equation.RowsBasedSolution.AlgebraicExpressions.map((item) =>
          item.includes("=") ? [item.split("=")[1].trim()] : ["<i>free</i>"]
        ),
        (index) => Equation.Labels[index] + " =",
        () => "Expression"
      ).outerHTML
    }`;

    if (Equation.RowsBasedSolution.FreeVariableIndices.length === 1) {
      content2A3 = `The free variable equal to <u>the lowest common multiple among the denominators of non-zero expressions</u> produces all-integer solution of:
      ${
        createTable(
          Equation.Labels.map((_item, index) => [
            Equation.RowsBasedSolution.SimplestSolution[index],
          ]),
          (index) => Equation.Labels[index] + " = ",
          () => "Value"
        ).outerHTML
      }`;

      THE_SOLUTION = `
      <p class="cre">${AssembleEquationString(
        Equation.Substances,
        Equation.RowsBasedSolution.SimplestSolution
      )}
      `;
    } else {
      content2A3 = `There are infinitely many unique solutions. For example: <mark>FREE VARS = (1,1) => TODO</mark>`;
    }

    recordDiv.innerHTML += `
    <div class="multiple-columns-container">
      <div class="first-column">
        ${content2A1}
      </div>
      <div class="second-column">
        ${content2A2}
      </div>
      <div class="third-column">
        ${content2A3}
      </div>
    </div>`;
  }

  recordDiv.innerHTML += `<h3>Columns-based solution</h3>`;

  if (Equation.ColumnsBasedSolution.Success === false) {
    recordDiv.innerHTML += `<p>${Equation.ColumnsBasedSolution.FailureMessage}</p>`;
  } else
  {
    recordDiv.innerHTML += `<p>RREF interpretation: the reaction <b>must be</b> a combination of equilibriums</p>`;

    var content2B1, content2B2, content2B3;

    content2B1 = `Look at columns of the inverse of augmented RREF:
    ${
      createTable(
        Equation.ColumnsBasedSolution.InverseMatrix,
        (index) => Equation.Substances[index],
        (index) => "&nbsp;&nbsp;&nbsp;",
        (index) => "0" + " = "
      ).outerHTML
    }`;

    content2B2 = `
      We find following equilibriums after scaling to integers:
      ${
        createTable(
          Equation.ColumnsBasedSolution.IndependentSetsOfCoefficients,
          (_index) => "0" + " = ",
          (index) => Equation.Substances[index]
        ).outerHTML
      }
      ${Equation.ColumnsBasedSolution.IndependentSetsOfCoefficients.map(
        (vector) =>
          '<p class="cre">' +
          AssembleEquationString(Equation.Substances, vector, true) +
          "</p>"
      ).join("")}
      `;

    if (
      Equation.ColumnsBasedSolution.IndependentSetsOfCoefficients.length === 1
    ) {
      content2B3 = null;
    } else {
      if (Equation.ColumnsBasedSolution.CombinationSample.Item2) {
        content2B3 = `There are infinitely many unique solutions. For example, <u>(${
          Equation.ColumnsBasedSolution.CombinationSample.Item1
        }) 
            combination</u> of those yields a solution: ${
              createTable(
                Equation.Labels.map((_item, index) => [
                  Equation.ColumnsBasedSolution.CombinationSample.Item2[index],
                ]),
                (index) => Equation.Labels[index] + " = ",
                () => "Value"
              ).outerHTML
            }
            <p class="cre">${AssembleEquationString(
              Equation.Substances,
              Equation.ColumnsBasedSolution.CombinationSample.Item2
            )}`;
      } else {
        content2B3 = `We couldn't find a simple combination of those. Use 'Combine' tool in the GUI`;
      }
    }

    if (content2B3 === null) {
      recordDiv.innerHTML += `
      <div class="multiple-columns-container">
        <div class="first-column">
          ${content2B1}
        </div>
        <div class="second-column">
          ${content2B2}
        </div>
      </div>`;
    }
    else
      recordDiv.innerHTML += `
      <div class="multiple-columns-container">
        <div class="first-column">
          ${content2B1}
        </div>
        <div class="second-column">
          ${content2B2}
        </div>
        <div class="third-column">
          ${content2B3}
        </div>
      </div>`;    
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

function AssembleEquationString(substances, coefs, useEquilibriumSign = false) {
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

  return (
    lhs.join(" + ") +
    " " +
    (useEquilibriumSign ? EQUILIBRIUM : "=") +
    " " +
    rhs.join(" + ")
  );
}
