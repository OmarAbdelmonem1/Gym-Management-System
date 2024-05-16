using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using WindowsFormsApp1.models;

namespace WindowsFormsApp1.Controllers
{
    public class MemberController
    {
        private const string connectionString = "Data Source=LAPTOP-SA22HL97\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;";

        public void InsertMemberIntoDatabase(Member member, int subscriptionId)
        {
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

        public Member CreateMemberFromFormInputs(string name, string email, string phoneNumber, string address, int age, string gender)
        {
            // Create a new member with the provided details (excluding subscription)
            return new Member(name, age, gender, email, phoneNumber, address);
        }

    }
}
