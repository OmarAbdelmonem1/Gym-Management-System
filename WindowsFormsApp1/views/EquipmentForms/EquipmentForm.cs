using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;




using System.Data.SqlClient;
using WindowsFormsApp1.models;
using WindowsFormsApp1.views;
using WindowsFormsApp1.Controller;

namespace WindowsFormsApp1
{
    public partial class EquipmentForm : Form
    {
        private DBConnection dbConnection;
        private Equipment equipment;
        private EquipmentController equipmentController;
        public EquipmentForm()
        {
            InitializeComponent();
            dbConnection = DBConnection.GetInstance();
            this.WindowState = FormWindowState.Maximized;
            equipmentController = new EquipmentController();

        }

        private void Form5_Load(object sender, EventArgs e)
        {
            groupBox2.Hide();
            ShowEquipment();
            dataGridView1.RowPrePaint += dataGridView1_RowPrePaint;

            //DateTime maintenanceDate = DateTime.Now.AddDays(-10); // Set maintenance date 10 days in the past
            //equipment = new Equipment("Test Equipment", "Test Type", "Test Model", 1000, maintenanceDate);

            //// Create maintenance team observer
            //MaintenanceTeam maintenanceTeam = new MaintenanceTeam();

            //// Create maintenance notification observer
            //MaintenanceNotificationObserver notificationObserver = new MaintenanceNotificationObserver();

            //// Attach observers to the equipment
            //equipment.Attach(maintenanceTeam);
            //equipment.Attach(notificationObserver);

            //// Check maintenance status
            //equipment.CheckMaintenanceStatus("TestEquipment123");
        }






        



        private void ShowEquipment()
        {
            // Call the GetEquipment method to retrieve the data
            DataTable equipmentTable = equipmentController.GetEquipment();

            // Clear existing columns and data in the DataGridView
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = null;

            // Set the DataTable as the DataSource for the DataGridView
            dataGridView1.DataSource = equipmentTable;

            // Add a new column for maintenance status
            DataGridViewButtonColumn maintenanceColumn = new DataGridViewButtonColumn();
            maintenanceColumn.HeaderText = "Maintenance";
            maintenanceColumn.Text = "Maintenance";
            maintenanceColumn.Name = "Maintenance";
            maintenanceColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(maintenanceColumn); 

            // Add a new column for the "Edit" button
            DataGridViewButtonColumn editColumn = new DataGridViewButtonColumn();
            editColumn.HeaderText = "Edit";
            editColumn.Text = "Edit";
            editColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(editColumn);

            // Add a new column for the "Delete" button
            DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn();
            deleteColumn.HeaderText = "Delete";
            deleteColumn.Text = "Delete";
            deleteColumn.UseColumnTextForButtonValue = true;
            dataGridView1.Columns.Add(deleteColumn);

            
            

            // Subscribe to the CellClick event to handle button clicks
            dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is the "Maintenance" button
            if (e.ColumnIndex == dataGridView1.Columns.Count - 3 && e.RowIndex >= 0) // Assuming the "Maintenance" button is the third-to-last column
            {
                // Get the equipment ID from the corresponding row
                int equipmentId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["EquipmentID"].Value); // Assuming "EquipmentID" is the name of the column containing the equipment ID

                // Perform the maintenance operation for the selected equipment
                PerformMaintenance(equipmentId);
            }
            else if (e.ColumnIndex == dataGridView1.Columns.Count - 2 && e.RowIndex >= 0) // Assuming the "Edit" button is the second-to-last column
            {
                // Get the equipment ID from the corresponding row
                int equipmentId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["EquipmentID"].Value); // Assuming "EquipmentID" is the name of the column containing the equipment ID

                // Open the edit form or perform the edit operation for the selected equipment
                EditEquipment(equipmentId);
            }
            else if (e.ColumnIndex == dataGridView1.Columns.Count - 1 && e.RowIndex >= 0) // Assuming the "Delete" button is the last column
            {
                // Get the equipment ID from the corresponding row
                int equipmentId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["EquipmentID"].Value); // Assuming "EquipmentID" is the name of the column containing the equipment ID

