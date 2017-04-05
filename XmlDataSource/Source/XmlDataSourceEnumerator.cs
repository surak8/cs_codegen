using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using NSMisc;

namespace NSXmlDatasource {
    public class XmlDataSourceEnumerator : DbDataSourceEnumerator {
        public override DataTable GetDataSources() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }
    }

}
