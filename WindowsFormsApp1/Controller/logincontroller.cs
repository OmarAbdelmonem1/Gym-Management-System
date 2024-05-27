using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using WindowsFormsApp1.models;

namespace WindowsFormsApp1.Controllers
{
    public class SessionController
    {
        private string connectionString = "Data Source=LAPTOP-SA22HL97\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;";
        private DBConnection dbConnection;

        public SessionController()
        {
            dbConnection = DBConnection.GetInstance();
        }
        public List<Coach> GetCoaches()
        {
            List<Coach> coaches = new List<Coach>();

            try
            {
                using (SqlConnection connection = dbConnection.GetConnection())
                {
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
                    dbConnection.CloseConnection(connection);

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
                using (SqlConnection connection = dbConnection.GetConnection())
                {
                    if (IsCoachAlreadyBooked( coachId, daysOfWeekString))
                    {
                        throw new Exception("The selected coach already has a session on the chosen day.");
                    }

                    if (IsTimeSlotOccupied( coachId, startTime, endTime, daysOfWeekString))
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
                        dbConnection.CloseConnection(connection);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding session: " + ex.Message);
            }
        }
        private bool IsCoachAlreadyBooked(int coachId, string daysOfWeek)
        {
            try
            {
                using (SqlConnection connection = dbConnection.GetConnection())
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
                        dbConnection.CloseConnection(connection);
                        return sessionCount == selectedDays.Length;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking coach booking status: " + ex.Message);
                // Handle or log the exception as needed
                return false; // Return false indicating that an error occurred
            }
        }


        private bool IsTimeSlotOccupied(int coachId, TimeSpan startTime, TimeSpan endTime, string daysOfWeek)
        {
            try
            {
                using (SqlConnection connection = dbConnection.GetConnection())
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
                        dbConnection.CloseConnection(connection);
                        return overlappingSessions > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking time slot occupancy: " + ex.Message);
                // Handle or log the exception as needed
                return false; // Return false indicating that an error occurred
            }
        }

        public List<Session> GetAllSessions()
        {
            List<Session> sessions = new List<Session>();

            try
            {
                using (SqlConnection connection = dbConnection.GetConnection())
                {
                  
                    string query = "SELECT s.Id, s.Name, s.Coach_Id, s.MaxCapacity, s.Start_Time, s.End_Time, s.Days_Of_Week, s.Description, c.Name AS CoachName " +
                                   "FROM Sessions s " +
                                   "INNER JOIN Coach c ON s.Coach_Id = c.Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Session session = new Session
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Name = reader["Name"].ToString(),
                                    MaxCapacity = Convert.ToInt32(reader["MaxCapacity"]),
                                    StartTime = (TimeSpan)reader["Start_Time"],
                                    EndTime = (TimeSpan)reader["End_Time"],
                                    Description = reader["Description"].ToString(),
                                    Coach = new Coach
                                    {
                                        id = Convert.ToInt32(reader["Coach_Id"]),
                                        Name = reader["CoachName"].ToString()
                                    },
                                    selectedDays = reader["Days_Of_Week"].ToString().Split(',').ToList()
                                };

                                sessions.Add(session);
                            }
                        }
                    }
                    dbConnection.CloseConnection(connection);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving sessions: " + ex.Message);
            }

