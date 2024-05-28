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
using WindowsFormsApp1.views.DashBoardForms;
using WindowsFormsApp1.Controller;

namespace WindowsFormsApp1
{
    public partial class DashBoardForm : Form
    {
        private DBConnection dbConnection;
        private SqlConnection connection;

        private Timer autoCheckOutTimer;
        private CheckInController checkInController;

        private DashboardController dashboardController;

        public DashBoardForm()
        {
            InitializeComponent();
            if(SESSION.UserRole== "Receptionist")
            {
                button6.Hide();
            }
            checkInController = new CheckInController();
            dashboardController = new DashboardController();

            dbConnection = DBConnection.GetInstance();
            connection = dbConnection.GetConnection();
            LoadSubscriptionCounts();

            
            InitializeTimer();

            var counts = dashboardController.FetchSubscriberCounts();
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
            var counts = dashboardController.FetchSubscriberCounts();

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




        


        private void LoadData()
        {
            try
            {
                // Fetch and display gym statistics
                int totalMembers = dashboardController.FetchTotalMembers();
                int totalEqu = dashboardController.FetchTotalEqu();
                int totalSession = dashboardController.FetchTotalSession();
                int totalcheckin = dashboardController.FetchTotalCheckin();

                label1.Text = totalMembers.ToString();
                label2.Text =  totalEqu.ToString();
                label3.Text = totalSession.ToString();
                label9.Text = totalcheckin.ToString();

               

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
            Form form = new EquipmentForm();
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
                
                Console.WriteLine(dashboardController.FetchTotalCheckin());
                if (dashboardController.FetchTotalCheckin() >= 4)
                {
                    MessageBox.Show("Limit Reached come later ", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    checkInController.CheckInMember(memberId);
                    this.Hide();
                    Form form = new DashBoardForm();
                    form.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid member ID.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtMemberId_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            SessionForm f = new SessionForm();
            f.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            CoachesTableForm f = new CoachesTableForm();
            f.ShowDialog();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form f = new DashBoardForm();
            f.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form f = new CredentialsForm();
            f.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Hide();
            Form f = new LoginForm();
            f.ShowDialog();
            SESSION.Clear();
        }
    }
}