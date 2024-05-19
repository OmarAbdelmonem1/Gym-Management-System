using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsFormsApp1.Controllers;

namespace WindowsFormsApp1.views
{
    public partial class SessionForm : Form
    {
        private SessionController sessionController;
        private List<Session> sessions;

        public SessionForm()
        {
            InitializeComponent();

            // Initialize SessionController
            sessionController = new SessionController();

            // Load sessions from the database
            LoadSessions();
        }

        private void LoadSessions()
        {
            try
            {
                // Retrieve sessions from SessionController
                sessions = sessionController.GetAllSessions();
                // Log the count of sessions retrieved
                Console.WriteLine($"Number of sessions retrieved: {sessions.Count}");
                // Clear existing controls in FlowLayoutPanel
                flowLayoutPanelSessions.Controls.Clear();

                // Create and add SessionCard controls for each session
                foreach (Session session in sessions)
                {
                    SessionControl1 sessionCard = new SessionControl1();
                    sessionCard.DisplaySession(session);
                    // Attach the SessionDeleted event handler
                    sessionCard.SessionDeleted += SessionCard_SessionDeleted;
                    // Add SessionCard to FlowLayoutPanel
                    flowLayoutPanelSessions.Controls.Add(sessionCard);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading sessions: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SessionCard_SessionDeleted(object sender, EventArgs e)
        {
            // Reload sessions after a session is deleted
            LoadSessions();
        }
    }
}
