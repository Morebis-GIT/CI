using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenSalesAreaRepository : ISalesAreaRepository
    {
        private readonly IDocumentSession _session;

        private int _customId = -1;

        private int GetNextCustomId()
        {
            if (_customId == -1)
            {
                _customId = _session.Query<SalesArea>().Take(int.MaxValue).Select(s => s.CustomId).AsEnumerable()
                    .DefaultIfEmpty(0).Max();
            }

            return ++_customId;
        }

        public RavenSalesAreaRepository(IDocumentSession session)
        {
            _session = session;
        }

        public SalesArea Find(Guid id) =>
            _session.Load<SalesArea>(id);

        public IEnumerable<SalesArea> GetAll() =>
            _session.Query<SalesArea>().Take(int.MaxValue).ToList();

        public void Add(SalesArea salesArea)
        {
            salesArea.CustomId = GetNextCustomId();
            _session.Store(salesArea);
        }

        public void Update(SalesArea salesArea) =>
            _session.Store(salesArea);

        public void Update(List<SalesArea> salesAreas)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions() { OverwriteExisting = true };
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    salesAreas.ToList().ForEach(salesArea =>

                        bulkInsert.Store(salesArea));
                }
            }
        }

        public void Remove(Guid id) =>
            _session.Delete<SalesArea>(id);

        /// <summary>
        /// Get the sales area by names
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public List<SalesArea> FindByNames(List<string> names) =>
            _session.GetAll<SalesArea>().Where(s => s.Name.In(names)).ToList();

        /// <summary>
        /// Get the sales areas by customId(s) (ints)
        /// </summary>
        /// <param name="Ids">Custom Ids</param>
        /// <returns>List<SalesArea></returns>
        public List<SalesArea> FindByIds(List<int> Ids) =>
            _session.Query<SalesArea>().Where(s => s.CustomId.In(Ids)).Take(int.MaxValue).ToList();

        public SalesArea FindByName(string name) =>
            _session.Query<SalesArea>().FirstOrDefault(s => s.Name == name);

        public List<string> GetListOfNames(List<SalesArea> salesAreas) =>
            salesAreas.Select(item => item.Name).ToList();

        public List<string> GetListOfNames() =>
            _session.Query<SalesArea>().Take(int.MaxValue).Select(a => a.Name).Distinct().ToList();

        public int CountAll
        {
            get
            {
                lock (_session)
                {
                    return _session.CountAll<SalesArea>();
                }
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public SalesArea FindByShortName(string shortName) =>
            _session.GetAll<SalesArea>(s => s.ShortName == shortName).FirstOrDefault();

        public IEnumerable<SalesArea> FindByShortNames(IEnumerable<string> shortNames) =>
            _session.GetAll<SalesArea>(s => s.Name.In(shortNames));

        public void DeleteByShortName(string shortName)
        {
            lock (_session)
            {
                var salesAreas = _session.GetAll<SalesArea>(s => s.ShortName == shortName);

                foreach (var salesArea in salesAreas)
                {
                    _session.Delete(salesArea);
                }
            }
        }

        public void DeleteRange(IEnumerable<Guid> ids)
        {
            lock (_session)
            {
                var salesAreas = _session.GetAll<SalesArea>(s => s.Id.In(ids));

                foreach (var salesArea in salesAreas)
                {
                    _session.Delete(salesArea);
                }
            }
        }
    }
}
