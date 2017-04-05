using System;
using System.Data;

namespace DotNetDataProviderTemplate {
    public class TemplateConnection : IDbConnection {
        ConnectionState m_state;
        string m_sConnString;

        // Use the "SampleDb" class to simulate a database connection.
        SampleDb m_sampleDb;

        // Always have a default constructor.
        public TemplateConnection() {
            // Initialize the connection object into the closed state.
            m_state = ConnectionState.Closed;

            /*
             * Obtain a connection to the database. In this case,
             * use the SampleDb class to simulate a connection to 
             * a real database.
             */
            m_sampleDb = new SampleDb();
        }

        // Have a constructor that takes a connection string.
        public TemplateConnection(string sConnString) {
            // Initialize the connection object into a closed state.
            m_state = ConnectionState.Closed;
        }

        /****
         * IMPLEMENT THE REQUIRED PROPERTIES.
         ****/
        // Always return exactly what the user set.
        // Security-sensitive information may be removed.
        public string ConnectionString { get { return m_sConnString; } set { m_sConnString = value; } }

        // Returns the connection time-out value set in the connection
        // string. Zero indicates an indefinite time-out period.
        public int ConnectionTimeout { get { return 0; } }

        // Returns an initial database as set in the connection string.
        // An empty string indicates not set - do not return a null reference.
        public string Database { get { return ""; } }

        public ConnectionState State { get { return m_state; } }

        /****
         * IMPLEMENT THE REQUIRED METHODS.
         ****/

        public IDbTransaction BeginTransaction() { throw new NotSupportedException(); }

        public IDbTransaction BeginTransaction(IsolationLevel level) { throw new NotSupportedException(); }

        /*
         * Change the database setting on the back-end. Note that it is a method
         * and not a property because the operation requires an expensive
         * round trip.
         */
        public void ChangeDatabase(string dbName) { }

        /*
             * Open the database connection and set the ConnectionState
             * property. If the underlying connection to the server is 
             * expensive to obtain, the implementation should provide
             * implicit pooling of that connection.
             * 
             * If the provider also supports automatic enlistment in 
             * distributed transactions, it should enlist during Open().
             */
        public void Open() { m_state = ConnectionState.Open; }

        /*
         * Close the database connection and set the ConnectionState
         * property. If the underlying connection to the server is
         * being pooled, Close() will release it back to the pool.
         */
        public void Close() { m_state = ConnectionState.Closed; }

        // Return a new instance of a command object.
        public IDbCommand CreateCommand() { return new TemplateCommand(); }
        /*
         * Implementation specific properties / methods.
         */
        internal SampleDb SampleDb { get { return m_sampleDb; } }

        void IDisposable.Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing) {
            /*
             * Dispose of the object and perform any cleanup.
             */

            if (m_state == ConnectionState.Open)
                this.Close();
        }
    }
}