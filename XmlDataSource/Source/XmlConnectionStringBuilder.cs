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

        public override Boolean ContainsKey(String keyword) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.ContainsKey(keyword);
        }

        public override Int32 Count {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return base.Count;
            }
        }

        public override Boolean Equals(Object obj) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.Equals(obj);
        }

        public override Boolean EquivalentTo(DbConnectionStringBuilder connectionStringBuilder) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.EquivalentTo(connectionStringBuilder);
        }

        protected override void GetProperties(Hashtable propertyDescriptors) {
            Logger.log(MethodBase.GetCurrentMethod());
            base.GetProperties(propertyDescriptors);
        }

        public override ICollection Keys {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return base.Keys;
            }
        }

        public override Boolean Remove(String keyword) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.Remove(keyword);
        }

        public override Boolean ShouldSerialize(String keyword) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.ShouldSerialize(keyword);
        }

        public override Object this[String keyword] {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return base[keyword];
            }

            set {
                Logger.log(MethodBase.GetCurrentMethod());
                base[keyword] = value;
            }
        }


        public override Boolean TryGetValue(String keyword, out Object value) {
            Logger.log(MethodBase.GetCurrentMethod());
            return base.TryGetValue(keyword, out value);
        }

        public override ICollection Values {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return base.Values;
            }
        }
        #endregion
        public override Int32 GetHashCode() {
            return base.GetHashCode();
        }
    }
}