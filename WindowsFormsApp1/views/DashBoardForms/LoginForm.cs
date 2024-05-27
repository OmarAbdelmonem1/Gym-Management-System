using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        private void button1_Click(object sender, EventArgs e)
        {
            // Retrieve the username and password entered by the user
            string username = textBox1.Text;
            string password = textBox2.Text;

            // Hardcoded credentials for demonstration purposes
            string validUsername = "admin";
            string validPassword = "admin";

            // Check if the entered credentials match the valid credentials
            if (username == validUsername && password == validPassword)
            {
                // Credentials are valid, perform login action
                MessageBox.Show("Login successful!");
                this.Hide();

                Form form = new DashBoardForm();
                form.ShowDialog();


                
                // You can perform further actions here, like opening a new form or enabling certain features.
            }
            else
            {
                // Credentials are invalid, show error message
                MessageBox.Show("Invalid username or password. Please try again.");
            }
        }

    }
}
