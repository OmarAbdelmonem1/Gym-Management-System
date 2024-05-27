// using System;
// using System.Data;
// using System.Data.SqlClient;
// using WindowsFormsApp1.models;

// namespace WindowsFormsApp1.Controllers
// {
//     public class CoachController
//     {
//         private const string connectionString = "Data Source=LAPTOP-SA22HL97\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;";

//         private DBConnection dbConnection;

//         public CoachController()
//         {
//             dbConnection = DBConnection.GetInstance();
//         }

//         public void InsertCoachIntoDatabase(Coach coach)
//         {
//             try
//             {
//                 using (SqlConnection connection = dbConnection.GetConnection())
//                 {
//                     string insertCoachQuery = @"
//                 INSERT INTO coach (Name, Age, Gender, ContactNumber, Salary, WorkingDays, Address, PrivateTrainingCost) 
// VALUES (@Name, @Age, @Gender, @ContactNumber, @Salary, @WorkingDays, @Address, @PrivateTrainingCost);
// ";

//                     using (SqlCommand coachCommand = new SqlCommand(insertCoachQuery, connection))
//                     {
//                         coachCommand.Parameters.AddWithValue("@Name", coach.Name);
//                         coachCommand.Parameters.AddWithValue("@Age", coach.Age);
//                         coachCommand.Parameters.AddWithValue("@Gender", coach.Gender);
//                         coachCommand.Parameters.AddWithValue("@ContactNumber", coach.ContactNumber);
//                         coachCommand.Parameters.AddWithValue("@Salary", coach.Salary);
//                         coachCommand.Parameters.AddWithValue("@WorkingDays", coach.WorkingDays);
//                         coachCommand.Parameters.AddWithValue("@Address", coach.Address);
//                         coachCommand.Parameters.AddWithValue("@PrivateTrainingCost", coach.private_training_cost);
                
//                         int rowsAffected = coachCommand.ExecuteNonQuery();
//                         Console.WriteLine($"Coach Inserted. Rows Affected: {rowsAffected}");
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine("Error inserting coach into database: " + ex.Message);
//                 // Handle or log the exception as needed
//             }
//         }

//         public DataTable GetAllCoaches()
//         {
//             DataTable coachesTable = new DataTable();

//             using (SqlConnection connection = dbConnection.GetConnection())
//             {
//                 string selectCoachesQuery = "SELECT * FROM Coach;";

//                 using (SqlDataAdapter adapter = new SqlDataAdapter(selectCoachesQuery, connection))
//                 {
//                     adapter.Fill(coachesTable);
//                 }
//             }

//             return coachesTable;
//         }

//         public void UpdateCoach(Coach coach)
//         {
//             try
//             {
//                 using (SqlConnection connection = dbConnection.GetConnection())
//                 {
//                     string updateCoachQuery = @"
//                         UPDATE Coach 
//                         SET Name = @Name, Age = @Age, Gender = @Gender, ContactNumber = @ContactNumber, Salary = @Salary, 
//                             WorkingDays = @WorkingDays, Address = @Address, PrivateTrainingCost = @PrivateTrainingCost
//                         WHERE Id = @Id;";

//                     using (SqlCommand coachCommand = new SqlCommand(updateCoachQuery, connection))
//                     {
//                         coachCommand.Parameters.AddWithValue("@Name", coach.Name);
//                         coachCommand.Parameters.AddWithValue("@Age", coach.Age);
//                         coachCommand.Parameters.AddWithValue("@Gender", coach.Gender);
//                         coachCommand.Parameters.AddWithValue("@ContactNumber", coach.ContactNumber);
//                         coachCommand.Parameters.AddWithValue("@Salary", coach.Salary);
//                         coachCommand.Parameters.AddWithValue("@WorkingDays", coach.WorkingDays);
//                         coachCommand.Parameters.AddWithValue("@Address", coach.Address);
//                         coachCommand.Parameters.AddWithValue("@PrivateTrainingCost", coach.private_training_cost);
//                         coachCommand.Parameters.AddWithValue("@Id", coach.id);
//                         Console.WriteLine($"Coach idd: {coach.id}");
//                         int rowsAffected = coachCommand.ExecuteNonQuery();
//                         Console.WriteLine($"Coach Updated. Rows Affected: {rowsAffected}");
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine("Error updating coach: " + ex.Message);
//                 // Handle or log the exception as needed
//             }
//         }

//         public void DeleteCoach(int coachId)
//         {
//             try
//             {
//                 using (SqlConnection connection = dbConnection.GetConnection())
//                 {
//                     string deleteCoachQuery = "DELETE FROM Coach WHERE Id = @Id;";

//                     using (SqlCommand coachCommand = new SqlCommand(deleteCoachQuery, connection))
//                     {
//                         coachCommand.Parameters.AddWithValue("@Id", coachId);

//                         int rowsAffected = coachCommand.ExecuteNonQuery();
//                         Console.WriteLine($"Coach Deleted. Rows Affected: {rowsAffected}");
//                     }
//                 }
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine("Error deleting coach: " + ex.Message);
//                 // Handle or log the exception as needed
//             }
//         }
//     }
// }
