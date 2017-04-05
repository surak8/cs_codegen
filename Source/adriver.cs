//
//#define OTHER_TEST
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NSMisc;
using System.Data.SqlClient;
using System.CodeDom.Compiler;

namespace NSCs_codegen {
    public partial class cs_codegenDriver {
        [STAThread()]
        public static void Main(string[] args) {
            int exitCode = 0;
            TextWriterTraceListener twtl = null;
            //<<<<<<< Updated upstream
            //            string connStr, logFile, dir, appName, nameSpace, server, database;
            //            const string TRACER_NAME = "blah";
            //            CodeDomProvider cdp = new CSharpCodeProvider();
            //            CodeGeneratorOptions opts = new CodeGeneratorOptions();
            //            string outDir;
            //            List<string> argsToProcess;
            //            bool showHelp;


            //            opts.BlankLinesBetweenMembers = false;
            //            opts.ElseOnClosing = true;

            //=======
            string logFile, dir, appName, connStr;
            const string TRACER_NAME = "blah";
            CodeGenArgs args2 = CodeGenArgs.parseArgs(args);

            args2.setProvider("System.Data.SqlClient");
            args2.setProvider("NSMyProvider");
            args2.nameSpace = "Colt.Database";

            //>>>>>>> Stashed changes
            appName = Assembly.GetEntryAssembly().GetName().Name;
#if TRACE
            logFile = Environment.ExpandEnvironmentVariables("%TEMP%" + "\\" + appName + "\\" + appName + ".log");
            if (!Directory.Exists(dir = Path.GetDirectoryName(logFile)))
                Directory.CreateDirectory(dir);

            twtl = new TextWriterTraceListener(logFile, TRACER_NAME);
            Trace.Listeners.Add(twtl);
#endif
            //<<<<<<< Updated upstream
            Trace.WriteLine(appName + " starts");

            List<string> argsToProcess;
            bool showHelp = false;
            string nameSpace, server, database, outDir;


            argsToProcess = parseArguments(args, out nameSpace, out server, out database, out showHelp, out outDir);
            if (string.IsNullOrEmpty(database)) {
                Console.Error.WriteLine("database not specified.  Cannot continue.");
                showHelp = true;
            }
            if (string.IsNullOrEmpty(outDir)) {
                Console.Error.WriteLine("output-directory not specified.  Cannot continue.");
                showHelp = true;
            }
            if (showHelp) {
                Console.Error.WriteLine("show help here");
                exitCode = 2;
            } else {
                SqlConnectionStringBuilder sb;

                sb = new SqlConnectionStringBuilder();
                sb.ApplicationName = appName;
                sb.DataSource = server;
                sb.InitialCatalog = database;
                //      sb.InitialCatalog = "QualityAndEngineering";
#if false
            sb.IntegratedSecurity = false;
            sb.UserID = "operator";
            sb.Password = "operator";
#else
                sb.IntegratedSecurity = true;
#endif
                connStr = sb.ConnectionString;

                Trace.WriteLine("ConnectionString is " + (connStr = sb.ConnectionString));

                Trace.WriteLine("Generate files in: " + outDir);
                //''        outDir = Path.Combine(Directory.GetCurrentDirectory(), "Generated_Files", sb.InitialCatalog);
                try {
                    if (!Directory.Exists(outDir))
                        Directory.CreateDirectory(outDir);
                    using (SqlConnection conn = new SqlConnection(connStr)) {
                        conn.InfoMessage += Conn_InfoMessage;
                        conn.Open();
                        //=======

                        Trace.WriteLine(appName + " starts");
                        extractDataFor(args2);
                        Trace.WriteLine(appName + " ends");
                    }
                } catch (Exception ex) {
                    Logger.log(MethodBase.GetCurrentMethod(), ex);
                } finally {

                }
#if TRACE
                Trace.Flush();
                if (twtl != null)
                    Trace.Listeners.Remove(TRACER_NAME);
#endif
                Environment.Exit(exitCode);
            }
            //                }
        }

        private static void Conn_InfoMessage(Object sender, SqlInfoMessageEventArgs e) {
            throw new NotImplementedException();
        }

        //    }

        static void extractDataFor(CodeGenArgs args2) {
            try {
                extractTablesAsClasses(args2);
            } catch (Exception ex) {
                Logger.log(MethodBase.GetCurrentMethod(), ex);
            }
        }

