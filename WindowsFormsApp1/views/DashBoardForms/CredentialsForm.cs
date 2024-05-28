using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using WindowsFormsApp1.Controller;

namespace WindowsFormsApp1.views.DashBoardForms
{
    public partial class CredentialsForm : Form
    {
        private CredentialController credentialController;
        public CredentialsForm()
        {
            InitializeComponent();
            credentialController = new CredentialController();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Text;
            string role = this.radioButton1.Checked ? "Manager" : "Receptionist";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Email and Password cannot be empty.");
                return;
            }

            credentialController.AddCredentials(email, password, role);
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
            string role = dataGridView1.SelectedRows[0].Cells["Role"].Value.ToString();
            credentialController.EditCredentials(id, email, password, role);
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
            string role = dataGridView1.SelectedRows[0].Cells["Role"].Value.ToString();
            credentialController.DeleteCredentials(id, role);
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

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Hide();
            Form f = new LoginForm();
            f.ShowDialog();
            SESSION.Clear();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}