using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using NSMisc;

namespace NSXmlDatasource {
    public class XmlParameters : DbParameterCollection {
        readonly List<XmlParameter> _parms = new List<XmlParameter>();
        readonly object _syncRoot = new object();
        bool _fixedSize = false;
        bool _readonly = false;
        #region ctor
        public XmlParameters() {
            //Logger.log(MethodBase.GetCurrentMethod());
        }
        #endregion

        #region DBParameterCollection implementation
        public override int Count { get { return _parms.Count; } }
        public override bool IsFixedSize { get { return _fixedSize; } }
        public override bool IsReadOnly { get { return _readonly; } }

        public override bool IsSynchronized { get { return true; } }

        public override object SyncRoot {
            get {
                Logger.log(MethodBase.GetCurrentMethod());
                return _syncRoot;
            }
        }

        public override int Add(object value) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override void AddRange(Array values) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override void Clear() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override bool Contains(string value) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override bool Contains(object value) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override void CopyTo(Array array, int index) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator() {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override int IndexOf(string parameterName) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override int IndexOf(object value) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override void Insert(int index, object value) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override void Remove(object value) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override void RemoveAt(string parameterName) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        public override void RemoveAt(int index) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        protected override DbParameter GetParameter(string parameterName) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        protected override DbParameter GetParameter(int index) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        protected override void SetParameter(string parameterName, DbParameter value) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        protected override void SetParameter(int index, DbParameter value) {
            Logger.log(MethodBase.GetCurrentMethod());
            throw new NotImplementedException();
        }

        #endregion
        //   }
    }
}