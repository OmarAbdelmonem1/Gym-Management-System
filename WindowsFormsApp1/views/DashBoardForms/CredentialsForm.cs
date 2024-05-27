using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1.views.DashBoardForms
{
    public partial class CredentialsForm : Form
    {
        public CredentialsForm()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Email and Password cannot be empty.");
                return;
            }

            AddCredentials(email, password);
            LoadData();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to edit.");
                return;
            }

            string email = txtEmail.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Email and Password cannot be empty.");
                return;
            }

            int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
            EditCredentials(id, email, password);
            LoadData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to delete.");
                return;
            }

            int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ID"].Value);
            DeleteCredentials(id);
            LoadData();
        }

        private void LoadData()
        {
            using (SqlConnection connection = DBConnection.GetInstance().GetConnection())
            {
                string query = "SELECT manager_id AS ID, email, password, 'Manager' AS Role FROM Manager " +
                               "UNION ALL " +
                               "SELECT receptionist_id AS ID, email, password, 'Receptionist' AS Role FROM Receptionist";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGridView1.DataSource = dataTable;

                DBConnection.GetInstance().CloseConnection(connection);
            }
        }

        private void AddCredentials(string email, string password)
        {
            using (SqlConnection connection = DBConnection.GetInstance().GetConnection())
            {
                // For simplicity, this example adds to the Manager table. Adjust logic as needed.
                string query = "INSERT INTO Manager (email, password) VALUES (@Email, @Password)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);
                command.ExecuteNonQuery();
                DBConnection.GetInstance().CloseConnection(connection);
            }
        }

        private void EditCredentials(int id, string email, string password)
        {
            using (SqlConnection connection = DBConnection.GetInstance().GetConnection())
            {
                // Determine if the selected row is a Manager or Receptionist
                string role = dataGridView1.SelectedRows[0].Cells["Role"].Value.ToString();
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

        private void DeleteCredentials(int id)
        {
            using (SqlConnection connection = DBConnection.GetInstance().GetConnection())
            {
                // Determine if the selected row is a Manager or Receptionist
                string role = dataGridView1.SelectedRows[0].Cells["Role"].Value.ToString();
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

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form f = new DashBoardForm();
            f.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new MemberRegisterForm();
            form.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new MembersTableForm();
            form.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new EquipmentForm();
            form.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            SessionForm f = new SessionForm();
            f.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            CoachesTableForm f = new CoachesTableForm();
            f.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }
    }
}