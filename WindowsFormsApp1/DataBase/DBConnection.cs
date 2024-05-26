using System;
using System.Data;
using System.Data.SqlClient;

public class DBConnection
{
    // Private static instance variable
    private static DBConnection instance;

    // Connection string for the database
    private string connectionString = "Data Source=LAPTOP-SA22HL97\\SQLEXPRESS;Initial Catalog=Gym;Integrated Security=True;";

    // Private constructor to prevent instantiation
    private DBConnection() { }

    // Public static method to get the instance
    public static DBConnection GetInstance()
    {
        // Lazy initialization: if the instance is null, create a new instance
        if (instance == null)
        {
            instance = new DBConnection();
        }
        return instance;
    }

    // Method to establish a connection to the database
    public SqlConnection GetConnection()
    {
        SqlConnection connection = new SqlConnection(connectionString);
        try
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                Console.WriteLine("Database connection opened.");
            }
            else
            {
                Console.WriteLine("Database connection already open.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error opening database connection: " + ex.Message);
        }
        return connection;
    }

    // Method to close the database connection
    public void CloseConnection(SqlConnection connection)
    {
        try
        {
            connection.Close();
            Console.WriteLine("Database connection closed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error closing database connection: " + ex.Message);
        }
    }
}
