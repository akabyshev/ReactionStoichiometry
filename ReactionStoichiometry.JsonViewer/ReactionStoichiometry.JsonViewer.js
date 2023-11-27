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
  ).outerHTML;

  const tableRREF = createTable(
    Equation.RREF,
    (index) => "R" + (index + 1).toString(),
    (index) => Equation.Labels[index]
  ).outerHTML;

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
      <p>A chemical composition matrix (CCM) is constructed: ${tableCCM}</p>
      <p>Then we get CCM in reduced row echelon form (RREF) using <u>Gaussian elimination</u>: ${tableRREF}</p>
  `;

  recordDiv.innerHTML += `
    <div class="multiple-columns-container">
      <div class="first-column">
        <h3>Rows-based approach</h3>
        ${report_RBS(Equation)}
      </div>
      <div class="second-column">
        <h3>Columns-based approach</h3>
        ${report_CBS(Equation)}
      </div>
    </div>`;

  if (Equation.RowsBasedSolution.Success) {
    recordDiv.innerHTML += `<h3>Solution</h3>`;
    if (Equation.RowsBasedSolution.FreeVariableIndices.length === 1) {
      recordDiv.innerHTML += `<p class="cre">${AssembleEquationString(
        Equation.Substances,
        Equation.RowsBasedSolution.InstanceSample
      )}</p>`;
    } else {
      recordDiv.innerHTML += `There are infinitely many solutions`;
    }
  }
  document.body.appendChild(recordDiv);
}

function report_RBS(Equation) {
  solution = Equation.RowsBasedSolution;

  if (solution.Success === false) {
    return `<p>${solution.FailureMessage}</p>`;
  } else {
    var result = "";
    result += `<p>RREF interpretation: no new atoms are created, so each row sum <b>must be</b> equal to zero.</p>`;

    var content2A1, content2A2, sample_RBS;

    content2A1 = `This interpretation forms a system of linear equations:
    ${
      createTable(
        Equation.RREF,
        (index) => "0" + " = ",
        (index) => Equation.Labels[index]
      ).outerHTML
    }`;

    content2A2 = `We see how all coefficients are a function of ${
      solution.FreeVariableIndices.length
    } ${
      solution.FreeVariableIndices.length > 1
        ? "free variables"
        : "free variable"
    }:
    ${
      createTable(
        solution.AlgebraicExpressions.map((item) =>
          item.includes("=") ? [item.split("=")[1].trim()] : ["<i>free</i>"]
        ),
        (index) => Equation.Labels[index] + " =",
        () => "Expression"
      ).outerHTML
    }`;

    if (solution.FreeVariableIndices.length === 1) {
      content2A2 += `The free variable equal to <u>the lowest common multiple among the denominators of non-zero expressions</u> produces all-integer solution of:
      ${
        createTable(
          [solution.InstanceSample],
          (index) => 0 + " = ",
          (index) => Equation.Substances[index]
        ).outerHTML
      }`;
    } else {
      sample_RBS = `For example, instantiating with mutually equal free variables produces:
      ${
        createTable(
          [solution.InstanceSample],
          (index) => "0" + " = ",
          (index) => Equation.Substances[index]
        ).outerHTML
      }
      <p class="cre">${AssembleEquationString(
        Equation.Substances,
        Equation.RowsBasedSolution.InstanceSample
      )}</p>`;
    }

    result += `
        ${content2A1}
        ${content2A2}
    `;

    if (sample_RBS) result += sample_RBS;

    return result;
  }
}

function report_CBS(Equation) {
  solution = Equation.ColumnsBasedSolution;
  if (solution.Success === false) {
    return `<p>${solution.FailureMessage}</p>`;
  } else {
    var result = "";
    result += `<p>RREF interpretation: the reaction <b>must be</b> a combination of equilibriums</p>`;

    var content2B1, content2B2, sample_CBS;

    content2B1 = `Look at columns of modified RREF:
    ${
      createTable(
        solution.InverseMatrix,
        (index) => Equation.Substances[index],
        (index) => "&nbsp;&nbsp;&nbsp;",
        (index) => "0" + " = "
      ).outerHTML
    }`;

    content2B2 = `
      Equilibriums after being scaled to integers:
      ${
        createTable(
          solution.IndependentSetsOfCoefficients,
          (_index) => "0" + " = ",
          (index) => Equation.Substances[index]
        ).outerHTML
      }
      `;

    if (solution.IndependentSetsOfCoefficients.length === 1) {
      content2B2 += `In this case, this vector uniquely defines the solution to the entire equation`;
    } else {
      content2B2 += solution.IndependentSetsOfCoefficients.map(
        (vector) =>
          '<p class="cre">' +
          AssembleEquationString(Equation.Substances, vector, true) +
          "</p>"
      ).join("");
      if (solution.CombinationSample.Item2) {
        sample_CBS = `For example, <u>(${solution.CombinationSample.Item1}) 
            combination</u> of those yields:
            <p class="cre">${AssembleEquationString(
              Equation.Substances,
              solution.CombinationSample.Item2
            )}`;
      }
    }

    result += `
          ${content2B1}
          ${content2B2}
      `;
    if (sample_CBS) result += sample_CBS;

    return result;
  }
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
