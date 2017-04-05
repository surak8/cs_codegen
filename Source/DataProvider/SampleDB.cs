// https://msdn.microsoft.com/en-us/library/aa720697(v=vs.71).aspx

using System;
using System.Data.Common;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using NSCs_codegen;

namespace DotNetDataProviderTemplate {
    /*
     * This class provides database-like operations to simulate a real
     * data source. The class generates sample data and uses a
     * fixed set of commands.
     */
    public class SampleDb {
        public const string SELECT_COMMAND = "select ";
        public const string UPDATE_COMMAND = "update ";

        public class SampleDbResultSet {
            public struct MetaData {
                public string name;
                public Type type;
                public int maxSize;
            }

            public int recordsAffected;
            public MetaData[] metaData;
            public object[,] data;
        }

        SampleDbResultSet m_resultset = null;

        public void Execute(string sCmd, out SampleDbResultSet resultset) {
            /*
             * The sample code simulates SELECT and UPDATE operations.
             */
            if (0 == string.Compare(sCmd, 0, SELECT_COMMAND, 0, SELECT_COMMAND.Length, true)) {
                _executeSelect(out resultset);
            } else if (0 == string.Compare(sCmd, 0, UPDATE_COMMAND, 0, UPDATE_COMMAND.Length, true)) {
                _executeUpdate(out resultset);
            } else
                throw new NotSupportedException("Command string was not recognized");
        }

        void _executeSelect(out SampleDbResultSet resultset) {
            // If no sample data exists, create it.
            if (m_resultset == null)
                _resultsetCreate();

            // Return the sample results.
            resultset = m_resultset;
        }

        void _executeUpdate(out SampleDbResultSet resultset) {
            // If no sample data exists, create it.
            if (m_resultset == null)
                _resultsetCreate();

            // Change a row to simulate an update command.
            m_resultset.data[2, 2] = 4199;

            // Create a result set object that is empty except for the RecordsAffected field.
            resultset = new SampleDbResultSet();
            resultset.recordsAffected = 1;
        }

        void _resultsetCreate() {
            m_resultset = new SampleDbResultSet();

            // RecordsAffected is always a zero value for a SELECT.
            m_resultset.recordsAffected = 0;

            const int numCols = 3;
            m_resultset.metaData = new SampleDbResultSet.MetaData[numCols];
            _resultsetFillColumn(0, "id", typeof(int), 0);
            _resultsetFillColumn(1, "name", typeof(string), 64);
            _resultsetFillColumn(2, "orderid", typeof(int), 0);

            m_resultset.data = new object[5, numCols];
            _resultsetFillRow(0, 1, "Biggs", 2001);
            _resultsetFillRow(1, 2, "Brown", 2121);
            _resultsetFillRow(2, 3, "Jones", 2543);
            _resultsetFillRow(3, 4, "Smith", 2772);
            _resultsetFillRow(4, 5, "Tyler", 3521);
        }

        void _resultsetFillColumn(int nIdx, string name, Type type, int maxSize) {
            m_resultset.metaData[nIdx].name = name;
            m_resultset.metaData[nIdx].type = type;
            m_resultset.metaData[nIdx].maxSize = maxSize;
        }

        void _resultsetFillRow(int nIdx, int id, string name, int orderid) {
            m_resultset.data[nIdx, 0] = id;
            m_resultset.data[nIdx, 1] = name;
            m_resultset.data[nIdx, 2] = orderid;
        }
    }
}


/*
 * IDbConnection
 * DbProviderFactory
 * 
 * */
 