using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.models;

namespace WindowsFormsApp1
{
    public partial class Form4 : Form
    {
        private string connectionString = "Data Source=LAPTOP-SA22HL97\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;";
        private List<Coach> coachList; // List to hold Coach objects

        public Form4()
        {
            InitializeComponent();
            coachList = PopulateCoachNames(); // Populate coachList with Coach objects
            PopulateComboBox(); // Populate ComboBox with Coach names
            PopulateDaysOfWeek(); // Populate days of the week in CheckedListBox
        }

        private List<Coach> PopulateCoachNames()
        {
            List<Coach> coaches = new List<Coach>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Name FROM Coach"; // Query to retrieve coach names and ids from the database

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int coachId = Convert.ToInt32(reader["Id"]);
                                string coachName = reader["Name"].ToString();

                                // Create a Coach object using the retrieved data and add it to the list
                                Coach coach = new Coach(coachId, coachName);
                                coaches.Add(coach);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving coach names: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return coaches;
        }

        private void PopulateComboBox()
        {
            // Clear the ComboBox before adding items (to avoid duplicates)
            txtcoach.Items.Clear();

            // Add Coach objects to the ComboBox
            foreach (Coach coach in coachList)
            {
                txtcoach.Items.Add(coach); // Add the entire Coach object
            }

            // Set DisplayMember to show coach names in the combo box
            txtcoach.DisplayMember = "Name";
        }

        private void PopulateDaysOfWeek()
        {
            // Array of days of the week
            string[] daysOfWeek = Enum.GetNames(typeof(DayOfWeek));
            foreach (string day in daysOfWeek)
            {
                checkedListBox1.Items.Add(day); // Add each day to CheckedListBox
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string name = txtname.Text;
            Coach selectedCoach = (Coach)txtcoach.SelectedItem; 

            int coachId = selectedCoach.id;
            int maxCapacity = (int)txtmax.Value;
            TimeSpan startTime = txtstart.Value.TimeOfDay; // Use TimeSpan for time part only
            TimeSpan endTime = txtend.Value.TimeOfDay;
            string description = txtdesc.Text;

            List<string> selectedDays = new List<string>();
            foreach (var item in checkedListBox1.CheckedItems)
            {
                selectedDays.Add(item.ToString());
            }
            string daysOfWeek = string.Join(",", selectedDays);

            try
            {
                // Check if the selected coach already has a session on the selected day
                if (IsCoachAlreadyBooked(coachId, daysOfWeek))
                {
                    MessageBox.Show("The selected coach already has a session on the chosen day.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Abort session creation
                }

                // Check for overlapping sessions
                if (IsTimeSlotOccupied(coachId, startTime, endTime, daysOfWeek))
                {
                    MessageBox.Show("There is an overlapping session for the selected coach during the chosen time.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Abort session creation
                }

                // Perform additional time validation (e.g., minimum and maximum duration)
                TimeSpan sessionDuration = endTime - startTime;
                if (sessionDuration < TimeSpan.FromHours(1) || sessionDuration > TimeSpan.FromHours(3))
                {
                    MessageBox.Show("Session duration must be between 1 hour and 3 hours.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Abort session creation
                }
                // Check maximum capacity
                if (maxCapacity > 10)
                {
                    MessageBox.Show("Maximum capacity cannot exceed 10 attendees.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Abort session creation
                }
                // If all validations pass, proceed to insert the session into the database
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Sessions (Name, Coach_Id, MaxCapacity, Start_Time, End_Time, Days_Of_Week, Description) " +
                                   "VALUES (@Name, @CoachId, @MaxCapacity, @StartTime, @EndTime, @DaysOfWeek, @Description)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@CoachId", coachId);
                        command.Parameters.AddWithValue("@MaxCapacity", maxCapacity);
                        command.Parameters.AddWithValue("@StartTime", startTime);
                        command.Parameters.AddWithValue("@EndTime", endTime);
                        command.Parameters.AddWithValue("@DaysOfWeek", daysOfWeek);
                        command.Parameters.AddWithValue("@Description", description);

                        command.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Session added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding session: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool IsCoachAlreadyBooked(int coachId, string daysOfWeek)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Split the daysOfWeek string into individual days
                string[] selectedDays = daysOfWeek.Split(',');

                // Build the query to count sessions for each selected day
                string query = "SELECT COUNT(*) FROM Sessions " +
                               "WHERE Coach_Id = @CoachId AND Days_Of_Week IN (" + string.Join(",", selectedDays.Select(_ => "@Day")) + ")";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CoachId", coachId);

                    // Add parameters for each selected day
                    foreach (var day in selectedDays)
                    {
                        command.Parameters.AddWithValue("@Day", day);
                    }

                    int sessionCount = (int)command.ExecuteScalar();

                    // Check if all selected days have sessions
                    return sessionCount == selectedDays.Length;
                }
            }
        }

        private bool IsTimeSlotOccupied(int coachId, TimeSpan startTime, TimeSpan endTime, string daysOfWeek)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Split the daysOfWeek string into individual days
                string[] selectedDays = daysOfWeek.Split(',');

                // Build the query to count overlapping sessions for each selected day
                string query = "SELECT COUNT(*) FROM Sessions " +
                               "WHERE Coach_Id = @CoachId AND Days_Of_Week IN (" + string.Join(",", selectedDays.Select(_ => "@Day")) + ") " +
                               "AND NOT (End_Time <= @StartTime OR Start_Time >= @EndTime)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CoachId", coachId);
                    command.Parameters.AddWithValue("@StartTime", startTime);
                    command.Parameters.AddWithValue("@EndTime", endTime);

                    // Add parameters for each selected day
                    foreach (var day in selectedDays)
                    {
                        command.Parameters.AddWithValue("@Day", day);
                    }

                    int overlappingSessions = (int)command.ExecuteScalar();

                    // Check if any selected day has an overlapping session
                    return overlappingSessions > 0;
                }
            }
        }




        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
    {

    }

    private void numericUpDown1_ValueChanged(object sender, EventArgs e)
    {

    }



    private void txtcoach_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}}