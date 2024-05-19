using System;
using System.Windows.Forms;
using WindowsFormsApp1.models;
using WindowsFormsApp1.Controllers;
using WindowsFormsApp1.Views;

namespace WindowsFormsApp1.views
{
    public partial class SessionControl1 : UserControl
    {
        public event EventHandler SessionDeleted;

        private Session currentSession;
        private SessionController sessionController;

        public SessionControl1()
        {
            InitializeComponent();
            sessionController = new SessionController();
        }

        public void DisplaySession(Session session)
        {
            currentSession = session;
            // Populate the controls with session details
            labelSessionName.Text = session.Name;
            labelCoachName.Text = session.Coach.Name;
            labelTime.Text = string.Join(", ", session.selectedDays);
        }

        private void imgSession_Click_1(object sender, EventArgs e)
        {
            if (currentSession != null)
            {
                SessionDetailsForm detailsForm = new SessionDetailsForm();
                detailsForm.DisplaySessionDetails(currentSession);
                detailsForm.ShowDialog();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Validate if a session is selected
            if (currentSession == null)
            {
                MessageBox.Show("No session is selected for deletion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Confirm deletion
            DialogResult result = MessageBox.Show("Are you sure you want to delete this session?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Call the session controller to delete the session
                try
                {
                    sessionController.DeleteSession(currentSession.Id);
                    MessageBox.Show("Session deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Raise the SessionDeleted event
                    OnSessionDeleted(EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting session: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected virtual void OnSessionDeleted(EventArgs e)
        {
            SessionDeleted?.Invoke(this, e);
        }

       

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Hide the current form
            this.Hide();
            SessionMembersForm1 sessionMembersForm = new SessionMembersForm1(currentSession);
            sessionMembersForm.Show();
        }
    }
}
