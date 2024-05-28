using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WindowsFormsApp1.Controllers;
using WindowsFormsApp1.models;
using WindowsFormsApp1.views.DashBoardForms;


namespace WindowsFormsApp1.views
{
    public partial class CoachRegisterFor : Form
    {
        private CoachController coachController;
        public CoachRegisterFor()
        {
            InitializeComponent();
            if (SESSION.UserRole == "Receptionist")
            {
                button9.Hide();
            }
            coachController = new CoachController();
          
        }
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                // Retrieve values from UI controls
                string name = txtname.Text;
                int age = Convert.ToInt32(txtage.Text);
                string gender = maleRadioButton.Checked ? "Male" : "Female";
                string contactNumber = txtphone.Text;
                double salary = Convert.ToDouble(numericsalary.Value);
                int workingDays = Convert.ToInt32(numericworkingdays.Value);
                string address = txtaddress.Text;
                int privateTrainingCost = Convert.ToInt32(numericprivatesalary.Value);

                // Create a Coach object
                Coach coach = new Coach
                {
                    Name = name,
                    Age = age,
                    Gender = gender,
                    ContactNumber = contactNumber,
                    Salary = salary,
                    WorkingDays = workingDays,
                    Address = address,
                    private_training_cost = privateTrainingCost // Assuming private_training_cost is the property name
                };

                // Call the InsertCoachIntoDatabase method with the Coach object
                coachController.InsertCoachIntoDatabase(coach);

                // Optionally, display a success message
                MessageBox.Show("Coach inserted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                Form form = new CoachesTableForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Handle or log the exception as needed
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new CoachesTableForm();
            form.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {

            this.Hide();
            SessionForm f = new SessionForm();
            f.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            CoachesTableForm f = new CoachesTableForm();
            f.ShowDialog();
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form f = new DashBoardForm();
            f.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form f = new CredentialsForm();
            f.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Hide();
            Form f = new LoginForm();
            f.ShowDialog();
            SESSION.Clear();
        }

        private void CoachRegisterFor_Load(object sender, EventArgs e)
        {

        }
    }
}
