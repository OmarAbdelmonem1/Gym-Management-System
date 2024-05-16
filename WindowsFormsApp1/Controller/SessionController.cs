using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using WindowsFormsApp1.models;

namespace WindowsFormsApp1.Controllers
{
    public class SessionController
    {
        private string connectionString = "Data Source=LAPTOP-SA22HL97\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;";

        public List<Coach> GetCoaches()
        {
            List<Coach> coaches = new List<Coach>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Name FROM Coach";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int coachId = Convert.ToInt32(reader["Id"]);
                                string coachName = reader["Name"].ToString();

                                Coach coach = new Coach(coachId, coachName);
                                coaches.Add(coach);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving coach names: " + ex.Message);
            }

            return coaches;
        }

        public void AddSession(string name, int coachId, int maxCapacity, TimeSpan startTime, TimeSpan endTime, List<string> daysOfWeek, string description)
        {
            string daysOfWeekString = string.Join(",", daysOfWeek);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    if (IsCoachAlreadyBooked(connection, coachId, daysOfWeekString))
                    {
                        throw new Exception("The selected coach already has a session on the chosen day.");
                    }

                    if (IsTimeSlotOccupied(connection, coachId, startTime, endTime, daysOfWeekString))
                    {
                        throw new Exception("There is an overlapping session for the selected coach during the chosen time.");
                    }

                    TimeSpan sessionDuration = endTime - startTime;
                    if (sessionDuration < TimeSpan.FromHours(1) || sessionDuration > TimeSpan.FromHours(3))
                    {
                        throw new Exception("Session duration must be between 1 hour and 3 hours.");
                    }

                    if (maxCapacity > 10)
                    {
                        throw new Exception("Maximum capacity cannot exceed 10 attendees.");
                    }

                    string query = "INSERT INTO Sessions (Name, Coach_Id, MaxCapacity, Start_Time, End_Time, Days_Of_Week, Description) " +
                                   "VALUES (@Name, @CoachId, @MaxCapacity, @StartTime, @EndTime, @DaysOfWeek, @Description)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@CoachId", coachId);
                        command.Parameters.AddWithValue("@MaxCapacity", maxCapacity);
                        command.Parameters.AddWithValue("@StartTime", startTime);
                        command.Parameters.AddWithValue("@EndTime", endTime);
                        command.Parameters.AddWithValue("@DaysOfWeek", daysOfWeekString);
                        command.Parameters.AddWithValue("@Description", description);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding session: " + ex.Message);
            }
        }

        private bool IsCoachAlreadyBooked(SqlConnection connection, int coachId, string daysOfWeek)
        {
            string[] selectedDays = daysOfWeek.Split(',');

            string query = "SELECT COUNT(*) FROM Sessions " +
                           "WHERE Coach_Id = @CoachId AND Days_Of_Week IN (" + string.Join(",", selectedDays.Select(_ => "@Day")) + ")";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CoachId", coachId);

                foreach (var day in selectedDays)
                {
                    command.Parameters.AddWithValue("@Day", day);
                }

                int sessionCount = (int)command.ExecuteScalar();
                return sessionCount == selectedDays.Length;
            }
        }

        private bool IsTimeSlotOccupied(SqlConnection connection, int coachId, TimeSpan startTime, TimeSpan endTime, string daysOfWeek)
        {
            string[] selectedDays = daysOfWeek.Split(',');

            string query = "SELECT COUNT(*) FROM Sessions " +
                           "WHERE Coach_Id = @CoachId AND Days_Of_Week IN (" + string.Join(",", selectedDays.Select(_ => "@Day")) + ") " +
                           "AND NOT (End_Time <= @StartTime OR Start_Time >= @EndTime)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CoachId", coachId);
                command.Parameters.AddWithValue("@StartTime", startTime);
                command.Parameters.AddWithValue("@EndTime", endTime);

                foreach (var day in selectedDays)
                {
                    command.Parameters.AddWithValue("@Day", day);
                }

                int overlappingSessions = (int)command.ExecuteScalar();
                return overlappingSessions > 0;
            }
        }
    }
}
