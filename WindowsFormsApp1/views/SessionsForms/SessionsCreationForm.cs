using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.Controllers;
using WindowsFormsApp1.models;
using WindowsFormsApp1.views;
using WindowsFormsApp1.views.DashBoardForms;

namespace WindowsFormsApp1
{
    public partial class SessionsCreationForm : Form
    {
        private SessionController sessionController;
        private List<Coach> coachList;

        public SessionsCreationForm()
        {
            InitializeComponent();
            if (SESSION.UserRole == "Receptionist")
            {
                button8.Hide();
            }
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

        private void button5_Click(object sender, EventArgs e)
        {
            string name = txtname.Text;
            Coach selectedCoach = (Coach)txtcoach.SelectedItem;
            if (selectedCoach == null)
            {
                MessageBox.Show("Please select a coach.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int coachId = selectedCoach.id;
            int maxCapacity = (int)txtmax.Value;
            TimeSpan startTime = txtstart.Value.TimeOfDay;
            TimeSpan endTime = txtend.Value.TimeOfDay;
            string description = txtdesc.Text;

            List<string> selectedDays = checkedListBox1.CheckedItems.Cast<string>().ToList();

            try
            {
                sessionController.AddSession(name, coachId, maxCapacity, startTime, endTime, selectedDays, description);
                MessageBox.Show("Session added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Hide();
                SessionForm f = new SessionForm();
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e) { }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) { }

        private void txtcoach_SelectedIndexChanged(object sender, EventArgs e) { }

        private void txtname_TextChanged(object sender, EventArgs e) { }

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

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            SessionForm f = new SessionForm();
            f.ShowDialog();
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

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form f = new DashBoardForm();
            f.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form f = new CredentialsForm();
            f.ShowDialog();
        }
    }
}
