using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using NSMisc;

namespace NSXmlDatasource {
    public class XmlCommandBuilder : DbCommandBuilder {
        protected override void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        protected override string GetParameterName(string parameterName) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        protected override string GetParameterName(int parameterOrdinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        protected override string GetParameterPlaceholder(int parameterOrdinal) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        protected override void SetRowUpdatingHandler(DbDataAdapter adapter) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }
    }


//#logger
}