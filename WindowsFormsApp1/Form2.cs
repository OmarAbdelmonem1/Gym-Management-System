using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        private DBConnection dbConnection;
        private SqlConnection connection;
        public Form2()
        {
            InitializeComponent();
            dbConnection = DBConnection.GetInstance();
            connection = dbConnection.GetConnection();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                LoadData();
            }
            else
            {
                MessageBox.Show("Failed to open database connection. Dashboard data cannot be loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void LoadData()
        {
            try
            {
                // Fetch and display gym statistics
                int totalMembers = FetchTotalMembers();
                int totalEqu = FetchTotalEqu();
                int totalSession = FetchTotalSession();

                label1.Text = $"Total Members: {totalMembers}";
                label2.Text = $"Total Equipments: {totalEqu}";
                label3.Text = $"Total Sessions: {totalSession}";

               

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbConnection.CloseConnection(connection);
            }
        }

        private int FetchTotalMembers()
        {
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Member", connection))
            {
                return (int)command.ExecuteScalar();
            }
        }
        private int FetchTotalEqu()
        {
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Equipment", connection))
            {
                return (int)command.ExecuteScalar();
            }
        }

        private int FetchTotalSession()
        {
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Sessions", connection))
            {
                return (int)command.ExecuteScalar();
            }
        }
       



        private void txtname_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new MemberRegisterForm();
            form.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new Form5();
            form.ShowDialog();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


    }
}