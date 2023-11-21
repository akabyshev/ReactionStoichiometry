function MakeJsonReadable(record, identifier) {
  const recordDiv = document.createElement("div");

  record.Substances = record.Substances.map((substance) =>
    substance.replace(/(\d+(\.\d+)?)/g, "<sub>$1</sub>")
  );

  record.Labels = record.Labels.map(
    (label) => label.replace(/(\d+(\.\d+)?)/g, "<sub>$1</sub>")
  );

  if (record.GeneralizedSolution.AlgebraicExpressions) {
    record.GeneralizedSolution.AlgebraicExpressions =
      record.GeneralizedSolution.AlgebraicExpressions.map((expression) =>
        expression.replace(/(?<=x)\d{2}/g, "<sub>$&</sub>")
      );
  }

  const tableCCM = createTable(
    record.CCM,
    (index) => record.Elements[index],
    (index) => record.Substances[index],
    true
  );

  const tableRREF = createTable(
    record.RREF,
    (index) => '0' + ' = ',
    (index) => record.Labels[index]
  );

  recordDiv.innerHTML = `
    <h3>${identifier}</h3>
    <p>
    We begin by expressing the original equation
    <p class="cre">${record.OriginalEquationString}</p>
    in its generalized form
    <p class="cre">${constructGeneralizedEquation(record)}</p>
    </p>
    <p>Following this, a chemical composition matrix (CCM) is constructed.
    The matrix columns represent the substances involved in the reaction, while the rows indicate the constituent chemical elements: ${
      tableCCM.outerHTML
    }</p>
    <p>The CCM is transformed into its reduced row echelon form (RREF) using Gaussian elimination: ${
      tableRREF.outerHTML
    }</p>`;

  if (record.GeneralizedSolution.Success === false) {
    recordDiv.innerHTML += `
      <p>That results in an identity matrix, indicating that it's impossible to balance the provided equation.</p>
      `;
  } else {
    const tableExpressions = createTable(
      record.GeneralizedSolution.AlgebraicExpressions.filter((item) =>
        item.includes(" = ")
      ).map((item) => [item.split('=')[1].trim()]),
      (index) => record.Labels[index] + ' = ',
      () => "Expression"
    );

    recordDiv.innerHTML += `
      <p>
      The RREF demonstrates how all coefficients can be expressed as linear functions of <u>${
        record.GeneralizedSolution.FreeVariableIndices.length > 1
          ? "free variables"
          : "the free variable"
      }
        ${record.GeneralizedSolution.FreeVariableIndices.map(
          (index) => record.Labels[index]
        ).join(", ")}</u>:
        ${tableExpressions.outerHTML}
      </p>`;

    if (record.GeneralizedSolution.FreeVariableIndices.length === 1) {
      const tableFoundSolution = createTable(
        record.Labels.map((item, index) => [
          record.GeneralizedSolution.SimplestSolution[index],
        ]),
        (index) => record.Labels[index] + ' = ',
        () => "Value"
      );

      recordDiv.innerHTML += `<p>The next step is determining a value of the free variable that yields integer coefficients - 
      ${record.Labels[record.GeneralizedSolution.FreeVariableIndices[0]]} equal to <u>the least common multiple of all the denominators</u> results in: ${tableFoundSolution.outerHTML}</p>
      `;
    } else {
      if (record.InverseBasedSolution.Success) {
        const tableSubreactions = createTable(
          record.InverseBasedSolution.IndependentReactions,
          (index) => (index + 1).toString(),
          (index) => record.Substances[index]
        );
        recordDiv.innerHTML += `<p>Any integer solutions will be a linear combination of these 'subreactions': ${tableSubreactions.outerHTML} </p>`;

        if (record.InverseBasedSolution.CombinationSample.Item2) {
          const tableCombination = createTable(
            record.Labels.map((item, index) => [
                record.InverseBasedSolution.CombinationSample.Item2[index],
            ]),
            (index) => record.Labels[index] + ' = ',
            () => "Value"
          );
          recordDiv.innerHTML += `</p>For example, ${
            "(" +
            record.InverseBasedSolution.CombinationSample.Item1.join(", ") +
            ")"
          } combination of those yields ${tableCombination.outerHTML}</p>`;
        }
      } else {
        recordDiv.innerHTML += `<p>Discover a solution instance by utilizing a calculator, Excel, or the 'Instantiation' feature in our GUI. </p>`;
      }
    }

    recordDiv.innerHTML += `Negative coefficients indicate a reactant, positive values denote produced substances. A coefficient of zero signifies that the substance is not a part of the reaction.`;
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
    (_, index) => record.Labels[index] + INTERPUNCT + record.Substances[index]
  ).join(" + ");
  return result + " = 0";
}

function createTable(data, rowLabelFunc, columnLabelFunc, matrix = true) {
  const table = document.createElement("table");
  table.classList.add("matrix");

  const tableHead = document.createElement("thead");
  const tableRowOfColumnHeaders = document.createElement("tr");
  tableRowOfColumnHeaders.appendChild(document.createElement("th"));
  for (let index = 0; index < data[0].length; index++) {
    const currentHeader = document.createElement("th");
    currentHeader.innerHTML = columnLabelFunc(index);
    tableRowOfColumnHeaders.appendChild(currentHeader);
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
