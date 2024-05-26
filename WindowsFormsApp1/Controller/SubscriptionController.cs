using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.models;

namespace WindowsFormsApp1.Controllers
{
    public class SubscriptionController
    {
        private const string connectionString = "Data Source=LAPTOP-SA22HL97\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;";
        private DBConnection dbConnection;

        public SubscriptionController()
        {
            dbConnection = DBConnection.GetInstance();
        }
        public Subscription CreateSubscription(bool wantsPrivateCoach, List<Services> selectedServices)
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

        public int InsertSubscriptionIntoDatabase(Subscription subscription)
        {
            try
            {
                using (SqlConnection connection = dbConnection.GetConnection())
                {
                  

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

                        int coachId = GetRandomCoachIdFromDatabase();

                        if (subscription is PlatinumSubscription)
                        {
                            subscriptionCommand.Parameters.AddWithValue("@CoachId", coachId);
                        }
                        else
                        {
                            subscriptionCommand.Parameters.AddWithValue("@CoachId", DBNull.Value);
                        }

                        int subscriptionId = Convert.ToInt32(subscriptionCommand.ExecuteScalar());
                        Console.WriteLine($"Subscription Inserted. ID: {subscriptionId}");
                        dbConnection.CloseConnection(connection);
                        return subscriptionId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting subscription into the database: " + ex.Message);
            }
        }

        private int GetRandomCoachIdFromDatabase()
        {
            try
            {
                using (SqlConnection connection = dbConnection.GetConnection())
                {
                    

                    string selectCoachIdQuery = "SELECT TOP 1 Id FROM Coach ORDER BY NEWID();";
                    using (SqlCommand selectCoachIdCommand = new SqlCommand(selectCoachIdQuery, connection))
                    {
                        object result = selectCoachIdCommand.ExecuteScalar();
                        dbConnection.CloseConnection(connection);
                        if (result != null && result != DBNull.Value)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            throw new Exception("No coach found in the database.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving random coach ID: " + ex.Message);
            }
        }


        public Subscription CreateSubscriptionFromFormInputs(DateTime startDate, int durationMonths, bool wantsPrivateCoach, bool wantsServices)
        {
            DateTime endDate = startDate.AddMonths(durationMonths);

            List<Services> selectedServices = new List<Services>();

            if (wantsServices)
            {
                selectedServices.Add(new Services("Spa", 50));
                selectedServices.Add(new Services("Sauna", 50));
                selectedServices.Add(new Services("Jacuzzi", 50));
            }

            Subscription subscription = CreateSubscription(wantsPrivateCoach, selectedServices);
            subscription.EndDate = endDate;

            return subscription;
        }
        public Subscription GetSubscriptionDetails(int subscriptionId)
        {
            SqlConnection connection = null;
            try
            {
                connection = dbConnection.GetConnection();
                string selectSubscriptionQuery = @"
    SELECT sub.Id, sub.Type, sub.start_date, sub.end_date, sub.totalprice, 
           sub.coach_id, sub.status, coach.Name AS CoachName
    FROM subscriptions AS sub
    LEFT JOIN Coach AS coach ON sub.coach_id = coach.id
    WHERE sub.Id = @SubscriptionId;
";

                using (SqlCommand subscriptionCommand = new SqlCommand(selectSubscriptionQuery, connection))
                {
                    subscriptionCommand.Parameters.AddWithValue("@SubscriptionId", subscriptionId);

                    using (SqlDataReader reader = subscriptionCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Subscription subscription;
                            string subscriptionType = reader["Type"].ToString().Trim();
                            DateTime startDate = Convert.ToDateTime(reader["start_date"]);
                            DateTime endDate = Convert.ToDateTime(reader["end_date"]);
                            double totalPrice = Convert.ToDouble(reader["totalprice"]);
                            string status = Convert.ToString(reader["status"]);
                            int coachId = reader["coach_id"] != DBNull.Value ? Convert.ToInt32(reader["coach_id"]) : 0;
                            string coachName = reader["CoachName"].ToString();
                            if (subscriptionType == "Silver")
                            {
                                subscription = new SilverSubscription();
                            }
                            else if (subscriptionType == "Gold")
                            {
                                subscription = new GoldSubscription(new List<Services>());
                            }
                            else if (subscriptionType == "Platinum")
                            {
                                // Fetch coach details if coachId is available
                                Coach coach =new  Coach(coachId, coachName);
                                subscription = new PlatinumSubscription(new List<Services>(), coach);
                            }
                            else
                            {
                                throw new Exception($"Unsupported subscription type: {subscriptionType}");
                            }

                            subscription.Id = subscriptionId;
                            subscription.StartDate = startDate;
                            subscription.EndDate = endDate;
                            subscription.TotalPrice = totalPrice;
                            subscription.Status = status;
                            return subscription;
                        }
                        else
                        {
                            throw new Exception($"Subscription with ID {subscriptionId} not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving subscription details: " + ex.Message);
            }
            finally
            {
                // Close the connection in the finally block to ensure it is closed regardless of exceptions
                if (connection != null)
                {
                    dbConnection.CloseConnection(connection);
                }
            }
        }



       
        public void EditSubscription(int subscriptionId, DateTime startDate, int durationMonths, List<Services> newServices, bool wantsPrivateCoach)
        {
            SqlConnection connection = null;
            try
            {
                // Retrieve the existing subscription details
                Subscription existingSubscription = GetSubscriptionDetails(subscriptionId);
                if (existingSubscription == null)
                {
                    throw new Exception("Subscription not found.");
                }

                // Create a new subscription with the new details
                Subscription newSubscription = CreateSubscription(wantsPrivateCoach, newServices);

                // Calculate the new end date based on the start date and duration in months
                DateTime endDate = startDate.AddMonths(durationMonths);

                // Assign the existing subscription ID and new dates to the new subscription
                newSubscription.Id = existingSubscription.Id;
                newSubscription.StartDate = startDate;
                newSubscription.EndDate = endDate;

                // Update the database with the new subscription details
                connection = dbConnection.GetConnection();
                string updateSubscriptionQuery = @"
        UPDATE subscriptions
        SET Type = @Type, start_date = @StartDate, end_date = @EndDate, totalprice = @TotalPrice, coach_id = @CoachId
        WHERE Id = @SubscriptionId;";

                using (SqlCommand updateCommand = new SqlCommand(updateSubscriptionQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@Type", newSubscription.Name);
                    updateCommand.Parameters.AddWithValue("@StartDate", newSubscription.StartDate);
                    updateCommand.Parameters.AddWithValue("@EndDate", newSubscription.EndDate);
                    updateCommand.Parameters.AddWithValue("@TotalPrice", newSubscription.CalculateTotalPrice());

                    if (newSubscription is PlatinumSubscription)
                    {
                        updateCommand.Parameters.AddWithValue("@CoachId", GetRandomCoachIdFromDatabase());
                    }
                    else
                    {
                        updateCommand.Parameters.AddWithValue("@CoachId", DBNull.Value);
                    }

                    updateCommand.Parameters.AddWithValue("@SubscriptionId", newSubscription.Id);

                    updateCommand.ExecuteNonQuery();
                    Console.WriteLine($"Subscription Updated. ID: {newSubscription.Id}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error editing subscription: " + ex.Message);
            }
            finally
            {
                if (connection != null)
                {
                    dbConnection.CloseConnection(connection);
                }
            }
        }



    }




}
