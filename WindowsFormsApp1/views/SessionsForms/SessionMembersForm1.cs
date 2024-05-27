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
using WindowsFormsApp1.views; // Assuming your Member class is in the Models namespace
using WindowsFormsApp1.views.DashBoardForms;

namespace WindowsFormsApp1.Views
{
    public partial class SessionMembersForm1 : Form
    {
        private SessionController sessionController;
        private List<Member> members;
        private Session currentSession;
        public SessionMembersForm1(Session currentSession)
        {
            InitializeComponent();
            if (SESSION.UserRole == "Receptionist")
            {
                button8.Hide();
            }
            sessionController = new SessionController();
            PopulateMembers(currentSession.Id); // Pass the session ID
            this.currentSession = currentSession;
        }

        private void PopulateMembers(int sessionId)
        {
            try
            {
                members = sessionController.GetAllMembers(sessionId);

                // Clear existing columns before adding new ones
                dataGridViewMembers.Columns.Clear();

                // Set the DataSource to your members list
                dataGridViewMembers.DataSource = members;

                // Hide the unwanted columns
                dataGridViewMembers.Columns["Id"].Visible = false;
                dataGridViewMembers.Columns["Name"].Visible = false;
                dataGridViewMembers.Columns["Gender"].Visible = false;
                dataGridViewMembers.Columns["Email"].Visible = false;
                dataGridViewMembers.Columns["Address"].Visible = false;
                dataGridViewMembers.Columns["PhoneNumber"].Visible = false;
                dataGridViewMembers.Columns["Age"].Visible = false;
                dataGridViewMembers.Columns["Subscription"].Visible = false;

                // Optionally, you can also set the header text for the Id column
                dataGridViewMembers.Columns["Id"].HeaderText = "Member Id";

                // Add a new column for deleting members
                DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
                deleteButtonColumn.HeaderText = "Delete";
                deleteButtonColumn.Text = "Delete";
                deleteButtonColumn.Name = "DeleteButtonColumn";
                deleteButtonColumn.UseColumnTextForButtonValue = true;
                dataGridViewMembers.Columns.Add(deleteButtonColumn);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading members: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void dataGridViewMembers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the cell clicked is the delete button column
            if (dataGridViewMembers.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0 && e.ColumnIndex == dataGridViewMembers.Columns["DeleteButtonColumn"].Index)
            {
                // Retrieve the member ID and session ID from the selected row
                int memberId = Convert.ToInt32(dataGridViewMembers.Rows[e.RowIndex].Cells["MemberId"].Value);
                int sessionId = Convert.ToInt32(currentSession.Id);
                // Log the member ID and session ID
                Console.WriteLine("Member ID: " + memberId);
                Console.WriteLine("Session ID: " + sessionId);
                // Call the method to delete the member from the session
                DeleteMember(memberId, sessionId);

                // Reload the members after deletion
                PopulateMembers(sessionId);
            }
        }


        private void DeleteMember(int memberId, int sessionId)
        {
            try
            {
                sessionController.DeleteMemberFromSession(memberId, sessionId);
                MessageBox.Show("Member deleted from session successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PopulateMembers(sessionId); // Reload the members after deletion
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting member from session: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                // Retrieve the member ID from the txtId TextBox
                int memberId = Convert.ToInt32(txtid.Text);

                // Insert the new member into the session
                sessionController.AddMemberToSession(memberId, currentSession.Id);

                // Show a success message
                MessageBox.Show("Member added to session successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reload the members after addition
                PopulateMembers(currentSession.Id);
            }
            catch (Exception ex)
            {
                // Show an error message if something goes wrong
                MessageBox.Show("Error adding member to session: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new SessionForm();
            form.ShowDialog();
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