        static void blah() {
            DataTable dt = DbProviderFactories.GetFactoryClasses();

            for (int nrow = 0 ; nrow < dt.Rows.Count ; nrow++) {
                for (int ncol = 0 ; ncol < dt.Columns.Count ; ncol++)
                    Debug.Print(dt.Columns[ncol].Caption + " = " + dt.Rows[nrow][ncol].ToString());
                Debug.Print(string.Empty);
            }
        }

        // This example assumes a reference to System.Data.Common.
        static DataTable findProviderFactoryClasses() {
            // Retrieve the installed providers and factories.
            DataTable table = DbProviderFactories.GetFactoryClasses();

            // Display each row and column value.
            foreach (DataRow row in table.Rows) {
                foreach (DataColumn column in table.Columns) {
                    Console.WriteLine(row[column]);
                }
                Console.WriteLine();
            }
            return table;
        }

        static void extractTablesAsClasses(CodeGenArgs args2) {
            string connStr, outDir, appName = Assembly.GetEntryAssembly().GetName().Name, outdir;
            DbProviderFactory factory = args2.providerFactory;
            outdir = "QualityAndEngineering";
#if TRACE
#endif
            DbConnectionStringBuilder sb = args2.providerFactory.CreateConnectionStringBuilder();

            if (factory is System.Data.SqlClient.SqlClientFactory) {
                ((System.Data.SqlClient.SqlConnectionStringBuilder) sb).ApplicationName = appName;
                ((System.Data.SqlClient.SqlConnectionStringBuilder) sb).DataSource = "colt-sql";
                ((System.Data.SqlClient.SqlConnectionStringBuilder) sb).InitialCatalog = "checkweigh_data_dev";
                ((System.Data.SqlClient.SqlConnectionStringBuilder) sb).InitialCatalog = outdir;
                ((System.Data.SqlClient.SqlConnectionStringBuilder) sb).IntegratedSecurity = true;
            }

            connStr = sb.ConnectionString;
            Trace.WriteLine("ConnectionString is " + (connStr = sb.ConnectionString));
            outDir = Path.Combine(Directory.GetCurrentDirectory(), "Generated_Files", outdir);
            try {
                using (DbConnection conn = factory.CreateConnection()) {
                    conn.ConnectionString = connStr;
                    if (conn is System.Data.SqlClient.SqlConnection)
                        ((System.Data.SqlClient.SqlConnection) conn).InfoMessage += infoMessageHandler; ;
                    //   ((System.Data.SqlClient.SqlConnection) conn).InfoMessage += Conn_InfoMessage;
                    conn.Open();
                    //>>>>>>> Stashed changes
#if OTHER_TEST
#else

                    //<<<<<<< Updated upstream
                    //                        //                    generateCodeForSingleTable(conn, "colt_employee", outDir, string.Empty, cdp, opts);
                    //                        //                    generateCodeForSingleTable(conn, "query", outDir, string.Empty, cdp, opts);
                    //                        generateCodeFromTables(conn, outDir, nameSpace, cdp, opts);
                    //                        //                    generateCodeFromViews(conn, outDir, string.Empty, cdp, opts);
                    //                        //                    generateCodeFromTables(conn, "KanbanTemp_CreateKanbanFile", Directory.GetCurrentDirectory(), string.Empty, cdp, opts);
                    //=======
                    //                    generateCodeForSingleTable(conn, "colt_employee", outDir, string.Empty, cdp, opts);
                    //                    generateCodeForSingleTable(conn, "query", outDir, string.Empty, cdp, opts);
                    generateCodeFromTables(conn, args2);
                    //                    generateCodeFromViews(conn, outDir, string.Empty, cdp, opts);
                    //                    generateCodeFromTables(conn, "KanbanTemp_CreateKanbanFile", Directory.GetCurrentDirectory(), string.Empty, cdp, opts);
                    //>>>>>>> Stashed changes
#endif
                    conn.Close();
                }
            } catch (Exception ex) {
                Trace.WriteLine(ex.Message);
            } finally {

            }
        }
        //    }

