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

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        private const string connectionString = "Data Source=DESKTOP-8PECM9F\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;";
        public Form3()
        {

            InitializeComponent();
            SqlConnection sql = new SqlConnection("Data Source=DESKTOP-8PECM9F\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;");

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

        private void CreateMemberAndSubscription()
        {
            try
            {
                // Retrieve form input values
                string name = txtname.Text;
                string email = txtemail.Text;
                string phoneNumber = txtphone.Text;
                string address = txtaddress.Text;
                int age = (int)txtage.Value;
                string gender = Male.Checked ? "Male" : "Female";
                DateTime startDate = dateTimePicker1.Value;
                int durationMonths = (int)txtdur.Value;

                bool wantsPrivateCoach = txtcoach.Checked;
                bool wantsServices = txtser.Checked;

                // Calculate the end date based on the start date and duration in months
                DateTime EndDate = startDate.AddMonths(durationMonths);

                // Create a subscription factory
                ISubscriptionFactory subscriptionFactory = new SubscriptionFactory();

                // Create a list to hold selected services
                List<Services> selectedServices = new List<Services>();

                // If user wants services, add them to the selected services list
                if (wantsServices)
                {
                    selectedServices.Add(new Services("Spa", 50));
                    selectedServices.Add(new Services("Sauna", 50));
                    selectedServices.Add(new Services("Jacuzzi", 50));
                }

                // Create a subscription based on user selections
                Subscription subscription = subscriptionFactory.CreateSubscription(wantsPrivateCoach, selectedServices, null); // Pass null for coach

                // Create a new member with the provided details and subscription
                Member newMember = new Member(name, age, gender, email, phoneNumber, address, subscription);

                // Set the calculated end date to the subscription
                newMember.Subscription.EndDate = EndDate;

                // Save member and subscription to the database
                SaveMemberAndSubscription(newMember);

                MessageBox.Show("Member and Subscription created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating member and subscription: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void SaveMemberAndSubscription(Member member)
        {
            string connectionString = "Data Source=DESKTOP-8PECM9F\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Insert subscription details into the subscriptions table
                    string insertSubscriptionQuery = @"
     INSERT INTO subscriptions (Type, start_date, end_date, totalprice, coach_id) 
     VALUES (@Type, @StartDate, @EndDate, @TotalPrice, @CoachId);
     SELECT SCOPE_IDENTITY();";

                    Subscription subscription = member.Subscription;
                    int subscriptionId = 0;

                    using (SqlCommand subscriptionCommand = new SqlCommand(insertSubscriptionQuery, connection))
                    {
                        subscriptionCommand.Parameters.AddWithValue("@Type", subscription.Name);
                        subscriptionCommand.Parameters.AddWithValue("@StartDate", subscription.StartDate);
                        subscriptionCommand.Parameters.AddWithValue("@EndDate", subscription.EndDate);
                        subscriptionCommand.Parameters.AddWithValue("@TotalPrice", subscription.CalculateTotalPrice());

                        // Retrieve a coach from the database (for PlatinumSubscription)
                        int coachId = GetRandomCoachIdFromDatabase(connection); // Example: Get random coach ID

                        // Check if subscription is PlatinumSubscription and assign coach ID
                        if (subscription is PlatinumSubscription platinumSubscription)
                        {
                            subscriptionCommand.Parameters.AddWithValue("@CoachId", coachId);
                        }
                        else
                        {
                            subscriptionCommand.Parameters.AddWithValue("@CoachId", DBNull.Value);
                        }

                        // Execute the subscription insert command and retrieve the inserted ID
                        subscriptionId = Convert.ToInt32(subscriptionCommand.ExecuteScalar());
                        Console.WriteLine($"Subscription Inserted. ID: {subscriptionId}");
                    }

                    // Insert member details into the members table
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

                        // Execute the insert command for the member
                        int rowsAffected = memberCommand.ExecuteNonQuery();
                        Console.WriteLine($"Member Inserted. Rows Affected: {rowsAffected}");
                    }
                }

                MessageBox.Show("Member and Subscription created successfully! Type: " + member.Subscription.Name, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving member and subscription to the database: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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