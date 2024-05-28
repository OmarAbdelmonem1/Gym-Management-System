using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace WindowsFormsApp1.Controller
{
    public class CredentialController
    {
        

        public void AddCredentials(string email, string password, string role)
        {
            using (SqlConnection connection = DBConnection.GetInstance().GetConnection())
            {
                if (role == "Manager")
                {
                    // For simplicity, this example adds to the Manager table. Adjust logic as needed.
                    string query = "INSERT INTO Manager (email, password) VALUES (@Email, @Password)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    command.ExecuteNonQuery();
                    DBConnection.GetInstance().CloseConnection(connection);
                }
                else
                {
                    // For simplicity, this example adds to the Manager table. Adjust logic as needed.
                    string query = "INSERT INTO Receptionist (email, password) VALUES (@Email, @Password)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", password);
                    command.ExecuteNonQuery();
                    DBConnection.GetInstance().CloseConnection(connection);
                }

            }
        }

        public void EditCredentials(int id, string email, string password, string role)
        {
            using (SqlConnection connection = DBConnection.GetInstance().GetConnection())
            {
                // Determine if the selected row is a Manager or Receptionist
                //string role = dataGridView1.SelectedRows[0].Cells["Role"].Value.ToString();
                string query = "";

                if (role == "Manager")
                {
                    query = "UPDATE Manager SET email = @Email, password = @Password WHERE manager_id = @ID";
                }
                else if (role == "Receptionist")
                {
                    query = "UPDATE Receptionist SET email = @Email, password = @Password WHERE receptionist_id = @ID";
                }

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@ID", id);
                command.ExecuteNonQuery();
                DBConnection.GetInstance().CloseConnection(connection);
            }
        }

        public void DeleteCredentials(int id, string role)
        {
            using (SqlConnection connection = DBConnection.GetInstance().GetConnection())
            {
                // Determine if the selected row is a Manager or Receptionist
                //string role = dataGridView1.SelectedRows[0].Cells["Role"].Value.ToString();
                string query = "";

                if (role == "Manager")
                {
                    query = "DELETE FROM Manager WHERE manager_id = @ID";
                }
                else if (role == "Receptionist")
                {
                    query = "DELETE FROM Receptionist WHERE receptionist_id = @ID";
                }

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", id);
                command.ExecuteNonQuery();
                DBConnection.GetInstance().CloseConnection(connection);
            }
        }
    }
}
