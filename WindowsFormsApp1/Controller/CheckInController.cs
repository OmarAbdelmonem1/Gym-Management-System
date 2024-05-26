
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using WindowsFormsApp1.models;


namespace WindowsFormsApp1.Controller
{
    public class CheckInController
    {
        public void CheckInMember(int memberId)
        {
            // Check if the member ID exists
            if (!IsMemberExists(memberId))
            {
                MessageBox.Show("Member ID does not exist.", "Check In Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit the method if member ID doesn't exist
            }

            // Automatically check out any existing check-ins for this member
            CheckOutMember(memberId);

            DBConnection dbConn = DBConnection.GetInstance();
            using (SqlConnection conn = dbConn.GetConnection())
            {
                string query = "INSERT INTO checkins (member_id, checkin_time) VALUES (@memberId, @checkinTime)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@memberId", memberId);
                    cmd.Parameters.AddWithValue("@checkinTime", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
            MessageBox.Show("Member checked in successfully.", "Check In Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
        }

        private bool IsMemberExists(int MemberId)
        {
            bool exists = false;
            DBConnection dbConn = DBConnection.GetInstance();
            using (SqlConnection conn = dbConn.GetConnection())
            {
                string query = "SELECT COUNT(*) FROM Member WHERE MemberId = @MemberId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MemberId", MemberId);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        exists = Convert.ToInt32(result) > 0;
                    }
                }
            }
            return exists;
        }


        public void CheckOutMember(int memberId)
        {
            DBConnection dbConn = DBConnection.GetInstance();
            using (SqlConnection conn = dbConn.GetConnection())
            {
                string query = "DELETE FROM checkins WHERE member_id = @memberId AND checkout_time IS NULL";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@memberId", memberId);
                    cmd.Parameters.AddWithValue("@checkoutTime", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AutoCheckOut()
        {
            DBConnection dbConn = DBConnection.GetInstance();
            using (SqlConnection conn = dbConn.GetConnection())
            {
                string query = "DELETE FROM checkins WHERE checkout_time IS NULL AND DATEADD(minute, 1, checkin_time) <= @currentTime";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@currentTime", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable GetCheckInRecords(int memberId)
        {
            DataTable dt = new DataTable();
            DBConnection dbConn = DBConnection.GetInstance();
            using (SqlConnection conn = dbConn.GetConnection())
            {
                string query = "SELECT * FROM checkins WHERE member_id = @memberId";
                using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@memberId", memberId);
                    da.Fill(dt);
                }
            }
            return dt;
        }
    }
}
