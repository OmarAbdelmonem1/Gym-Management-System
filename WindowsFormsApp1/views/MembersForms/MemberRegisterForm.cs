using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



using System.Data.SqlClient;
using WindowsFormsApp1.models;
using System.Windows.Forms.DataVisualization.Charting;
using WindowsFormsApp1.Controllers;
using WindowsFormsApp1.views;
using WindowsFormsApp1.views.DashBoardForms;


namespace WindowsFormsApp1
{
    public partial class MemberRegisterForm : Form
    {
        private MemberController memberController;
        private SubscriptionController subscriptionController;
        private Subscription currentsub;
        public MemberRegisterForm()
        {
            InitializeComponent();
            if (SESSION.UserRole == "Receptionist")
            {
                button8.Hide();
            }
            memberController = new MemberController();
            subscriptionController = new SubscriptionController();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

            CreateMemberAndSubscription();
            SubscriptionDetailsForm s = new SubscriptionDetailsForm(currentsub);
            s.ShowDialog ();
        }



        private void txtsubs_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void CreateMemberAndSubscription()
        {
            try
            {
                // Create a new member based on form inputs
                Member newMember = memberController.CreateMemberFromFormInputs(
                    txtname.Text,
                    txtemail.Text,
                    txtphone.Text,
                    txtaddress.Text,
                    (int)txtage.Value,
                    Male.Checked ? "Male" : "Female"
                );

                // Create a new subscription based on form inputs
                Subscription subscription = subscriptionController.CreateSubscriptionFromFormInputs(
                    dateTimePicker1.Value,
                    (int)txtdur.Value,
                    txtcoach.Checked,
                    txtser.Checked
                );

                newMember.Subscription = subscription;
                currentsub = subscription;

                int subscriptionId = subscriptionController.InsertSubscriptionIntoDatabase(subscription);
                memberController.InsertMemberIntoDatabase(newMember, subscriptionId);

                MessageBox.Show("Member and Subscription created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating member and subscription: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    

private void label6_Click(object sender, EventArgs e)
        {

        }

        private void txtaddress_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new EquipmentForm();
            form.ShowDialog();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new EquipmentForm();
            form.ShowDialog();
        }

        private void txtaddres_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtphone_TextChanged(object sender, EventArgs e)
        {

        }

        private void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new MemberRegisterForm();
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

        private void button4_Click_1(object sender, EventArgs e)
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