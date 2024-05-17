using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;




using System.Data.SqlClient;
using WindowsFormsApp1.models;

namespace WindowsFormsApp1
{
    public partial class Form5 : Form
    {
        private DBConnection dbConnection;
        private Equipment equipment;
        public Form5()
        {
            InitializeComponent();
            dbConnection = DBConnection.GetInstance();
            this.WindowState = FormWindowState.Maximized;

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



        //insert (CRUD)
        public void InsertEquipment()
        {
            try
            {
                string name = txtname.Text;
                string type = txttype.Text;
                string model = txtmodel.Text;
                decimal price = txtprice.Value;
                DateTime currentDate = DateTime.Now;
                DateTime maintenanceDate = currentDate.AddMonths(-1); // Calculate maintenance date by adding 6 months to the current date

                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(type))
                {
                    MessageBox.Show("Type is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(model))
                {
                    MessageBox.Show("Model is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (price <= 0)
                {
                    MessageBox.Show("Price must be a positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection connection = dbConnection.GetConnection())
                {
                    string insertQuery = "INSERT INTO Equipment (Name, Type, Model, Price, MaintenanceDate) VALUES (@Name, @Type, @Model, @Price, @MaintenanceDate)";
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Type", type);
                        command.Parameters.AddWithValue("@Model", model);
                        command.Parameters.AddWithValue("@Price", price);
                        command.Parameters.AddWithValue("@MaintenanceDate", maintenanceDate);

                        command.ExecuteNonQuery();
                    }
                }


                MessageBox.Show("Equipment added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                Form form = new Form5();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add equipment: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        //Read (CRUD)
        public DataTable GetEquipment()
        {
            DataTable dataTable = new DataTable();

            try
            {
                DBConnection dbConnection = DBConnection.GetInstance();

                using (SqlConnection connection = dbConnection.GetConnection())
                {
                    string selectQuery = "SELECT * FROM Equipment";

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching equipment from the database: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MaintenanceTeam maintenanceTeam = new MaintenanceTeam();
            MaintenanceNotificationObserver notificationObserver = new MaintenanceNotificationObserver();

            List<Equipment> equipmentList = new List<Equipment>();
            bool isAnyEquipmentOverdue = false;

            foreach (DataRow row in dataTable.Rows)
            {
                DateTime maintenanceDate = row.Field<DateTime>("MaintenanceDate");
                int equipmentId = row.Field<int>("EquipmentID");

                Equipment equipment = new Equipment(maintenanceDate);
                equipment.Attach(maintenanceTeam);
                equipment.Attach(notificationObserver);

                if (equipment.IsMaintenanceOverdue())
                {
                    isAnyEquipmentOverdue = true;
                }

                equipmentList.Add(equipment);
            }

            if (isAnyEquipmentOverdue)
            {
                notificationObserver.NotifyMaintenanceOverdue("There are equipments that need maintenance.");
            }

            return dataTable;
        }



        private void ShowEquipment()
        {
            // Call the GetEquipment method to retrieve the data
            DataTable equipmentTable = GetEquipment();

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
            Equipment equipment = FindEquipmentById(equipmentId);

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
                    UpdateMaintenanceDateInDatabase(equipmentId, updatedMaintenanceDate);

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



        private void UpdateMaintenanceDateInDatabase(int equipmentId, DateTime updatedMaintenanceDate)
        {
            // Prepare the SQL update command
            string updateQuery = "UPDATE Equipment SET MaintenanceDate = @UpdatedMaintenanceDate WHERE EquipmentID = @EquipmentId";

            // Use DBConnection to get the connection
            DBConnection dbConnection = DBConnection.GetInstance();

            using (SqlConnection connection = dbConnection.GetConnection())
            {
                
                // Create the command
                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    // Add parameters to the command
                    command.Parameters.AddWithValue("@UpdatedMaintenanceDate", updatedMaintenanceDate);
                    command.Parameters.AddWithValue("@EquipmentId", equipmentId);

                    // Execute the update command
                    int rowsAffected = command.ExecuteNonQuery();

                    // Check if the update was successful
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Maintenance date updated successfully in the database.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to update maintenance date in the database.");
                    }
                }
            }
        }



        private Equipment FindEquipmentById(int equipmentId)
        {
            Equipment equipment = null;

            try
            {
                // Get a database connection instance
                DBConnection dbConnection = DBConnection.GetInstance();

                // Open a connection to the database
                using (SqlConnection connection = dbConnection.GetConnection())
                {
                    // Define the SQL query to select equipment by ID
                    string selectQuery = "SELECT * FROM Equipment WHERE EquipmentID = @EquipmentId";

                    // Create a SqlCommand with the select query and connection
                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        // Add the equipment ID parameter to the command
                        command.Parameters.AddWithValue("@EquipmentId", equipmentId);

                        // Execute the command to retrieve the equipment data
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Check if data was retrieved
                            if (reader.Read())
                            {
                                // Extract data from the reader and create an Equipment object
                                string name = reader["Name"].ToString();
                                string type = reader["Type"].ToString();
                                string model = reader["Model"].ToString();
                                decimal price = Convert.ToDecimal(reader["Price"]);
                                DateTime maintenanceDate = Convert.ToDateTime(reader["MaintenanceDate"]);

                                // Create the Equipment object
                                equipment = new Equipment(name, type, model, price, maintenanceDate);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions, such as database connection errors
                MessageBox.Show($"Error finding equipment by ID: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return equipment;
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
                    DeleteEquipmentFromDatabase(equipmentId);
                }
            }
            else
            {
                MessageBox.Show("Equipment not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to delete equipment from the database
        private void DeleteEquipmentFromDatabase(int equipmentId)
        {
            try
            {
                // Get the DBConnection instance through an instance of the Form5 class
                DBConnection dbConnection = DBConnection.GetInstance();

                using (SqlConnection connection = dbConnection.GetConnection())
                {
                    string deleteQuery = "DELETE FROM Equipment WHERE EquipmentID = @EquipmentId";

                    using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@EquipmentId", equipmentId);


                        int rowsAffected = command.ExecuteNonQuery();
                      
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Equipment deleted from the database.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No equipment deleted from the database.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting equipment from the database: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }





        //Update (CRUD)
        private void UpdateEquipmentInDatabase(int equipmentId, string updatedName, string updatedType, string updatedModel, decimal updatedPrice)
        {
            try
            {
                // Get the DBConnection instance through an instance of the Form5 class
                DBConnection dbConnection = DBConnection.GetInstance();

                using (SqlConnection connection = dbConnection.GetConnection())
                {
                    string updateQuery = @"
                UPDATE Equipment
                SET Name = @Name, Type = @Type, Model = @Model, Price = @Price
                WHERE EquipmentID = @EquipmentId";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", updatedName);
                        command.Parameters.AddWithValue("@Type", updatedType);
                        command.Parameters.AddWithValue("@Model", updatedModel);
                        command.Parameters.AddWithValue("@Price", updatedPrice);
                        command.Parameters.AddWithValue("@EquipmentId", equipmentId);

                        
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Equipment details updated in the database.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No equipment details updated in the database.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating equipment details in the database: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
                                    UpdateEquipmentInDatabase(equipmentId, updatedName, updatedType, updatedModel, updatedPrice);
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
            InsertEquipment();
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
            Form form = new Form2();
            form.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            groupBox2.Show();
            button6.Hide();
        }
    }
}
