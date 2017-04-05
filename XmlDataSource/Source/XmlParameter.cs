using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using NSMisc;

namespace NSXmlDatasource {
    public class XmlParameter : DbParameter {

        public XmlParameter() {
            Logger.log(MethodBase.GetCurrentMethod());

        }

        public override DbType DbType {
            get {
                Logger.log(MethodBase.GetCurrentMethod());

                throw new NotImplementedException();
            }

            set {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override ParameterDirection Direction {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }

            set {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override bool IsNullable {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }

            set {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override string ParameterName {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }

            set {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override int Size {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }

            set {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override string SourceColumn {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }

            set {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override bool SourceColumnNullMapping {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }

            set {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override DataRowVersion SourceVersion {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }

            set {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override object Value {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }

            set {
                Logger.log(MethodBase.GetCurrentMethod());
                throw new NotImplementedException();
            }
        }

        public override void ResetDbType() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }
    }
}
