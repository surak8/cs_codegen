
using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using NSMisc;


// http://msdn.microsoft.com/en-us/library/ms379620(VS.80).aspx
// http://www.c-sharpcorner.com/UploadFile/amit_agrl/ProviderFactory08052005030042AM/ProviderFactory.aspx


namespace NSXmlDatasource {
    public partial class XmlProviderFactory : DbProviderFactory {
        #region fields
        public static XmlProviderFactory Instance = new XmlProviderFactory();
        #endregion

        bool _canCreateDSEnum = false;
        #region ctor
        public XmlProviderFactory() {
            //Logger.log(MethodBase.GetCurrentMethod());
        }
        #endregion

        #region DbProviderFactory implementation
        public override bool CanCreateDataSourceEnumerator {
            get {
                //Logger.log(MethodBase.GetCurrentMethod());
                return _canCreateDSEnum;
            }
        }

        public override DbCommand CreateCommand() {
            Logger.log(MethodBase.GetCurrentMethod());
            return new XmlCommand();
        }

        public override DbCommandBuilder CreateCommandBuilder() {
            Logger.log(MethodBase.GetCurrentMethod());
            return new XmlCommandBuilder();
        }

        public override DbConnection CreateConnection() {
            //Logger.log(MethodBase.GetCurrentMethod());
            return new XmlConnection();
        }

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() {
            Logger.log(MethodBase.GetCurrentMethod());
            return new XmlConnectionStringBuilder();
        }

        public override DbDataAdapter CreateDataAdapter() {
            Logger.log(MethodBase.GetCurrentMethod());
            return new XmlDataAdapter();
        }

        public override DbDataSourceEnumerator CreateDataSourceEnumerator() {
            Logger.log(MethodBase.GetCurrentMethod());
            return new XmlDataSourceEnumerator();
        }

        public override DbParameter CreateParameter() {
            Logger.log(MethodBase.GetCurrentMethod());
            return new XmlParameter();
        }

        public override CodeAccessPermission CreatePermission(PermissionState state) {
            Logger.log(MethodBase.GetCurrentMethod());
            return new XmlPermission(state);
        }
        #endregion
    }
}