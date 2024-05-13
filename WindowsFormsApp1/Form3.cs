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

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        private const string connectionString = "Data Source=LAPTOP-SA22HL97\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;";
        public Form3()
        {

            InitializeComponent();
            SqlConnection sql = new SqlConnection("Data Source=LAPTOP-SA22HL97\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;");

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

        private Subscription CreateSubscription(bool wantsPrivateCoach, List<Services> selectedServices)
        {
            ISubscriptionFactory subscriptionFactory;

            if (wantsPrivateCoach)
            {
                subscriptionFactory = new PlatinumSubscriptionFactory();
            }
            else if (selectedServices.Any())
            {
                subscriptionFactory = new GoldSubscriptionFactory();
            }
            else
            {
                subscriptionFactory = new SilverSubscriptionFactory();
            }

            return subscriptionFactory.CreateSubscription(selectedServices, null); // Pass null for coach
        }


        private int InsertSubscriptionIntoDatabase(Subscription subscription)
        {
            string connectionString = "Data Source=LAPTOP-SA22HL97\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                
                string insertSubscriptionQuery = @"
            INSERT INTO subscriptions (Type, start_date, end_date, totalprice, coach_id) 
            VALUES (@Type, @StartDate, @EndDate, @TotalPrice, @CoachId);
            SELECT SCOPE_IDENTITY();";

                using (SqlCommand subscriptionCommand = new SqlCommand(insertSubscriptionQuery, connection))
                {
                    subscriptionCommand.Parameters.AddWithValue("@Type", subscription.Name);
                    subscriptionCommand.Parameters.AddWithValue("@StartDate", subscription.StartDate);
                    subscriptionCommand.Parameters.AddWithValue("@EndDate", subscription.EndDate);
                    subscriptionCommand.Parameters.AddWithValue("@TotalPrice", subscription.CalculateTotalPrice());
                    // Retrieve a random coach ID from the database (for PlatinumSubscription)
                    int coachId = GetRandomCoachIdFromDatabase(connection);

                    // Check if subscription requires a coach (e.g., PlatinumSubscription)
                    if (subscription is PlatinumSubscription platinumSubscription)
                    {
                        subscriptionCommand.Parameters.AddWithValue("@CoachId", coachId);
                    }
                    else
                    {
                        subscriptionCommand.Parameters.AddWithValue("@CoachId", DBNull.Value);
                    }

                    int subscriptionId = Convert.ToInt32(subscriptionCommand.ExecuteScalar());
                    Console.WriteLine($"Subscription Inserted. ID: {subscriptionId}");
                    return subscriptionId;
                }
            }
        }


        private void InsertMemberIntoDatabase(Member member, int subscriptionId)
        {
            string connectionString = "Data Source=LAPTOP-SA22HL97\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string insertMemberQuery = @"
            INSERT INTO Member (Name, Age, Gender, Email, PhoneNumber, Address, subscriptions_id) 
            VALUES (@Name, @Age, @Gender, @Email, @PhoneNumber, @Address, @SubscriptionId);";

                using (SqlCommand memberCommand = new SqlCommand(insertMemberQuery, connection))
                {
                    memberCommand.Parameters.AddWithValue("@Name", member.Name);
                    memberCommand.Parameters.AddWithValue("@Age", member.Age);
                    memberCommand.Parameters.AddWithValue("@Gender", member.Gender);
                    memberCommand.Parameters.AddWithValue("@Email", member.Email);
                    memberCommand.Parameters.AddWithValue("@PhoneNumber", member.PhoneNumber);
                    memberCommand.Parameters.AddWithValue("@Address", member.Address);
                    memberCommand.Parameters.AddWithValue("@SubscriptionId", subscriptionId);

                    int rowsAffected = memberCommand.ExecuteNonQuery();
                    Console.WriteLine($"Member Inserted. Rows Affected: {rowsAffected}");
                }
            }

            // Check if member's subscription is not null before accessing its properties
            if (member.Subscription != null)
            {
                MessageBox.Show("Member and Subscription created successfully! Type: " + member.Subscription.Name, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Member created successfully, but no subscription assigned.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private Member CreateMemberFromFormInputs()
        {
            // Retrieve form input values
            string name = txtname.Text;
            string email = txtemail.Text;
            string phoneNumber = txtphone.Text;
            string address = txtaddress.Text;
            int age = (int)txtage.Value;
            string gender = Male.Checked ? "Male" : "Female";

            // Create a new member with the provided details (excluding subscription)
            Member newMember = new Member(name, age, gender, email, phoneNumber, address);
            return newMember;
        }

        private Subscription CreateSubscriptionFromFormInputs()
        {
            DateTime startDate = dateTimePicker1.Value;
            int durationMonths = (int)txtdur.Value;
            bool wantsPrivateCoach = txtcoach.Checked;
            bool wantsServices = txtser.Checked;

            // Calculate the end date based on the start date and duration in months
            DateTime endDate = startDate.AddMonths(durationMonths);

            // Create a list to hold selected services
            List<Services> selectedServices = new List<Services>();

            // Add selected services
            if (wantsServices)
            {
                selectedServices.Add(new Services("Spa", 50));
                selectedServices.Add(new Services("Sauna", 50));
                selectedServices.Add(new Services("Jacuzzi", 50));
            }

            // Create a subscription
            Subscription subscription = CreateSubscription(wantsPrivateCoach, selectedServices);
            subscription.EndDate = endDate; // Set the end date to the subscription

            return subscription;
        }

        private void CreateMemberAndSubscription()
        {
            try
            {
                // Create a new member based on form inputs
                Member newMember = CreateMemberFromFormInputs();

                // Create a new subscription based on form inputs
                Subscription subscription = CreateSubscriptionFromFormInputs();

                // Assign the subscription to the member
                newMember.Subscription = subscription;

                // Insert subscription into database and get the ID
                int subscriptionId = InsertSubscriptionIntoDatabase(subscription);

                // Insert member into database and associate with the subscription
                InsertMemberIntoDatabase(newMember, subscriptionId);

                MessageBox.Show("Member and Subscription created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating member and subscription: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private int GetRandomCoachIdFromDatabase(SqlConnection connection)
        {
            string selectCoachIdQuery = "SELECT TOP 1 id FROM Coach ORDER BY NEWID();"; // Retrieve a random coach ID
            SqlCommand selectCoachIdCommand = new SqlCommand(selectCoachIdQuery, connection);
            object result = selectCoachIdCommand.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                throw new Exception("No coach found in the database."); // Handle if no coach is found
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
            this.Hide();
            Form form = new Form2();
            form.ShowDialog();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new Form5();
            form.ShowDialog();
        }

        private void txtaddres_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtphone_TextChanged(object sender, EventArgs e)
        {

        }
    }
}