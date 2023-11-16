function MakeJsonReadable(serialized, identifier) {
  const recordDiv = document.createElement("div");

  serialized.Equation.Substances = serialized.Equation.Substances.map(
    (substance) =>
      "<div>" + substance.replace(/(\d+(\.\d+)?)/g, "<sub>$1</sub>") + "</div>"
  );

  const tableCCM = createTable(
    serialized.Equation.CCM,
    (index) => (index + 1).toString(),
    (index) => serialized.Equation.Substances[index]
  );
  tableCCM.classList.add("vertical-headers");
  const tableRREF = createTable(
    serialized.Equation.RREF,
    (index) => labelFor(serialized.Equation.Substances.length, index),
    (index) => labelFor(serialized.Equation.Substances.length, index)
  );

  recordDiv.innerHTML = `
    <h3>${identifier}</h3>
    <p>
    We begin by transforming the original equation
    <div class="cre">${serialized.Equation["Original input"]}</div>
    into its generalized form
    <div class="cre">${constructGeneralizedEquation(serialized)}</div>
    </p>
    <p>Following that, we create a chemical composition matrix (CCM).
    The matrix columns denote the substances engaged in the reaction (either as reactants or products),
    with the matrix rows indicating the individual chemical elements that compose those substances: ${
      tableCCM.outerHTML
    }</p>
    <p>Subsequently, we convert the CCM into its Reduced Row Echelon Form (RREF) through Gaussian elimination: ${
      tableRREF.outerHTML
    }</p>`;

  if (serialized.Success === false) {
    recordDiv.innerHTML += `
      <p>RREF turns out to be an identity matrix, so balancing the given equation is impossible</p>
      `;
  } else {
    const tableExpressions = createTable(
      serialized["Algebraic expressions"].map((item) => [item]),
      (index) => (index + 1).toString(),
      () => "Expression"
    );
    recordDiv.innerHTML += `
      <p>
      RREF shows that all coefficients can be expressed as linear functions of ${
        serialized["Free variables"].length > 1
          ? "free variables"
          : "the free variable"
      }
        <b>${serialized["Free variables"]
          .map((i) => labelFor(serialized.Equation.Substances.length, i))
          .join(", ")}
        </b>:${tableExpressions.outerHTML}
      </p>`;
    if (serialized["Free variables"].length === 1) {
      recordDiv.innerHTML += `That provides the generalized solution we sought. To obtain a simpler solution,
      the next step involves identifying values for the free variable that yield coefficients with all-integer values.
      In this case ${labelFor(
        serialized.Equation.Substances.length,
        serialized["Free variables"][0]
      )} = ${serialized["Simplest solution"].Item1} works, and produces
    <div class="cre">${serialized["Simplest solution"].Item2}</div>
    `;
    } else {
      recordDiv.innerHTML += `Discover a solution instance by utilizing a calculator, Excel, or the 'Instantiation' feature in our GUI. 
        Negative coefficients indicate that the substance functions as a reactant (is consumed), positive values denote produced substances. 
        A coefficient of zero signifies that the substance is not essential for the reaction.`;
    }
  }

  recordDiv.style.border = "1px solid black";
  recordDiv.style.padding = "10px";
  recordDiv.style.width = "100%";
  document.body.appendChild(recordDiv);
}

function labelFor(total, i) {
  return total > 7
    ? "x" + (i + 1).toString().padStart(2, "0")
    : String.fromCharCode("a".charCodeAt(0) + i);
}

function constructGeneralizedEquation(record) {
  const INTERPUNCT = "\u00B7";
  let result = "";
  result = Array.from(
    { length: record.Equation.Substances.length },
    (_, index) =>
      "<div>" +
      labelFor(record.Equation.Substances.length, index) +
      "</div>" +
      INTERPUNCT +
      record.Equation.Substances[index]
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
    currentRowHeaderCell.innerText = rowLabelFunc(dataRowIndex);
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
