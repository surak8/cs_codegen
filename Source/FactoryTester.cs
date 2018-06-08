using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace NSCs_codegen {
    public class FactoryTester {
        DbProviderFactory _factory;

        public FactoryTester(DbProviderFactory f) {
            _factory = f;
        }

        public void doTests() {
            DbConnection conn = createConnection();

            if (_factory is SqlClientFactory) {
                DbConnectionStringBuilder dcsb;

                dcsb = new DbConnectionStringBuilder();
                dcsb.Add("User ID", "operator");
                dcsb.Add("Password", "operator");
                dcsb.Add("Application Name", Assembly.GetEntryAssembly().GetName().Name);
                dcsb.Add("Workstation ID", Environment.MachineName);
                dcsb.Add("Data Source", "colt-sql");
                dcsb.Add("Initial Catalog", "checkweigh_data_dev");
                dcsb.Add("Persist Security Info", true);
                dcsb.Add("Integrated Security", true);
                conn.ConnectionString = dcsb.ConnectionString;
            }
            if (_factory.CanCreateDataSourceEnumerator)
                findDataSources(_factory);
            DbCommandSelect(conn);
            ExecuteDbCommand(conn);
            CreateDataAdapter(_factory, conn.ConnectionString);
            CreateDataAdapter2(_factory, conn.ConnectionString);
        }

        static void findDataSources(DbProviderFactory f) {
            DbDataSourceEnumerator e = f.CreateDataSourceEnumerator();
            DataTable t = e.GetDataSources();
            Trace.WriteLine("here");

            for (int col = 0; col < t.Columns.Count; col++) {
                if (col > 0)
                    Trace.Write(",");
                Trace.Write(t.Columns[col].ColumnName);
            }
            Trace.WriteLine(string.Empty);
            for (int row = 0; row < t.Rows.Count; row++) {
                for (int col = 0; col < t.Columns.Count; col++) {
                    if (col > 0)
                        Trace.Write(",");
                    Trace.Write(t.Rows[row][col].ToString());
                }
                Trace.WriteLine(string.Empty);
            }
            Trace.WriteLine(string.Empty);
            // uses chooses a Data Row r
            //DataRow r = t.Rows[0];
            //string dataSource = (string) r["ServerName"];
            //if (r[InstanceName] != null)
            //    dataSource += ("\\" + r["InstanceName"]);
            //// this method is defined below
            //RewriteConnectionStringAndUpdateConfigFile(f, dataSource);
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
                //using (connection) {
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
                        Trace.WriteLine(reader[0] + ". " + reader[1]);
                    }
                } catch (Exception ex) {
                    Trace.WriteLine("Exception.Message: {0}", ex.Message);
                } finally {
                    connection.Close();
                }
                //}
            } else {
                Trace.WriteLine("Failed: DbConnection is null.");
            }
        }

        // Takes a DbConnection, creates and executes a DbCommand. 
        // Assumes SQL INSERT syntax is supported by provider.
        static void ExecuteDbCommand(DbConnection connection) {
            // Check for valid DbConnection object.
            if (connection != null) {
                //using (connection) {
                try {
                    // Open the connection.

                    connection.Open();

                    // Create and execute the DbCommand.
                    DbCommand command = connection.CreateCommand();
                    command.CommandText =
                        "INSERT INTO Categories (CategoryName) VALUES ('Low Carb')";
                    int rows = command.ExecuteNonQuery();

                    // Display number of rows inserted.
                    Trace.WriteLine("Inserted " + rows + " rows.");
                }
                // Handle data errors.
                catch (DbException exDb) {
                    Trace.WriteLine("DbException.GetType: " + exDb.GetType());
                    Trace.WriteLine("DbException.Source: " + exDb.Source);
                    Trace.WriteLine("DbException.ErrorCode: " + exDb.ErrorCode);
                    Trace.WriteLine("DbException.Message: " + exDb.Message);
                }
                // Handle all other exceptions.
                catch (Exception ex) {
                    Trace.WriteLine("Exception.Message: {0}", ex.Message);
                    //}
                } finally {
                    connection.Close();
                }
            } else {
                Trace.WriteLine("Failed: DbConnection is null.");
            }
        }

        // https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/modifying-data-with-a-dbdataadapter

        static void CreateDataAdapter(DbProviderFactory factory, string connectionString) {
            //DbProviderFactory factory;
            DbConnection connection;
            DbCommand command;
            DbDataAdapter adapter;
            DataTable table;
            try {
                // Create the DbProviderFactory and DbConnection.
                //factory =
                //    DbProviderFactories.GetFactory(providerName);

                connection = factory.CreateConnection();
                connection.ConnectionString = connectionString;

                using (connection) {
                    // Define the query.
                    string queryString =
                        "SELECT CategoryName FROM Categories";

                    // Create the DbCommand.
                    command = factory.CreateCommand();
                    command.CommandText = queryString;
                    command.Connection = connection;

                    // Create the DbDataAdapter.
                    adapter = factory.CreateDataAdapter();
                    adapter.SelectCommand = command;

                    // Fill the DataTable.
                    table = new DataTable();
                    adapter.Fill(table);

                    //  Display each row and column value.
                    foreach (DataRow row in table.Rows) {
                        foreach (DataColumn column in table.Columns) {
                            Trace.WriteLine(row[column]);
                        }
                    }
                }
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message);
            }
        }

        static void CreateDataAdapter2(DbProviderFactory factory, string connectionString) {
            try {
                // Create the DbProviderFactory and DbConnection.
                //DbProviderFactory factory =
                //    DbProviderFactories.GetFactory(providerName);

                DbConnection connection = factory.CreateConnection();
                connection.ConnectionString = connectionString;

                using (connection) {
                    // Define the query.
                    string queryString =
                        "SELECT CustomerID, CompanyName FROM Customers";

                    // Create the select command.
                    DbCommand command = factory.CreateCommand();
                    command.CommandText = queryString;
                    command.Connection = connection;

                    // Create the DbDataAdapter.
                    DbDataAdapter adapter = factory.CreateDataAdapter();
                    adapter.SelectCommand = command;

                    // Create the DbCommandBuilder.
                    DbCommandBuilder builder = factory.CreateCommandBuilder();
                    builder.DataAdapter = adapter;

                    // Get the insert, update and delete commands.
                    adapter.InsertCommand = builder.GetInsertCommand();
                    adapter.UpdateCommand = builder.GetUpdateCommand();
                    adapter.DeleteCommand = builder.GetDeleteCommand();

                    // Display the CommandText for each command.
                    Trace.WriteLine("InsertCommand: {0}",
                        adapter.InsertCommand.CommandText);
                    Trace.WriteLine("UpdateCommand: {0}",
                        adapter.UpdateCommand.CommandText);
                    Trace.WriteLine("DeleteCommand: {0}",
                        adapter.DeleteCommand.CommandText);

                    // Fill the DataTable.
                    DataTable table = new DataTable();
                    adapter.Fill(table);

                    // Insert a new row.
                    DataRow newRow = table.NewRow();
                    newRow["CustomerID"] = "XYZZZ";
                    newRow["CompanyName"] = "XYZ Company";
                    table.Rows.Add(newRow);

                    adapter.Update(table);

                    // Display rows after insert.
                    Trace.WriteLine(string.Empty);
                    Trace.WriteLine("----List All Rows-----");
                    foreach (DataRow row in table.Rows) {
                        Trace.WriteLine(row[0] + " " + row[1]);
                    }
                    Trace.WriteLine("----After Insert-----");

                    // Edit an existing row.
                    DataRow[] editRow = table.Select("CustomerID = 'XYZZZ'");
                    editRow[0]["CompanyName"] = "XYZ Corporation";

                    adapter.Update(table);

                    // Display rows after update.
                    Trace.WriteLine(string.Empty);
                    foreach (DataRow row in table.Rows) {
                        Trace.WriteLine(row[0] + " " + row[1]);
                    }
                    Trace.WriteLine("----After Update-----");

                    // Delete a row.
                    DataRow[] deleteRow = table.Select("CustomerID = 'XYZZZ'");
                    foreach (DataRow row in deleteRow) {
                        row.Delete();
                    }

                    adapter.Update(table);

                    // Display rows after delete.
                    Trace.WriteLine(string.Empty);
                    foreach (DataRow row in table.Rows) {
                        Trace.WriteLine(row[0] + " " + row[1]);
                    }
                    Trace.WriteLine("----After Delete-----");
                    Trace.WriteLine("Customer XYZZZ was deleted.");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}