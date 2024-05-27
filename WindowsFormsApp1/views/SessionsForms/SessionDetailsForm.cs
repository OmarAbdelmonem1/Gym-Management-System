using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.Controllers;
using WindowsFormsApp1.models;

namespace WindowsFormsApp1.views
{
    public partial class SessionDetailsForm : Form
    {
        private SessionController sessionController;
        private List<Coach> coachList;
        private Session currentSession;
        public SessionDetailsForm()
        {
            InitializeComponent();

            sessionController = new SessionController();
            coachList = sessionController.GetCoaches();
            PopulateComboBox();
            PopulateDaysOfWeek();
        }

        private void PopulateComboBox()
        {
            txtcoach.Items.Clear();
            foreach (Coach coach in coachList)
            {
                txtcoach.Items.Add(coach);
            }
            txtcoach.DisplayMember = "Name";
        }

        private void PopulateDaysOfWeek()
        {
            string[] daysOfWeek = Enum.GetNames(typeof(DayOfWeek));
            foreach (string day in daysOfWeek)
            {
                checkedListBox1.Items.Add(day);
            }
        }

        public void DisplaySessionDetails(Session session)
        {
            currentSession = session;

            txtname.Text = session.Name;
            txtcoach.Text = session.Coach.Name;
            txtmax.Text = session.MaxCapacity.ToString();
            txtstart.Text = session.StartTime.ToString();
            txtend.Text = session.EndTime.ToString();
            txtdesc.Text = session.Description;

            // Clear all previously checked items
            checkedListBox1.ClearSelected();

            // Check the items that match the session's days of the week
            foreach (string day in session.selectedDays)
            {
                // Convert the day to the proper format for comparison
                string formattedDay = day.Substring(0, 3); // Assuming the days are abbreviated
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (checkedListBox1.Items[i].ToString().StartsWith(formattedDay, StringComparison.OrdinalIgnoreCase))
                    {
                        checkedListBox1.SetItemChecked(i, true);
                        break;
                    }
                }
            }
        }





        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (currentSession == null)
            {
                MessageBox.Show("No session is selected for editing.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update session details based on the form fields
            currentSession.Name = txtname.Text;
            currentSession.Coach = (Coach)txtcoach.SelectedItem;
            currentSession.MaxCapacity = int.Parse(txtmax.Text);
            currentSession.StartTime = TimeSpan.Parse(txtstart.Text);
            currentSession.EndTime = TimeSpan.Parse(txtend.Text);
            currentSession.Description = txtdesc.Text;

            // Update selected days of the week
            currentSession.selectedDays.Clear();
            foreach (var index in checkedListBox1.CheckedIndices)
            {
                currentSession.selectedDays.Add(checkedListBox1.Items[(int)index].ToString());
            }

            // Call the session controller to edit the session
            try
            {
                sessionController.EditSession(currentSession);
                MessageBox.Show("Session edited successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error editing session: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
    } }
     
   
