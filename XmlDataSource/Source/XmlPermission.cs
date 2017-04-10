using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using NSMisc;

namespace NSXmlDatasource {
 public class XmlPermission : CodeAccessPermission {
          PermissionState _state;

        public XmlPermission(PermissionState state) {
            Logger.log(MethodBase.GetCurrentMethod());
            this._state = state;
        }

        public override IPermission Copy() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override void FromXml(SecurityElement elem) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override IPermission Intersect(IPermission target) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override bool IsSubsetOf(IPermission target) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override SecurityElement ToXml() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }
    }
}