            return sessions;
        }
        public void EditSession(Session session)
        {
            try
            {
                using (SqlConnection connection = dbConnection.GetConnection())
                {
                  

                    // Check if the coach is already booked on the chosen days
                    if (IsCoachAlreadyBooked( session.Coach.id, string.Join(",", session.selectedDays)))
                    {
                        throw new Exception("The selected coach already has a session on the chosen day.");
                    }

                    // Check for overlapping sessions
                    if (IsTimeSlotOccupied( session.Coach.id, session.StartTime, session.EndTime, string.Join(",", session.selectedDays)))
                    {
                        throw new Exception("There is an overlapping session for the selected coach during the chosen time.");
                    }

                    // Check session duration
                    TimeSpan sessionDuration = session.EndTime - session.StartTime;
                    if (sessionDuration < TimeSpan.FromHours(1) || sessionDuration > TimeSpan.FromHours(3))
                    {
                        throw new Exception("Session duration must be between 1 hour and 3 hours.");
                    }

                    // Check maximum capacity
                    if (session.MaxCapacity > 10)
                    {
                        throw new Exception("Maximum capacity cannot exceed 10 attendees.");
                    }

                    // Update session details in the database
                    string query = "UPDATE Sessions " +
                                   "SET Name = @Name, Coach_Id = @CoachId, MaxCapacity = @MaxCapacity, " +
                                   "Start_Time = @StartTime, End_Time = @EndTime, Days_Of_Week = @DaysOfWeek, Description = @Description " +
                                   "WHERE Id = @SessionId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", session.Name);
                        command.Parameters.AddWithValue("@CoachId", session.Coach.id);
                        command.Parameters.AddWithValue("@MaxCapacity", session.MaxCapacity);
                        command.Parameters.AddWithValue("@StartTime", session.StartTime);
                        command.Parameters.AddWithValue("@EndTime", session.EndTime);
                        command.Parameters.AddWithValue("@DaysOfWeek", string.Join(",", session.selectedDays));
                        command.Parameters.AddWithValue("@Description", session.Description);
                        command.Parameters.AddWithValue("@SessionId", session.Id);

                        command.ExecuteNonQuery();
                    }
                    dbConnection.CloseConnection(connection);
                }

            }
            
            catch (Exception ex)
            {
                throw new Exception("Error editing session: " + ex.Message);
            }
        }


        public void DeleteSession(int sessionId)
        {
            try
            {
                using (SqlConnection connection = dbConnection.GetConnection())
                {
                  
                    string query = "DELETE FROM Sessions WHERE Id = @SessionId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SessionId", sessionId);
                        command.ExecuteNonQuery();
                    }
                    dbConnection.CloseConnection(connection);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting session: " + ex.Message);
            }
        }
        // In your SessionController
        public List<Member> GetAllMembers(int sessionId)
        {
            List<Member> members = new List<Member>();

            try
            {
                using (SqlConnection connection = dbConnection.GetConnection())
                {
                   
                    string query = "SELECT SM.session_id, SM.member_id, M.Name " +
                                   "FROM Sessions_Members SM " +
                                   "JOIN Member M ON SM.member_id = M.memberid " +
                                   "WHERE SM.session_id = @SessionId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SessionId", sessionId);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int memberId = Convert.ToInt32(reader["member_id"]);
                                string memberName = reader["Name"].ToString();
                                int session_id = Convert.ToInt32(reader["session_id"]);

                                Member member = new Member(memberId, memberName);
                                members.Add(member);
                            }
                        }
                     
                    }
                    dbConnection.CloseConnection(connection);
                }

            }

            catch (Exception ex)
            {
                throw new Exception("Error retrieving members for session: " + ex.Message);
            }

            return members;
        }
        public void AddMemberToSession(int memberId, int sessionId)
        {
            try
            {
                using (SqlConnection connection = dbConnection.GetConnection())
                {
                  
                    string query = "INSERT INTO Sessions_Members (member_id, session_id) VALUES (@MemberId, @SessionId)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MemberId", memberId);
                        command.Parameters.AddWithValue("@SessionId", sessionId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new Exception("Failed to add member to session.");
                        }
                    }
                    dbConnection.CloseConnection(connection);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding member to session: " + ex.Message);
            }
        }

            public void DeleteMemberFromSession(int memberId, int sessionId)
        {
            try
            {
                using (SqlConnection connection = dbConnection.GetConnection())
                {
                

                    string query = "DELETE FROM Sessions_Members WHERE member_id = @MemberId AND session_id = @SessionId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MemberId", memberId);
                        command.Parameters.AddWithValue("@SessionId", sessionId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            throw new Exception("The specified member is not associated with the session.");
                        }
                    }
                    dbConnection.CloseConnection(connection);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting member from session: " + ex.Message);
            }
        }





    }
}
