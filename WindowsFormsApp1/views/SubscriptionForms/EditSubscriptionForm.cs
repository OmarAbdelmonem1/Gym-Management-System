using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Controllers;
using WindowsFormsApp1.models;
using WindowsFormsApp1.views.DashBoardForms;


namespace WindowsFormsApp1.views
{
    public partial class EditSubscriptionForm : Form
    {
        private Subscription cursubscription;
        private SubscriptionController subscriptionController;
        public EditSubscriptionForm(Subscription subscription)
        {
            InitializeComponent();
            if (SESSION.UserRole == "Receptionist")
            {
                button5.Hide();
            }
            cursubscription = subscription;
            subscriptionController = new SubscriptionController();
        }
        private void ShowSubscriptionDetails(int subscriptionId)
        {
            try
            {
                Subscription subscription = subscriptionController.GetSubscriptionDetails(subscriptionId);

                if (subscription != null)
                {
                    SubscriptionDetailsForm subscriptionForm = new SubscriptionDetailsForm(subscription);
                    subscriptionForm.ShowDialog(); // Display as modal dialog
                }
                else
                {
                    MessageBox.Show($"Subscription with ID {subscriptionId} not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdateSubscriptionFromForm(int subscriptionId)
        {
            try
            {
                // Read and validate start date
                DateTime startDate = dateTimePicker1.Value;

                // Read and validate duration
                if (!int.TryParse(txtdur.Text, out int durationMonths))
                {
                    MessageBox.Show("Please enter a valid duration in months.");
                    return;
                }

                // Determine if the user wants a private coach
                bool wantsPrivateCoach = txtcoach.Checked;

                // Determine if the user wants additional services
                List<Services> selectedServices = new List<Services>();
                if (txtservices.Checked)
                {
                    selectedServices.Add(new Services("Spa", 50));
                    selectedServices.Add(new Services("Sauna", 50));
                    selectedServices.Add(new Services("Jacuzzi", 50));
                }

                // Call the EditSubscription method
               subscriptionController.EditSubscription(subscriptionId, startDate, durationMonths, selectedServices, wantsPrivateCoach);

                MessageBox.Show("Subscription updated successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating subscription: " + ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            UpdateSubscriptionFromForm(cursubscription.Id);
        }

        private void button7_Click(object sender, EventArgs e)
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

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form f = new CredentialsForm();
            f.ShowDialog();
        }
    }
}
