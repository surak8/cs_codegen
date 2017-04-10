using System;
using System.Reflection;
using System.Data.Common;
using NSMisc;
using System.Collections;

namespace NSXmlDatasource {

    public class XmlConnectionStringBuilder : DbConnectionStringBuilder {
        public XmlConnectionStringBuilder() {
            Logger.log(MethodBase.GetCurrentMethod());
        }
        #region overrides
        public override void Clear() {
            Logger.log(MethodBase.GetCurrentMethod());
            base.Clear();
        }

        public override bool ContainsKey(string keyword) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.ContainsKey(keyword);
        }

        public override int Count {
            get {
                int ret = base.Count;

                Logger.log(MethodBase.GetCurrentMethod() , " returning "+ret);
                return ret;
            }
        }

        public override bool Equals(object obj) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.Equals(obj);
        }

        public override bool EquivalentTo(DbConnectionStringBuilder connectionStringBuilder) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.EquivalentTo(connectionStringBuilder);
        }

        protected override void GetProperties(Hashtable propertyDescriptors) {
            Logger.log(MethodBase.GetCurrentMethod());
            base.GetProperties(propertyDescriptors);
        }

        public override ICollection Keys {
            get {
                //Logger.log(MethodBase.GetCurrentMethod());
                return base.Keys;
            }
        }

        public override bool Remove(string keyword) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.Remove(keyword);
        }

        public override bool ShouldSerialize(string keyword) {
            bool ret = base.ShouldSerialize(keyword);

            Logger.log(MethodBase.GetCurrentMethod(),"keyword = "+keyword );
            return ret;
        }

        public override object this[string keyword] {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return base[keyword];
            }

            set {
       //         Logger.log(MethodBase.GetCurrentMethod(),keyword+" = "+value);
                base[keyword] = value;
            }
        }


        public override bool TryGetValue(string keyword, out object value) {
            bool ret;

      //      Logger.log(MethodBase.GetCurrentMethod());
            ret=base.TryGetValue(keyword, out value);
            Logger.log(MethodBase.GetCurrentMethod(),"returning "+ret+" for keyword '"+keyword+"'!");
            return ret;
        }

        public override ICollection Values {
            get {
                //Logger.log(MethodBase.GetCurrentMethod());
                return base.Values;
            }
        }
        #endregion
        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}