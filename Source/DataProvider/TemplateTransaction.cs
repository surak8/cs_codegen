using System;
using System.Data;

namespace DotNetDataProviderTemplate {
    public class TemplateTransaction : IDbTransaction {
        /*
           * Should return the current transaction isolation
           * level. For the template, assume the default
           * which is ReadCommitted.
           */
        public IsolationLevel IsolationLevel { get { return IsolationLevel.ReadCommitted; } }
        /*
                     * Implement Commit here. Although the template does
                     * not provide an implementation, it should never be 
                     * a no-op because data corruption could result.
                     */
        public void Commit() { }

        /*
         * Implement Rollback here. Although the template does
         * not provide an implementation, it should never be
         * a no-op because data corruption could result.
         */
        public void Rollback() { }

        /*
         * Return the connection for the current transaction.
         */
        public IDbConnection Connection { get { return this.Connection; } }

        public void Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing) {
            if (disposing) {
                if (null != this.Connection) {
                    // implicitly rollback if transaction still valid
                    this.Rollback();
                }
            }
        }
    }
}