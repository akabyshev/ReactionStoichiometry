function MakeJsonReadable(serialized, identifier) {
  const recordDiv = document.createElement("div");

  serialized.Substances = serialized.Substances.map(
    (substance) =>
      "<div>" + substance.replace(/(\d+(\.\d+)?)/g, "<sub>$1</sub>") + "</div>"
  );

  const tableCCM = createTable(
    serialized.CCM,
    (index) => (index + 1).toString(),
    (index) => serialized.Substances[index]
  );
  tableCCM.classList.add("vertical-headers");
  const tableRREF = createTable(
    serialized.RREF,
    (index) => labelFor(serialized.Substances.length, index),
    (index) => labelFor(serialized.Substances.length, index)
  );

  recordDiv.innerHTML = `
    <h3>${identifier}</h3>
    <p>
    We begin by expressing the original equation
    <div class="cre">${serialized["Original input"]}</div>
    in its generalized form
    <div class="cre">${constructGeneralizedEquation(serialized)}</div>
    </p>
    <p>Following this, a chemical composition matrix (CCM) is constructed.
    The matrix columns represent the substances involved in the reaction, while the rows indicate the constituent chemical elements: ${
      tableCCM.outerHTML
    }</p>
    <p>Subsequently, the CCM is transformed into its Reduced Row Echelon Form (RREF) using Gaussian elimination: ${
      tableRREF.outerHTML
    }</p>`;

  if (serialized.Solutions.Generalized.Success === false) {
    recordDiv.innerHTML += `
      <p>That results in an identity matrix, indicating that it's impossible to balance the provided equation.</p>
      `;
  } else {
    const tableExpressions = createTable(
      serialized.Solutions.Generalized["Algebraic expressions"].filter(item => item !== null).map((item) => [item]),
      (index) => (index + 1).toString(),
      () => "Expression"
    );
    recordDiv.innerHTML += `
      <p>
      The RREF demonstrates how all coefficients can be expressed as linear functions of ${
        serialized.Solutions.Generalized["Free variables"].length > 1
          ? "free variables"
          : "the free variable"
      }
        <b>${serialized.Solutions.Generalized["Free variables"]
          .map((i) => labelFor(serialized.Substances.length, i))
          .join(", ")}
        </b>:${tableExpressions.outerHTML}
      </p>`;
    if (serialized.Solutions.Generalized["Free variables"].length === 1) {
      recordDiv.innerHTML += `The obtained generalized solution leads us to the next step: 
      determining values for the free variable that ensure all coefficients are integers. 
      Setting it to the least common multiple of all the denominators, <b> ${labelFor(
        serialized.Substances.length,
        serialized.Solutions.Generalized["Free variables"][0]
      )} = ${serialized.Solutions.Generalized["Simplest solution"]} </b> yields the final solution:
    <div class="cre">BHAHAHA</div>
    `;
    } else {
      recordDiv.innerHTML += `There is an infinite number of solutions in this case. Discover a solution instance by utilizing a calculator, Excel, or the 'Instantiation' feature in our GUI. 
      Negative coefficients indicate a reactant, positive values denote produced substances. A coefficient of zero signifies that the substance is not essential for the reaction.
      `;
    }
  }

  recordDiv.style.border = "1px solid black";
  recordDiv.style.padding = "10px";
  recordDiv.style.width = "100%";
  document.body.appendChild(recordDiv);
}

function labelFor(total, i) {
  return "<i>" + (total > 7
    ? "x" + "<sub>" + (i + 1).toString().padStart(2, "0") + "</sub>" // could be Math.ceil(Math.log10(total + 1)) but...
    : String.fromCharCode("a".charCodeAt(0) + i))
    + "</i>";
}

function constructGeneralizedEquation(record) {
  const INTERPUNCT = "\u00B7";
  let result = "";
  result = Array.from(
    { length: record.Substances.length },
    (_, index) =>
      "<div>" +
      labelFor(record.Substances.length, index) +
      "</div>" +
      INTERPUNCT +
      record.Substances[index]
  ).join(" + ");
  return result + " = <div>0</div>";
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
      currentCell.textContent = entry;
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
