function MakeJsonReadable(record, identifier) {
  const recordDiv = document.createElement("div");

  record.Substances = record.Substances.map(
    (substance) =>
      substance.replace(/(\d+(\.\d+)?)/g, "<sub>$1</sub>")
  );

  record.Labels = record.Labels.map(
    (label) => "<i>" + label.replace(/(\d+(\.\d+)?)/g, "<sub>$1</sub>") + "</i>"
  );

  if (record.Solutions.Generalized.AlgebraicExpressions) {
    record.Solutions.Generalized.AlgebraicExpressions =
      record.Solutions.Generalized.AlgebraicExpressions.map((expression) =>
        expression.replace(/(?<=x)\d{2}/g, "<sub>$&</sub>")
      );
  }

  const tableCCM = createTable(
    record.CCM,
    (index) => (index + 1).toString(),
    (index) => record.Labels[index]
  );
  tableCCM.classList.add("vertical-headers");

  const tableRREF = createTable(
    record.RREF,
    (index) => record.Labels[index],
    (index) => record.Labels[index]
  );

  recordDiv.innerHTML = `
    <h3>${identifier}</h3>
    <p>
    We begin by expressing the original equation
    <p class="cre">${record.OriginalInput}</p>
    in its generalized form
    <p class="cre">${constructGeneralizedEquation(record)}</p>
    </p>
    <p>Following this, a chemical composition matrix (CCM) is constructed.
    The matrix columns represent the substances involved in the reaction, while the rows indicate the constituent chemical elements: ${
      tableCCM.outerHTML
    }</p>
    <p>Subsequently, the CCM is transformed into its reduced row echelon form (RREF) using Gaussian elimination: ${
      tableRREF.outerHTML
    }</p>`;

  if (record.Solutions.Generalized.Success === false) {
    recordDiv.innerHTML += `
      <p>That results in an identity matrix, indicating that it's impossible to balance the provided equation.</p>
      `;
  } else {
    const tableExpressions = createTable(
      record.Solutions.Generalized.AlgebraicExpressions.filter((item) =>
        item.includes(" = ")
      ).map((item) => [item]),
      (index) => (index + 1).toString(),
      () => "Expression"
    );

    recordDiv.innerHTML += `
      <p>
      The RREF demonstrates how all coefficients can be expressed as linear functions of <u>${
        record.Solutions.Generalized.FreeVariableIndices.length > 1
          ? "free variables"
          : "the free variable"
      }
        ${record.Solutions.Generalized.FreeVariableIndices.map(
          (index) => record.Labels[index]
        ).join(", ")}</u>:
        ${tableExpressions.outerHTML}
      </p>`;

    if (record.Solutions.Generalized.FreeVariableIndices.length === 1) {
      const tableFoundSolution = createTable(
        record.Labels.map((item, index) => [
          item + " = " + record.Solutions.Generalized.SimplestSolution[index],
        ]),
        (index) => (index + 1).toString(),
        () => "Coefficients"
      );

      recordDiv.innerHTML += `<p>The next step is determining values for the free variable that yields integer coefficients. 
      Setting it to <u>the least common multiple of all the denominators</u> results in: ${tableFoundSolution.outerHTML}</p>
      `;
    } else {
      recordDiv.innerHTML += `<p> There is an infinite number of solutions in this case. 
      Discover a solution instance by utilizing a calculator, Excel, or the 'Instantiation' feature in our GUI. </p>
      `;
    }

    recordDiv.innerHTML += `Negative coefficients indicate a reactant, positive values denote produced substances. A coefficient of zero signifies that the substance is not essential for the reaction.`
  }

  recordDiv.style.border = "1px solid black";
  recordDiv.style.padding = "10px";
  recordDiv.style.width = "100%";
  document.body.appendChild(recordDiv);
}

function constructGeneralizedEquation(record) {
  const INTERPUNCT = "\u00B7";
  let result = "";
  result = Array.from(
    { length: record.Substances.length },
    (_, index) =>
      record.Labels[index] +
      INTERPUNCT +
      record.Substances[index]
  ).join(" + ");
  return result + " = 0";
}

function createTable(data, rowLabelFunc, columnLabelFunc) {
  const table = document.createElement("table");

  const tableHead = document.createElement("thead");
  const tableRowOfColumnHeaders = document.createElement("tr");
  tableRowOfColumnHeaders.appendChild(document.createElement("th"));
  for (let index = 0; index < data[0].length; index++) {
    const currentCell = document.createElement("th");
    currentCell.innerHTML = columnLabelFunc(index);
    tableRowOfColumnHeaders.appendChild(currentCell);
  }
  tableHead.appendChild(tableRowOfColumnHeaders);

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

  table.appendChild(tableHead);
  table.appendChild(tableBody);
  return table;
}
