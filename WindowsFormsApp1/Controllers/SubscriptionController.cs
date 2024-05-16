using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
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
    }
}

