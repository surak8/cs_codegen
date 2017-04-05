using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using NSMisc;

namespace NSXmlDatasource {
    public class XmlCommand : DbCommand {
        DbConnection _connection;
        CommandType _cmdType;
        UpdateRowSource _updateRowSource;
        XmlParameters _parms;
        DbTransaction _tran;
        bool _designVis;
        string _text;
        int _timeout;

        public XmlCommand() {
            Logger.log(MethodBase.GetCurrentMethod());
            _designVis = true;
            _timeout = 6000;
            _parms = new XmlParameters();
        }

        public override string CommandText {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return _text;
            }
            set {
    //            Logger.log(MethodBase.GetCurrentMethod());
                _text = value;
            }
        }

        public override int CommandTimeout {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return _timeout;
            }
            set {
                Logger.log(MethodBase.GetCurrentMethod());
                _timeout = value;
            }
        }

        public override CommandType CommandType {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return _cmdType;
            }
            set {
                Logger.log(MethodBase.GetCurrentMethod());
                _cmdType = value;
            }
        }

        public override bool DesignTimeVisible {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return _designVis;
            }
            set {
                Logger.log(MethodBase.GetCurrentMethod());
                _designVis = value;
            }
        }

        public override UpdateRowSource UpdatedRowSource {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return _updateRowSource;
            }
            set {
                Logger.log(MethodBase.GetCurrentMethod());
                _updateRowSource = value;
            }
        }

        protected override DbConnection DbConnection {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return _connection;
            }
            set {
  //              Logger.log(MethodBase.GetCurrentMethod());
                _connection = value;
            }
        }

        protected override DbParameterCollection DbParameterCollection {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return _parms;
            }
        }

        protected override DbTransaction DbTransaction {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return _tran;
            }
            set {
                Logger.log(MethodBase.GetCurrentMethod());
                _tran = value;
            }
        }

        public override void Cancel() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override int ExecuteNonQuery() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override object ExecuteScalar() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override void Prepare() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        protected override DbParameter CreateDbParameter() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) {
            Logger.log(MethodBase.GetCurrentMethod());
            //Logger.log(MethodBase.GetCurrentMethod());
            //throw new NotImplementedException();
            return new XmlDataReader(behavior);
        }
    }

    
}