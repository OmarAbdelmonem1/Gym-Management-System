using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsFormsApp1.Controllers;
using WindowsFormsApp1.views.DashBoardForms;

namespace WindowsFormsApp1.views
{
    public partial class SessionForm : Form
    {
        private SessionController sessionController;
        private List<Session> sessions;

        public SessionForm()
        {
            InitializeComponent();
            if (SESSION.UserRole == "Receptionist")
            {
                button8.Hide();
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new MemberRegisterForm();
            form.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new EquipmentForm();
            form.ShowDialog();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new MembersTableForm();
            form.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
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

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form f = new DashBoardForm();
            f.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form f = new SessionsCreationForm();
            f.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form f = new CredentialsForm();
            f.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Hide();
            Form f = new LoginForm();
            f.ShowDialog();
            SESSION.Clear();
        }
    }
}
