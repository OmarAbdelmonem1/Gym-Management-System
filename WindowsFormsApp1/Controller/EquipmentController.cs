using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


namespace WindowsFormsApp1.Controller
{
    public class EquipmentController
    {
        private DBConnection dbConnection = DBConnection.GetInstance();






        //insert (CRUD)
        public bool InsertEquipment(string name,string type,string model, decimal price)
        {
            try
            {
                
                DateTime currentDate = DateTime.Now;
                DateTime maintenanceDate = currentDate.AddMonths(-1); // Calculate maintenance date by adding 6 months to the current date

                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (string.IsNullOrEmpty(type))
                {
                    MessageBox.Show("Type is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (string.IsNullOrEmpty(model))
                {
                    MessageBox.Show("Model is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (price <= 0)
                {
                    MessageBox.Show("Price must be a positive number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
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
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add equipment: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
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

        public void DeleteEquipmentFromDatabase(int equipmentId)
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
        public void UpdateEquipmentInDatabase(int equipmentId, string updatedName, string updatedType, string updatedModel, decimal updatedPrice)
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











        public void UpdateMaintenanceDateInDatabase(int equipmentId, DateTime updatedMaintenanceDate)
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





        public Equipment FindEquipmentById(int equipmentId)
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




    }
}
