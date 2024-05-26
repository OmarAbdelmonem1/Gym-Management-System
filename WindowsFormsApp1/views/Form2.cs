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
using System.Windows.Forms.DataVisualization.Charting;
using WindowsFormsApp1.views;
using WindowsFormsApp1.Controller;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        private DBConnection dbConnection;
        private SqlConnection connection;

        private Timer autoCheckOutTimer;
        private CheckInController checkInController;

        public Form2()
        {
            InitializeComponent();
            dbConnection = DBConnection.GetInstance();
            connection = dbConnection.GetConnection();
            LoadSubscriptionCounts();

            checkInController = new CheckInController();
            InitializeTimer();

            var counts = FetchSubscriberCounts();
            int platinumCount = counts.PlatinumCount;
            int goldCount = counts.GoldCount;
            int silverCount = counts.SilverCount;

            // Now you can use platinumCount, goldCount, and silverCount as needed
            Console.WriteLine("Platinum subscribers count: " + platinumCount);
            Console.WriteLine("Gold subscribers count: " + goldCount);
            Console.WriteLine("Silver subscribers count: " + silverCount);

            if (connection.State == System.Data.ConnectionState.Open)
            {
                LoadData();
            }
            else
            {
                MessageBox.Show("Failed to open database connection. Dashboard data cannot be loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void LoadSubscriptionCounts()
        {
            var counts = FetchSubscriberCounts();

            // Clear existing series data
            this.subscriptionChart.Series.Clear();

            // Create a new series and set its chart type
            Series series = new Series
            {
                Name = "Subscribers",
                IsVisibleInLegend = true,
                ChartType = SeriesChartType.Doughnut
            };

            // Add data points to the series
            series.Points.AddXY("Platinum", counts.PlatinumCount);
            series.Points.AddXY("Gold", counts.GoldCount);
            series.Points.AddXY("Silver", counts.SilverCount);

            // Add the series to the chart
            subscriptionChart.Series.Add(series);

            // Customize chart appearance
            subscriptionChart.ChartAreas[0].AxisX.Title = "Subscription Type";
            subscriptionChart.ChartAreas[0].AxisY.Title = "Count";
            
        }


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
                    platinumCount = platinumCount+1;
                }
                else if (subscriptionType.Contains("Gold"))
                {
                    goldCount = goldCount+1;
                }
                else if (subscriptionType.Contains("Silver"))
                {
                    silverCount = silverCount +1;
                }
            }
            return (platinumCount, goldCount, silverCount);
        }






        private void LoadData()
        {
            try
            {
                // Fetch and display gym statistics
                int totalMembers = FetchTotalMembers();
                int totalEqu = FetchTotalEqu();
                int totalSession = FetchTotalSession();

                label1.Text = totalMembers.ToString();
                label2.Text =  totalEqu.ToString();
                label3.Text = totalSession.ToString();

               

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbConnection.CloseConnection(connection);
            }
        }

        private int FetchTotalMembers()
        {
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Member", connection))
            {
                return (int)command.ExecuteScalar();
            }
        }
        private int FetchTotalEqu()
        {
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Equipment", connection))
            {
                return (int)command.ExecuteScalar();
            }
        }

        private int FetchTotalSession()
        {
            using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Sessions", connection))
            {
                return (int)command.ExecuteScalar();
            }
        }
       



        private void txtname_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new MemberRegisterForm();
            form.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new Form5();
            form.ShowDialog();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form form = new MembersTableForm();
            form.ShowDialog();
        }

        private void label6_Click_1(object sender, EventArgs e)
        {
           
        }




        

       

        private void InitializeTimer()
        {
            autoCheckOutTimer = new Timer();
            autoCheckOutTimer.Interval = 30000; // 1 minute interval

            autoCheckOutTimer.Tick += new EventHandler(AutoCheckOutTimer_Tick);
            autoCheckOutTimer.Start();
        }

        private void AutoCheckOutTimer_Tick(object sender, EventArgs e)
        {
            checkInController.AutoCheckOut();
        }

        private void btnCheckIn_Click_1(object sender, EventArgs e)
        {
            if (int.TryParse(txtMemberId.Text, out int memberId))
            {
                checkInController.CheckInMember(memberId);
                //LoadCheckInRecords(memberId);
            }
            else
            {
                MessageBox.Show("Please enter a valid member ID.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtMemberId_TextChanged(object sender, EventArgs e)
        {

        }
    }
}