using System;
using System.Data;

namespace DotNetDataProviderTemplate {
    public class TemplateCommand : IDbCommand {
        TemplateConnection m_connection;
        TemplateTransaction m_txn;
        string m_sCmdText;
        UpdateRowSource m_updatedRowSource = UpdateRowSource.None;
        TemplateParameterCollection m_parameters = new TemplateParameterCollection();

        // Implement the default constructor here.
        public TemplateCommand() : this(null, null, null) { }

        // Implement other constructors here.
        public TemplateCommand(string cmdText) : this(cmdText, null, null) { }

        public TemplateCommand(string cmdText, TemplateConnection connection) : this(cmdText, connection, null) { }

        public TemplateCommand(string cmdText, TemplateConnection connection, TemplateTransaction txn) {
            m_sCmdText = cmdText;
            m_connection = connection;
            m_txn = txn;
        }

        /****
         * IMPLEMENT THE REQUIRED PROPERTIES.
         ****/
        public string CommandText { get { return m_sCmdText; } set { m_sCmdText = value; } }

        /*
         * The sample does not support a command time-out. As a result,
         * for the get, zero is returned because zero indicates an indefinite
         * time-out period. For the set, throw an exception.
         */
        public int CommandTimeout { get { return 0; } set { if (value != 0) throw new NotSupportedException(); } }

        /*
         * The sample only supports CommandType.Text.
         */
        public CommandType CommandType { get { return CommandType.Text; } set { if (value != CommandType.Text) throw new NotSupportedException(); } }

        public IDbConnection Connection {
            /*
             * The user should be able to set or change the connection at 
             * any time.
             */
            get { return m_connection; }
            set {
                /*
                 * The connection is associated with the transaction
                 * so set the transaction object to return a null reference if the connection 
                 * is reset.
                 */
                if (m_connection != value)
                    this.Transaction = null;

                m_connection = (TemplateConnection) value;
            }
        }

        public TemplateParameterCollection Parameters { get { return m_parameters; } }

        IDataParameterCollection IDbCommand.Parameters { get { return m_parameters; } }
        /*
         * Set the transaction. Consider additional steps to ensure that the transaction
         * is compatible with the connection, because the two are usually linked.
         */
        public IDbTransaction Transaction { get { return m_txn; } set { m_txn = (TemplateTransaction) value; } }

        public UpdateRowSource UpdatedRowSource { get { return m_updatedRowSource; } set { m_updatedRowSource = value; } }

        /****
         * IMPLEMENT THE REQUIRED METHODS.
         ****/
        public void Cancel() {
            // The sample does not support canceling a command
            // once it has been initiated.
            throw new NotSupportedException();
        }

        public IDbDataParameter CreateParameter() { return (IDbDataParameter) (new TemplateParameter()); }

        /*
         * ExecuteNonQuery is intended for commands that do
         * not return results, instead returning only the number
         * of records affected.
         */
        public int ExecuteNonQuery() {

            // There must be a valid and open connection.
            if (m_connection == null || m_connection.State != ConnectionState.Open)
                throw new InvalidOperationException("Connection must valid and open");

            // Execute the command.
            SampleDb.SampleDbResultSet resultset;
            m_connection.SampleDb.Execute(m_sCmdText, out resultset);

            // Return the number of records affected.
            return resultset.recordsAffected;
        }

        public IDataReader ExecuteReader() {
            /*
             * ExecuteReader should retrieve results from the data source
             * and return a DataReader that allows the user to process 
             * the results.
             */
            // There must be a valid and open connection.
            if (m_connection == null || m_connection.State != ConnectionState.Open)
                throw new InvalidOperationException("Connection must valid and open");

            // Execute the command.
            SampleDb.SampleDbResultSet resultset;
            m_connection.SampleDb.Execute(m_sCmdText, out resultset);

            return new TemplateDataReader(resultset);
        }

        public IDataReader ExecuteReader(CommandBehavior behavior) {
            /*
             * ExecuteReader should retrieve results from the data source
             * and return a DataReader that allows the user to process 
             * the results.
             */

            // There must be a valid and open connection.
            if (m_connection == null || m_connection.State != ConnectionState.Open)
                throw new InvalidOperationException("Connection must valid and open");

            // Execute the command.
            SampleDb.SampleDbResultSet resultset;
            m_connection.SampleDb.Execute(m_sCmdText, out resultset);

            /*
             * The only CommandBehavior option supported by this
             * sample is the automatic closing of the connection
             * when the user is done with the reader.
             */
            if (behavior == CommandBehavior.CloseConnection)
                return new TemplateDataReader(resultset, m_connection);
            else
                return new TemplateDataReader(resultset);
        }

        public object ExecuteScalar() {
            /*
             * ExecuteScalar assumes that the command will return a single
             * row with a single column, or if more rows/columns are returned
             * it will return the first column of the first row.
             */

            // There must be a valid and open connection.
            if (m_connection == null || m_connection.State != ConnectionState.Open)
                throw new InvalidOperationException("Connection must valid and open");

            // Execute the command.
            SampleDb.SampleDbResultSet resultset;
            m_connection.SampleDb.Execute(m_sCmdText, out resultset);

            // Return the first column of the first row.
            // Return a null reference if there is no data.
            if (resultset.data.Length == 0)
                return null;

            return resultset.data[0, 0];
        }

        // The sample Prepare is a no-op.
        public void Prepare() { }

        void IDisposable.Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        /*
           * Dispose of the object and perform any cleanup.
           */
        void Dispose(bool disposing) { }
    }
}