using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp1.Controllers;
using WindowsFormsApp1.models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1.views
{
    public partial class CoachesTableForm : Form
    {
        private CoachController coachController;
        private DataTable originalDataTable;

        public CoachesTableForm()
        {
            InitializeComponent();
            coachController = new CoachController();
            LoadCoaches();
        }

        private void LoadCoaches()
        {
            originalDataTable = coachController.GetAllCoaches();
            dataGridView2.DataSource = originalDataTable;
            AddButtonsToDataGridView();
            dataGridView2.CellValueChanged += DataGridView2_CellValueChanged;
        }

        private void AddButtonsToDataGridView()
        {
            // Add Edit and Delete buttons
            AddButtonColumn("Edit", "Edit");
            AddButtonColumn("Delete", "Delete");

            // Subscribe to CellContentClick event
            dataGridView2.CellContentClick += DataGridView2_CellContentClick;
        }

        private void AddButtonColumn(string name, string headerText)
        {
            if (!dataGridView2.Columns.Contains(name))
            {
                DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                buttonColumn.Name = name;
                buttonColumn.HeaderText = headerText;
                buttonColumn.Text = headerText;
                buttonColumn.UseColumnTextForButtonValue = true;
                dataGridView2.Columns.Add(buttonColumn);
            }
        }

        private void DataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string columnName = dataGridView2.Columns[e.ColumnIndex].Name;

                if (columnName == "Edit")
                {
                    dataGridView2.BeginEdit(true);
                }
                else if (columnName == "Delete")
                {
                    int coachId = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells["id"].Value);
                    DeleteCoach(coachId);
                }
            }
        }

        private void DataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
                int coachId = Convert.ToInt32(row.Cells["Id"].Value);

                string name = row.Cells["Name"].Value.ToString();
                int age = Convert.ToInt32(row.Cells["Age"].Value);
                string gender = row.Cells["Gender"].Value.ToString();
                string contactNumber = row.Cells["ContactNumber"].Value.ToString();
                double salary = Convert.ToDouble(row.Cells["Salary"].Value);
                int workingDays = Convert.ToInt32(row.Cells["WorkingDays"].Value);
                string address = row.Cells["Address"].Value.ToString();
                int privateTrainingCost = Convert.ToInt32(row.Cells["PrivateTrainingCost"].Value);

                Coach coach = new Coach(coachId, name, age, gender, contactNumber, salary, workingDays, address, privateTrainingCost);

                coachController.UpdateCoach(coach);
                LoadCoaches(); // Reload coaches after updating
            }
        }

        private void DeleteCoach(int coachId)
        {
            var confirmResult = MessageBox.Show("Are you sure to delete this coach?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                coachController.DeleteCoach(coachId);
                LoadCoaches(); // Reload coaches after deletion
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Create a filtered view of the original data table
                DataView dataView = originalDataTable.DefaultView;

                // Build a filter expression to match any column containing the search term
                StringBuilder filterExpression = new StringBuilder();
                bool firstColumn = true;

                foreach (DataColumn column in originalDataTable.Columns)
                {
                    if (!firstColumn)
                    {
                        filterExpression.Append(" OR ");
                    }

                    filterExpression.Append($"CONVERT([{column.ColumnName}], 'System.String') LIKE '%{searchTerm}%'");
                    firstColumn = false;
                }

                // Apply the filter to the DataView
                dataView.RowFilter = filterExpression.ToString();

                // Update the DataGridView to display the filtered data
                dataGridView2.DataSource = dataView.ToTable(); // Convert DataView back to DataTable
            }
            else
            {
                // If search term is empty, reset the DataGridView to show the original data
                dataGridView2.DataSource = originalDataTable;
            }
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    this.Hide();
        //    Form form = new CoachRegisterForm();
        //    form.ShowDialog();
        //}

        private void button2_Click(object sender, EventArgs e)
        {
            // Replace this with the form you want to navigate to
            // this.Hide();
            // Form form = new Form5();
            // form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Replace this with the form you want to navigate to
            // this.Hide();
            // Form form = new CoachesTableForm();
            // form.ShowDialog();
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Create a filtered view of the original data table
                DataView dataView = originalDataTable.DefaultView;

                // Build a filter expression to match any column containing the search term
                StringBuilder filterExpression = new StringBuilder();
                bool firstColumn = true;

                foreach (DataColumn column in originalDataTable.Columns)
                {
                    if (!firstColumn)
                    {
                        filterExpression.Append(" OR ");
                    }

                    filterExpression.Append($"CONVERT([{column.ColumnName}], 'System.String') LIKE '%{searchTerm}%'");
                    firstColumn = false;
                }

                // Apply the filter to the DataView
                dataView.RowFilter = filterExpression.ToString();

                // Update the DataGridView to display the filtered data
                dataGridView2.DataSource = dataView.ToTable(); // Convert DataView back to DataTable
            }
            else
            {
                // If search term is empty, reset the DataGridView to show the original data
                dataGridView2.DataSource = originalDataTable;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new CoachRegisterFor();
            form.ShowDialog();
        }
    }
    }

