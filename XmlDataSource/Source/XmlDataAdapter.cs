using System.Data;
using System.Data.Common;
using System.Reflection;
using NSMisc;

namespace NSXmlDatasource {
    public class XmlDataAdapter : DbDataAdapter {
        public XmlDataAdapter() {
            Logger.log(MethodBase.GetCurrentMethod());
        }
        #region DbDataAdapter implementation

        protected override System.Int32 AddToBatch(IDbCommand command) {
            return base.AddToBatch(command);
        }
        #endregion DbDataAdapter implementation

    }
}