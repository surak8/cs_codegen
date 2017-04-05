using System;
using System.Data;
using System.Globalization;

namespace DotNetDataProviderTemplate {
    public class TemplateDataReader : IDataReader {
        // The DataReader should always be open when returned to the user.
         bool m_fOpen = true;

        // Keep track of the results and position
        // within the resultset (starts prior to first record).
         SampleDb.SampleDbResultSet m_resultset;
         static int m_STARTPOS = -1;
         int m_nPos = m_STARTPOS;

        /* 
         * Keep track of the connection in order to implement the
         * CommandBehavior.CloseConnection flag. A null reference means
         * normal behavior (do not automatically close).
         */
         TemplateConnection m_connection = null;

        /*
         * Because the user should not be able to directly create a 
         * DataReader object, the constructors are
         * marked as internal.
         */
        internal TemplateDataReader(SampleDb.SampleDbResultSet resultset) {
            m_resultset = resultset;
        }

        internal TemplateDataReader(SampleDb.SampleDbResultSet resultset, TemplateConnection connection) {
            m_resultset = resultset;
            m_connection = connection;
        }

        /****
         * METHODS / PROPERTIES FROM IDataReader.
         ****/
        public int Depth {
            /*
             * Always return a value of zero if nesting is not supported.
             */
            get { return 0; }
        }

        public bool IsClosed {
            /*
             * Keep track of the reader state - some methods should be
             * disallowed if the reader is closed.
             */
            get { return !m_fOpen; }
        }

        public int RecordsAffected {
            /*
             * RecordsAffected is only applicable to batch statements
             * that include inserts/updates/deletes. The sample always
             * returns -1.
             */
            get { return -1; }
        }

        public void Close() {
            /*
             * Close the reader. The sample only changes the state,
             * but an actual implementation would also clean up any 
             * resources used by the operation. For example,
             * cleaning up any resources waiting for data to be
             * returned by the server.
             */
            m_fOpen = false;
        }

        public bool NextResult() {
            // The sample only returns a single resultset. However,
            // DbDataAdapter expects NextResult to return a value.
            return false;
        }

        public bool Read() {
            // Return true if it is possible to advance and if you are still positioned
            // on a valid row. Because the data array in the resultset
            // is two-dimensional, you must divide by the number of columns.
            if (++m_nPos >= m_resultset.data.Length / m_resultset.metaData.Length)
                return false;
            else
                return true;
        }

        public DataTable GetSchemaTable() {
            //$
            throw new NotSupportedException();
        }

        /****
         * METHODS / PROPERTIES FROM IDataRecord.
         ****/
        public int FieldCount {
            // Return the count of the number of columns, which in
            // this case is the size of the column metadata
            // array.
            get { return m_resultset.metaData.Length; }
        }

        public string GetName(int i) {
            return m_resultset.metaData[i].name;
        }

        public string GetDataTypeName(int i) {
            /*
             * Usually this would return the name of the type
             * as used on the back end, for example 'smallint' or 'varchar'.
             * The sample returns the simple name of the .NET Framework type.
             */
            return m_resultset.metaData[i].type.Name;
        }

        public Type GetFieldType(int i) {
            // Return the actual Type class for the data type.
            return m_resultset.metaData[i].type;
        }

        public object GetValue(int i) {
            return m_resultset.data[m_nPos, i];
        }

        public int GetValues(object[] values) {
            int i = 0, j = 0;
            for (; i < values.Length && j < m_resultset.metaData.Length ; i++, j++) {
                values[i] = m_resultset.data[m_nPos, j];
            }

            return i;
        }

        public int GetOrdinal(string name) {
            // Look for the ordinal of the column with the same name and return it.
            for (int i = 0 ; i < m_resultset.metaData.Length ; i++) {
                if (0 == _cultureAwareCompare(name, m_resultset.metaData[i].name)) {
                    return i;
                }
            }

            // Throw an exception if the ordinal cannot be found.
            throw new IndexOutOfRangeException("Could not find specified column in results");
        }

        public object this[int i] {
            get { return m_resultset.data[m_nPos, i]; }
        }

        public object this[string name] {
            // Look up the ordinal and return 
            // the value at that position.
            get { return this[GetOrdinal(name)]; }
        }

        public bool GetBoolean(int i) {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (bool) m_resultset.data[m_nPos, i];
        }

        public byte GetByte(int i) {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (byte) m_resultset.data[m_nPos, i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) {
            // The sample does not support this method.
            throw new NotSupportedException("GetBytes not supported.");
        }

        public char GetChar(int i) {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (char) m_resultset.data[m_nPos, i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) {
            // The sample does not support this method.
            throw new NotSupportedException("GetChars not supported.");
        }

        public Guid GetGuid(int i) {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (Guid) m_resultset.data[m_nPos, i];
        }

        public short GetInt16(int i) {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (short) m_resultset.data[m_nPos, i];
        }

        public int GetInt32(int i) {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (int) m_resultset.data[m_nPos, i];
        }

        public long GetInt64(int i) {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (long) m_resultset.data[m_nPos, i];
        }

        public float GetFloat(int i) {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (float) m_resultset.data[m_nPos, i];
        }

        public double GetDouble(int i) {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (double) m_resultset.data[m_nPos, i];
        }

        public string GetString(int i) {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (string) m_resultset.data[m_nPos, i];
        }

        public Decimal GetDecimal(int i) {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
             */
            return (Decimal) m_resultset.data[m_nPos, i];
        }

        public DateTime GetDateTime(int i) {
            /*
             * Force the cast to return the type. InvalidCastException
             * should be thrown if the data is not already of the correct type.
            */
            return (DateTime) m_resultset.data[m_nPos, i];
        }

        public IDataReader GetData(int i) {
            /*
             * The sample code does not support this method. Normally,
             * this would be used to expose nested tables and
             * other hierarchical data.
             */
            throw new NotSupportedException("GetData not supported.");
        }

        public bool IsDBNull(int i) {
            return m_resultset.data[m_nPos, i] == DBNull.Value;
        }

        /*
         * Implementation specific methods.
         */
         int _cultureAwareCompare(string strA, string strB) {
            return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth | CompareOptions.IgnoreCase);
        }

        void IDisposable.Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

         void Dispose(bool disposing) {
            if (disposing) {
                try {
                    this.Close();
                } catch (Exception e) {
                    throw new SystemException("An exception of type " + e.GetType() +
                                              " was encountered while closing the TemplateDataReader.");
                }
            }
        }

    }
}