        //<<<<<<< Updated upstream
        /// <summary>parse the command-line parameters.</summary>
        /// <param name="args"></param>
        /// <param name="nameSpace"></param>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="showHelp"></param>
        /// <param name="outDir"></param>
        /// <returns></returns>
        static List<string> parseArguments(string[] args, out string nameSpace, out string server, out string database, out bool showHelp, out string outDir) {
            List<string> argsToProcess = new List<string>();
            int nargs = args.Length, len;
            string anArg;

            nameSpace = "Colt.Database";
            server = "colt-sql";
            outDir = null;
            database = null;
            //            database = "checkweigh_data_dev";

            //        zipName = null;
            showHelp = false;
            argsToProcess = new List<string>();
            for (int i = 0 ; i < nargs ; i++) {
                anArg = args[i];
                if ((len = anArg.Length) >= 2) {
                    if (anArg[0] == '-' || anArg[0] == '/') {
                        switch (anArg[1]) {
                            case 'd':
                                if (len > 2)
                                    database = anArg.Substring(2).Trim();
                                else { database = args[i + 1]; i++; }
                                break;
                            case 'n':
                                if (len > 2)
                                    nameSpace = anArg.Substring(2).Trim();
                                else { nameSpace = args[i + 1]; i++; }
                                break;
                            case 'o':
                                if (len > 2)
                                    outDir = anArg.Substring(2).Trim();
                                else { outDir = args[i + 1]; i++; }
                                break;
                            case 's':
                                if (len > 2)
                                    server = anArg.Substring(2).Trim();
                                else { server = args[i + 1]; i++; }
                                break;
                            case 'h': showHelp = true; break;
                            case '?': showHelp = true; break;
                        }
                    } else {
                        argsToProcess.Add(anArg);
                    }
                }
            }
            return argsToProcess;
            //}
        }


        static void generateCodeFromViews(SqlConnection conn, CodeGenArgs args) {
            //      static void generateCodeFromViews(SqlConnection conn, string outDir, string nameSpace, CodeDomProvider cdp, CodeGeneratorOptions opts) {
            //            generateCodeSysObjectType(conn, outDir, nameSpace, cdp, opts, "V");
            generateCodeSysObjectType(conn, args, "V");
        }
        static void infoMessageHandler(object sender, System.Data.SqlClient.SqlInfoMessageEventArgs e) {
            Logger.log(MethodBase.GetCurrentMethod(), e.Message);
        }

        static void generateCodeFromViews(DbConnection conn, CodeGenArgs args) {
            generateCodeSysObjectType(conn, args, "V");
            //>>>>>>> Stashed changes
        }

        static void generateCodeFromTables(DbConnection conn, CodeGenArgs args) {
            generateCodeSysObjectType(conn, args, "U");
        }

        static void generateCodeSysObjectType(DbConnection conn, CodeGenArgs args, string objType) {
            List<string> names = new List<string>();
            DbDataReader reader;
            DbProviderFactory factory = args.providerFactory;

            try {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                using (DbCommand cmd = factory.CreateCommand()) {
                    cmd.Connection = conn;
                    cmd.CommandText = "select name from sysobjects where type='" + objType + "' and uid=user_id('DBO') order by name";
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                        names.Add(reader.GetString(0));
                    reader.Close();
                }
                foreach (string aTable in names)
                    generateCodeForSingleTable(conn, aTable, args);

                conn.Close();
            } catch (Exception ex) {
                Logger.log(MethodBase.GetCurrentMethod(), ex);
            }
        }

        static void generateCodeForSingleTable(DbConnection conn, string aTable, CodeGenArgs args) {
            DbDataReader reader;

            using (DbCommand cmd = args.providerFactory.CreateCommand()) {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM " + aTable + " WHERE 1=0";
                cmd.CommandType = CommandType.Text;
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                //<<<<<<< Updated upstream
                //                cmd.CommandText = "SELECT * FROM " + aTable + " WHERE 1=0";
                reader = cmd.ExecuteReader();
                //<<<<<<< Updated upstream
                //                generateStuff(makeClassName(aTable), outDir, nameSpace, cdp, opts, reader, aTable);
                //=======
                //                generateStuff(makeClassName(aTable), outDir, nameSpace, cdp, opts, reader,aTable);
                //=======
                generateStuff(makeClassName(aTable), args, reader = cmd.ExecuteReader(), aTable);
                //>>>>>>> Stashed changes
                //>>>>>>> Stashed changes
                reader.Close();
            }
        }
    }
}