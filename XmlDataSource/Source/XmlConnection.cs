using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;
using NSMisc;

namespace NSXmlDatasource {
    public class XmlConnection : DbConnection {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ConnectionState _state;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string _db;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string _datasrc;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string _svrVersion;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string _connStr;

        #region ctor
        public XmlConnection() {
            Logger.log(MethodBase.GetCurrentMethod());
            _state = ConnectionState.Closed;
            _svrVersion = "V1.0";
        }


        #endregion

        #region DbConnection implementation
        #region properties
        public override string ConnectionString {
            get { Logger.log(MethodBase.GetCurrentMethod()); return _connStr; }
            set { _connStr = value; if (!string.IsNullOrEmpty(ConnectionString)) extractParts(); }
        }

        void extractParts() {
            string[] parts = ConnectionString.Split(';');
            string aKey, aValue;
            int pos;

            if (parts != null)
                foreach (string aPart in parts) {
                    pos = aPart.IndexOf('=');
                    aKey = aPart.Substring(0, pos);
                    aValue = aPart.Substring(pos + 1);
                    if (string.Compare(aKey, "database", true) == 0) {
                        this._db = aValue;
                    } else if (string.Compare(aKey, "server", true) == 0) {
                        this._datasrc = aValue;
                    } else {
                        Trace.WriteLine("unhandled key:" + aKey);
                    }
                }
        }

        public override string Database { get { Logger.log(MethodBase.GetCurrentMethod()); return _db; } }

        public override string DataSource { get { Logger.log(MethodBase.GetCurrentMethod()); return _datasrc; } }

        public override string ServerVersion { get { Logger.log(MethodBase.GetCurrentMethod()); return _svrVersion; } }

        public override ConnectionState State { get { return _state; } }
        #endregion properties

        public override void Open() {
            ConnectionState prev = State;

            _state = ConnectionState.Open;
            OnStateChange(new StateChangeEventArgs(prev, State));
        }

        public override void Close() {
            Logger.log(MethodBase.GetCurrentMethod());
            ConnectionState prev = State;

            _state = ConnectionState.Closed;
            OnStateChange(new StateChangeEventArgs(prev, State));
        }

        public override void ChangeDatabase(string databaseName) {
            _db = databaseName;
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        protected override DbCommand CreateDbCommand() {
            return new XmlCommand();
        }

        #endregion DbConnection implementation

        #region DbConnection stuff
        protected override bool CanRaiseEvents {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return base.CanRaiseEvents;
            }
        }

        public override int ConnectionTimeout {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return base.ConnectionTimeout;
            }
        }

        public override ObjRef CreateObjRef(Type requestedType) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.CreateObjRef(requestedType);
        }

        protected override DbProviderFactory DbProviderFactory {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return base.DbProviderFactory;
            }
        }

        public override DataTable GetSchema() {
            DataTable ret = new DataTable();
            Logger.log(MethodBase.GetCurrentMethod());

            return ret;
        }

        public override DataTable GetSchema(string collectionName) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.GetSchema(collectionName);
        }

        public override DataTable GetSchema(string collectionName, string[] restrictionValues) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.GetSchema(collectionName, restrictionValues);
        }

        protected override void OnStateChange(StateChangeEventArgs stateChange) {
            Logger.log(MethodBase.GetCurrentMethod());
            base.OnStateChange(stateChange);
            this.StateChange?.Invoke(this, stateChange);
        }

        public override Task OpenAsync(CancellationToken cancellationToken) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.OpenAsync(cancellationToken);
        }

        public override event StateChangeEventHandler StateChange;

        public override ISite Site {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return base.Site;
            }

            set {
                base.Site = value;
            }
        }
        #endregion
    }
}