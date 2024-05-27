using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using WindowsFormsApp1.models;
using WindowsFormsApp1.views;

namespace WindowsFormsApp1.Controller
{
    public class DashboardController
    {
        private DBConnection dbConnection = DBConnection.GetInstance();
        //private DBConnection dbConnection;
        private SqlConnection connection;
        //dbConnection = DBConnection.GetInstance();

        public (int PlatinumCount, int GoldCount, int SilverCount) FetchSubscriberCounts()
        {
            List<string> subscriptionTypes = new List<string>();
            int platinumCount = 0;
            int goldCount = 0;
            int silverCount = 0;

            using (SqlConnection connection = dbConnection.GetConnection())
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    string query = "SELECT [Type] FROM subscriptions;";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        try
                        {
                            SqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                string subscriptionType = reader["Type"].ToString();

                                subscriptionTypes.Add(subscriptionType);
                            }
                            reader.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error executing SQL query: " + ex.Message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Connection state is not open.");
                }
            }

            Console.WriteLine(subscriptionTypes);
            // Count the occurrences of each subscription type
            foreach (string subscriptionType in subscriptionTypes)
            {
                Console.WriteLine(subscriptionType);
                Console.WriteLine("-----------------");
                if (subscriptionType.Contains("Platinum"))
                {
                    platinumCount = platinumCount + 1;
                }
                else if (subscriptionType.Contains("Gold"))
                {
                    goldCount = goldCount + 1;
                }
                else if (subscriptionType.Contains("Silver"))
                {
                    silverCount = silverCount + 1;
                }
            }
            return (platinumCount, goldCount, silverCount);
        }




        public int FetchTotalMembers()
        {
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Member", dbConnection.GetConnection()))
            {
                return (int)command.ExecuteScalar();
            }
        }
        public int FetchTotalEqu()
        {
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Equipment", dbConnection.GetConnection()))
            {
                return (int)command.ExecuteScalar();
            }
        }

        public int FetchTotalSession()
        {
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Sessions", dbConnection.GetConnection()))
            {
                return (int)command.ExecuteScalar();
            }
        }
        public int FetchTotalCheckin()
        {
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM checkins", dbConnection.GetConnection()))
            {
                return (int)command.ExecuteScalar();
            }
        }

    }
}
