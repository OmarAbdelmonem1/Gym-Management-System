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

                    int coachId = GetRandomCoachIdFromDatabase(connection);

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
                    return subscriptionId;
                }
            }
        }

        private int GetRandomCoachIdFromDatabase(SqlConnection connection)
        {
            string selectCoachIdQuery = "SELECT TOP 1 id FROM Coach ORDER BY NEWID();";
            SqlCommand selectCoachIdCommand = new SqlCommand(selectCoachIdQuery, connection);
            object result = selectCoachIdCommand.ExecuteScalar();

            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                throw new Exception("No coach found in the database.");
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectSubscriptionQuery = @"
                    SELECT Id, Type, start_date, end_date, totalprice, coach_id
                    FROM subscriptions
                    WHERE Id = @SubscriptionId;";

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
                            int coachId = reader["coach_id"] != DBNull.Value ? Convert.ToInt32(reader["coach_id"]) : 0;

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
                                Coach coach = coachId != 0 ? GetCoachDetails(coachId) : null;
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

                            return subscription;
                        }
                        else
                        {
                            throw new Exception($"Subscription with ID {subscriptionId} not found.");
                        }
                    }
                }
            }
        }

        private Coach GetCoachDetails(int coachId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string selectCoachQuery = @"
                    SELECT id, name, specialization
                    FROM Coach
                    WHERE id = @CoachId;";

                using (SqlCommand coachCommand = new SqlCommand(selectCoachQuery, connection))
                {
                    coachCommand.Parameters.AddWithValue("@CoachId", coachId);

                    using (SqlDataReader reader = coachCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string coachName = reader["name"].ToString();
                            string specialization = reader["specialization"].ToString();

                            return new Coach { id = coachId, Name = coachName,  };
                        }
                        else
                        {
                            throw new Exception($"Coach with ID {coachId} not found.");
                        }
                    }
                }
            }
        }
    }




}
