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
using WindowsFormsApp1;

namespace WindowsFormsApp1
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    // Retrieve the username and password entered by the user
        //    string username = textBox1.Text;
        //    string password = textBox2.Text;

        //    // Hardcoded credentials for demonstration purposes
        //    string validUsername = "admin";
        //    string validPassword = "admin";

        //    // Check if the entered credentials match the valid credentials
        //    if (username == validUsername && password == validPassword)
        //    {
        //        // Credentials are valid, perform login action
        //        MessageBox.Show("Login successful!");
        //        this.Hide();

        //        Form form = new DashBoardForm();
        //        form.ShowDialog();


                
        //        // You can perform further actions here, like opening a new form or enabling certain features.
        //    }
        //    else
        //    {
        //        // Credentials are invalid, show error message
        //        MessageBox.Show("Invalid username or password. Please try again.");
        //    }
        //}
        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            // Query the database to check credentials
            string queryManager = "SELECT COUNT(*) FROM Manager WHERE email = @username AND password = @password";
            string queryReceptionist = "SELECT COUNT(*) FROM Receptionist WHERE email = @username AND password = @password";

            using (SqlConnection connection = DBConnection.GetInstance().GetConnection())
            {
                bool isValidManager = CheckCredentials(connection, queryManager, username, password);
                bool isValidReceptionist = CheckCredentials(connection, queryReceptionist, username, password);

                
                if (isValidManager)
                {
                    MessageBox.Show($"Login successful! Role: Manager");
                    this.Hide();
                    SESSION.UserRole = "Manager";
                    Form form = new DashBoardForm(); // Or a specific form for Manager/Receptionist
                    form.ShowDialog();
                }else if(isValidReceptionist)
                {
                    
                    MessageBox.Show($"Login successful! Role: Receptionist");
                    this.Hide();
                    SESSION.UserRole = "Receptionist";
                    Form form = new DashBoardForm(); // Or a specific form for Manager/Receptionist
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Invalid username or password. Please try again.");
                }

                DBConnection.GetInstance().CloseConnection(connection);
            }
        }

        private bool CheckCredentials(SqlConnection connection, string query, string username, string password)
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }


    }
}