                // Perform the delete operation for the selected equipment
                DeleteEquipment(equipmentId);
            }
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            // Get the maintenance date cell for the current row
            DataGridViewCell maintenanceDateCell = dataGridView1.Rows[e.RowIndex].Cells["MaintenanceDate"];

            // Check if the cell value is not null and is a valid DateTime
            if (maintenanceDateCell.Value != null && maintenanceDateCell.Value != DBNull.Value)
            {
                if (DateTime.TryParse(maintenanceDateCell.Value.ToString(), out DateTime maintenanceDate))
                {
                    // Compare maintenance date with current date to determine if it's overdue
                    if (maintenanceDate < DateTime.Now)
                    {
                        // If maintenance date is overdue, set the row's background color to red
                        dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Red;
                    }
                }
            }
        }



        private void PerformMaintenance(int equipmentId)
        {
            // Find the equipment with the specified ID
            Equipment equipment = equipmentController.FindEquipmentById(equipmentId);

            if (equipment != null)
            {
                // Notify maintenance team
                //NotifyMaintenanceTeam(equipment);

                // Update maintenance date by adding 6 months
                DateTime updatedMaintenanceDate = DateTime.Now.AddMonths(6);

                // Show confirmation dialog
                DialogResult result = MessageBox.Show($"Are you sure you want to perform maintenance for equipment with ID: {equipmentId}?\nMaintenance date will be updated to: {updatedMaintenanceDate.ToShortDateString()}",
                                                        "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Update maintenance date in the database
                    equipmentController.UpdateMaintenanceDateInDatabase(equipmentId, updatedMaintenanceDate);

                    NotifyMaintenanceTeam(equipmentId);

                }
                else
                {
                    MessageBox.Show("Maintenance operation canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show($"Equipment with ID {equipmentId} not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void NotifyMaintenanceTeam(int equipmentId)
        {
            // Assuming you have a maintenance team object
            MaintenanceTeam maintenanceTeam = new MaintenanceTeam();

            // Notify maintenance team
            maintenanceTeam.UpdateMaintenance($"Maintenance needed for equipment: {equipmentId}");
        }









        //Delete (CRUD)
        private void DeleteEquipment(int equipmentId)
        {
            // Find the row index of the equipment with the specified ID
            int rowIndex = -1;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["EquipmentID"].Value != null && (int)row.Cells["EquipmentID"].Value == equipmentId)
                {
                    rowIndex = row.Index;
                    break;
                }
            }

            if (rowIndex != -1)
            {
                // Confirm deletion
                DialogResult result = MessageBox.Show("Are you sure you want to delete this equipment?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Remove the equipment from the DataGridView
                    dataGridView1.Rows.RemoveAt(rowIndex);

                    // Perform deletion from the database
                    equipmentController.DeleteEquipmentFromDatabase(equipmentId);
                }
            }
            else
            {
                MessageBox.Show("Equipment not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
















        //Update (Crud)

        private string InputBox(string prompt, string title, string defaultValue = "")
        {
            Form inputForm = new Form();
            Label lbl = new Label();
            TextBox txtBox = new TextBox();
            Button btnOk = new Button();

            inputForm.Text = title;
            lbl.Text = prompt;
            txtBox.Text = defaultValue;

            btnOk.Text = "OK";
            btnOk.DialogResult = DialogResult.OK;

            lbl.SetBounds(9, 20, 372, 13);
            txtBox.SetBounds(12, 36, 372, 20);
            btnOk.SetBounds(309, 72, 75, 23);

            lbl.AutoSize = true;

            btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            inputForm.ClientSize = new System.Drawing.Size(396, 107);
            inputForm.Controls.AddRange(new Control[] { lbl, txtBox, btnOk });
            inputForm.ClientSize = new Size(Math.Max(300, lbl.Right + 10), inputForm.ClientSize.Height);
            inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputForm.StartPosition = FormStartPosition.CenterScreen;
            inputForm.MinimizeBox = false;
            inputForm.MaximizeBox = false;
            inputForm.AcceptButton = btnOk;

            DialogResult dialogResult = inputForm.ShowDialog();
            return dialogResult == DialogResult.OK ? txtBox.Text : "";
        }


        private void EditEquipment(int equipmentId)
        {
            // Find the row index of the equipment with the specified ID
            int rowIndex = -1;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["EquipmentID"].Value != null && (int)row.Cells["EquipmentID"].Value == equipmentId)
                {
                    rowIndex = row.Index;
                    break;
                }
            }

            if (rowIndex != -1)
            {
                // Get the current equipment details from the DataGridView
                string name = dataGridView1.Rows[rowIndex].Cells["Name"].Value.ToString();
                string type = dataGridView1.Rows[rowIndex].Cells["Type"].Value.ToString();
                string model = dataGridView1.Rows[rowIndex].Cells["Model"].Value.ToString();
                decimal price = Convert.ToDecimal(dataGridView1.Rows[rowIndex].Cells["Price"].Value);

                // Prompt the user for new values using InputBox
                string updatedName = InputBox("Enter new name:", "Edit Name", name);
                if (updatedName != "")
                {
                    string updatedType = InputBox("Enter new type:", "Edit Type", type);
                    if (updatedType != "")
                    {
                        string updatedModel = InputBox("Enter new model:", "Edit Model", model);
                        if (updatedModel != "")
                        {
                            string updatedPriceStr = InputBox("Enter new price:", "Edit Price", price.ToString());
                            if (updatedPriceStr != "")
                            {
                                // Convert the updated price string to decimal
                                decimal updatedPrice;
                                if (decimal.TryParse(updatedPriceStr, out updatedPrice))
                                {
                                    if(updatedPrice <= 0 )
                                    {
                                        MessageBox.Show("Price must be a positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }
                                    else
                                    {
                                    // Update the DataGridView with the edited equipment details
                                    dataGridView1.Rows[rowIndex].Cells["Name"].Value = updatedName;
                                    dataGridView1.Rows[rowIndex].Cells["Type"].Value = updatedType;
                                    dataGridView1.Rows[rowIndex].Cells["Model"].Value = updatedModel;
                                    dataGridView1.Rows[rowIndex].Cells["Price"].Value = updatedPrice;

                                    // Update the equipment details in the database
                                    equipmentController.UpdateEquipmentInDatabase(equipmentId, updatedName, updatedType, updatedModel, updatedPrice);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Invalid price format. Please enter a valid decimal number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                               
                            }
                        }
                        else
                        {
                            MessageBox.Show("Model is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Type is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Equipment not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void txtmodel_TextChanged(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string name = txtname.Text;
            string type = txttype.Text;
            string model = txtmodel.Text;
            decimal price = txtprice.Value;
            
            if(equipmentController.InsertEquipment(name, type, model, price))
            {
                this.Hide();
                Form form = new EquipmentForm();
                form.ShowDialog();
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new MemberRegisterForm();
            form.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new DashBoardForm();
            form.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            groupBox2.Show();
            button6.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new MembersTableForm();
            form.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            SessionForm f = new SessionForm();
            f.ShowDialog();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            CoachesTableForm f = new CoachesTableForm();
            f.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form f = new DashBoardForm();
            f.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
