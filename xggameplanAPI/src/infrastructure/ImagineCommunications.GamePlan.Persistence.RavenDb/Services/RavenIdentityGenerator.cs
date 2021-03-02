using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Services
{
    /// <summary>
    /// Generates IDs from Raven
    /// </summary>
    public class RavenIdentityGenerator : IRavenIdentityGenerator, IDisposable
    {
        private readonly IDocumentSession _session;

        public RavenIdentityGenerator(IDocumentStore documentStore)
        {
            _session = (documentStore ?? throw new ArgumentNullException(nameof(documentStore))).OpenSession();
        }

        /// <summary>
        /// Generates N new identities
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<T> GetIdentities<T>(int number) where T : class, IIntIdentifier, new()
        {
            List<T> items = new List<T>();
            try
            {
                lock (_session)
                {
                    for (int index = 0; index < number; index++)
                    {
                        T item = (T)Activator.CreateInstance(typeof(T));
                        _session.Store(item);
                        items.Add(item);
                    }
                    _session.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                lock (_session)
                {
                    items.ForEach(item => _session.Delete(item));
                    _session.SaveChanges();
                }
            }
            return items;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _session?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
