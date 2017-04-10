using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Reflection;
using NSMisc;

namespace NSXmlDatasource {
    public class XmlDataReader : DbDataReader {
        CommandBehavior behavior;

        public XmlDataReader() {
            Logger.log(MethodBase.GetCurrentMethod());
        }

        public XmlDataReader(CommandBehavior behavior) : this() {
            Logger.log(MethodBase.GetCurrentMethod());
            this.behavior = behavior;
        }

        public override object this[string name] {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override object this[int ordinal] {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override int Depth {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override int FieldCount {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override bool HasRows {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override bool IsClosed {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override int RecordsAffected {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override void Close() {
            Logger.log(MethodBase.GetCurrentMethod());
        }

        public override bool GetBoolean(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override Byte GetByte(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, Byte[] buffer, int bufferOffset, int length) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override Decimal GetDecimal(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override double GetDouble(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override Type GetFieldType(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override float GetFloat(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override int GetInt32(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override long GetInt64(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override string GetName(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override int GetOrdinal(string name) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override DataTable GetSchemaTable() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override string GetString(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override object GetValue(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override int GetValues(object[] values) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int ordinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override bool NextResult() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override bool Read() {
            Logger.log(System.Reflection.MethodBase.GetCurrentMethod());
            return false;
            throw new NotImplementedException();
        }
    }
}