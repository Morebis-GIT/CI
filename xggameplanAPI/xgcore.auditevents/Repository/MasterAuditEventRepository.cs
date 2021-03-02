using System;
using System.Collections.Generic;
using System.Threading;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Master repository for logging of audit events.
    /// </summary>
    public class MasterAuditEventRepository
        : IAuditEventRepository, IDisposable
    {
        private readonly List<IAuditEventRepository> _auditEventRepositories;
        private readonly Mutex _mutex = new Mutex();

        public MasterAuditEventRepository(List<IAuditEventRepository> auditEventRepositories)
        {
            _auditEventRepositories = auditEventRepositories;
            _ = _auditEventRepositories.RemoveAll(repository => repository == null);
        }

        /// <summary>
        /// Inserts audit event in to repository
        /// </summary>
        /// <param name="auditEvent"></param>
        public void Insert(AuditEvent auditEvent)
        {
            bool waited = false;
            try
            {
                waited = _mutex.WaitOne();
                foreach (IAuditEventRepository auditEventRepository in _auditEventRepositories)
                {
                    try
                    {
                        auditEventRepository.Insert(auditEvent);
                    }
                    catch { };
                }
            }
            finally
            {
                if (waited)
                {
                    _mutex.ReleaseMutex();
                }
            }
        }

        /// <summary>
        /// Returns all audit events matching criteria
        /// </summary>
        /// <param name="auditEventFilter"></param>
        /// <returns></returns>
        public List<AuditEvent> Get(AuditEventFilter auditEventFilter) =>
            _auditEventRepositories[0].Get(auditEventFilter);

        public void Delete(AuditEventFilter auditEventFilter)
        {
            foreach (IAuditEventRepository auditEventRepository in _auditEventRepositories)
            {
                auditEventRepository.Delete(auditEventFilter);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _mutex?.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
