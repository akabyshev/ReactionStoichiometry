namespace ReactionStoichiometry.GUI;

public partial class RisteskiInstantiatorForm : Form
{
    internal IBalancerInstantiatable? Balancer;

    public RisteskiInstantiatorForm() => InitializeComponent();

    internal void InitRisteskiTable()
    {
        if (Balancer == null) return;
        dataGridView1.Rows.Clear();
        dataGridView1.RowCount = Balancer.FragmentsCount;
        for (var i = 0; i < Balancer.FragmentsCount; i++)
        {
            dataGridView1.Rows[i].HeaderCell.Value = Balancer.LabelFor(i);
            dataGridView1.Rows[i].Cells["Fragment"].Value = Balancer.Fragment(i);

            if (Balancer.GetCoefficientExpression(i) == String.Empty)
            {
                dataGridView1.Rows[i].Cells["Value"].ReadOnly = false;
                dataGridView1.Rows[i].Cells["Value"].Value = 1;
                dataGridView1.Rows[i].Cells["IsFreeVariable"].Value = true;
            }
            else
            {
                dataGridView1.Rows[i].Cells["Value"].Value = Balancer.GetCoefficientExpression(i);
                dataGridView1.Rows[i].Cells["IsFreeVariable"].Value = false;
            }
        }

        dataGridView1.Refresh();

        UpdateTable();
    }

    private void UpdateTable()
    {
        if (Balancer == null) return;

        try
        {
            var parameters = new List<Int64>();
            for (var i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (!Boolean.Parse(dataGridView1.Rows[i].Cells["IsFreeVariable"].Value.ToString()!)) continue;

                var cellValue = dataGridView1.Rows[i].Cells["Value"].Value.ToString();
                if (Int64.TryParse(cellValue, out var parsedValue))
                {
                    parameters.Add(parsedValue);
                }
                else
                {
                    txtInstance.Text = "Parsing error occurred";
                    return;
                }
            }

            txtInstance.Text = Balancer.Instantiate(parameters.ToArray());
        } catch (InvalidOperationException)
        {
            txtInstance.Text = "Could not get integer coefficients";
        }
    }

    private void On_dataGridView1_CellEndEdit(Object sender, DataGridViewCellEventArgs e) => UpdateTable();

    private void On_txtInstance_TextChanged(Object sender, EventArgs e)
    {
        var size = TextRenderer.MeasureText(txtInstance.Text, txtInstance.Font);
        Width = size.Width + 25;
        Height = txtInstance.Height * (1 + Balancer!.FragmentsCount + 1) + 120;
    }
}