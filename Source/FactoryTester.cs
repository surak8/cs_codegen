using System;
using System.Data;
using System.Data.Common;

public class FactoryTester {
    DbProviderFactory _factory;

    public FactoryTester(DbProviderFactory f) {
        _factory = f;
    }

    public void doTests() {
        DbConnection conn = createConnection();
        DbCommandSelect(conn);
        ExecuteDbCommand(conn);
    }

    DbConnection createConnection() {
        return _factory.CreateConnection();
    }

    //    https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/dbconnection-dbcommand-and-dbexception

    // Takes a DbConnection and creates a DbCommand to retrieve data
    // from the Categories table by executing a DbDataReader. 
    static void DbCommandSelect(DbConnection connection) {
        string queryString =
            "SELECT CategoryID, CategoryName FROM Categories";

        // Check for valid DbConnection.
        if (connection != null) {
            using (connection) {
                try {
                    // Create the command.
                    DbCommand command = connection.CreateCommand();
                    command.CommandText = queryString;
                    command.CommandType = CommandType.Text;

                    // Open the connection.
                    connection.Open();

                    // Retrieve the data.
                    DbDataReader reader = command.ExecuteReader();
                    while (reader.Read()) {
                        Console.WriteLine("{0}. {1}", reader[0], reader[1]);
                    }
                } catch (Exception ex) {
                    Console.WriteLine("Exception.Message: {0}", ex.Message);
                }
            }
        } else {
            Console.WriteLine("Failed: DbConnection is null.");
        }
    }

    // Takes a DbConnection, creates and executes a DbCommand. 
    // Assumes SQL INSERT syntax is supported by provider.
    static void ExecuteDbCommand(DbConnection connection) {
        // Check for valid DbConnection object.
        if (connection != null) {
            using (connection) {
                try {
                    // Open the connection.
                    connection.Open();

                    // Create and execute the DbCommand.
                    DbCommand command = connection.CreateCommand();
                    command.CommandText =
                        "INSERT INTO Categories (CategoryName) VALUES ('Low Carb')";
                    int rows = command.ExecuteNonQuery();

                    // Display number of rows inserted.
                    Console.WriteLine("Inserted {0} rows.", rows);
                }
                // Handle data errors.
                catch (DbException exDb) {
                    Console.WriteLine("DbException.GetType: {0}", exDb.GetType());
                    Console.WriteLine("DbException.Source: {0}", exDb.Source);
                    Console.WriteLine("DbException.ErrorCode: {0}", exDb.ErrorCode);
                    Console.WriteLine("DbException.Message: {0}", exDb.Message);
                }
                // Handle all other exceptions.
                catch (Exception ex) {
                    Console.WriteLine("Exception.Message: {0}", ex.Message);
                }
            }
        } else {
            Console.WriteLine("Failed: DbConnection is null.");
        }
    }
}