﻿using System;
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
        public Form5()
        {
            InitializeComponent();
            dbConnection = DBConnection.GetInstance();
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            groupBox2.Hide();
            ShowEquipment();

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

                using (SqlConnection connection = dbConnection.GetConnection())
                {
                    string insertQuery = "INSERT INTO Equipment (Name, Type, Model, Price) VALUES (@Name, @Type, @Model, @Price)";
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Type", type);
                        command.Parameters.AddWithValue("@Model", model);
                        command.Parameters.AddWithValue("@Price", price);

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
                // Get the DBConnection instance through an instance of the Form5 class
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
            // Check if the clicked cell is the "Edit" button
            if (e.ColumnIndex == dataGridView1.Columns.Count - 2 && e.RowIndex >= 0) // Assuming the "Edit" button is the second-to-last column
            {
                // Get the equipment ID from the corresponding row
                int equipmentId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["EquipmentID"].Value); // Assuming "EquipmentID" is the name of the column containing the equipment ID

                // Open the edit form or perform the edit operation for the selected equipment
                EditEquipment(equipmentId);
            }

            // Check if the clicked cell is the "Delete" button
            if (e.ColumnIndex == dataGridView1.Columns.Count - 1 && e.RowIndex >= 0) // Assuming the "Delete" button is the last column
            {
                // Get the equipment ID from the corresponding row
                int equipmentId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["EquipmentID"].Value); // Assuming "EquipmentID" is the name of the column containing the equipment ID

                // Perform the delete operation for the selected equipment
                DeleteEquipment(equipmentId);
            }
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
                                    // Update the DataGridView with the edited equipment details
                                    dataGridView1.Rows[rowIndex].Cells["Name"].Value = updatedName;
                                    dataGridView1.Rows[rowIndex].Cells["Type"].Value = updatedType;
                                    dataGridView1.Rows[rowIndex].Cells["Model"].Value = updatedModel;
                                    dataGridView1.Rows[rowIndex].Cells["Price"].Value = updatedPrice;

                                    // Update the equipment details in the database
                                    UpdateEquipmentInDatabase(equipmentId, updatedName, updatedType, updatedModel, updatedPrice);
                                }
                                else
                                {
                                    MessageBox.Show("Invalid price format. Please enter a valid decimal number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
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
            Form form = new Form3();
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
        }
    }
}