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

namespace WindowsFormsApp1.views
{
    public partial class EditSubscriptionForm : Form
    {
        private Subscription cursubscription;
        private SubscriptionController subscriptionController;
        public EditSubscriptionForm(Subscription subscription)
        {
            InitializeComponent();
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
    }
}
