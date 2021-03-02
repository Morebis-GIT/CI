using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Queries;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileProgrammeRepository
        : FileRepositoryBase, IProgrammeRepository
    {
        public FileProgrammeRepository(string folder) : base(folder, "programme")
        {
        }

        public void Dispose()
        {
        }

        public void Add(IEnumerable<Programme> items)
        {
            InsertItems(_folder, _type, items.ToList(), items.Select(i => i.Id.ToString()).ToList());
        }

        public void Add(Programme item)
        {
            List<Programme> items = new List<Programme>() { item };
            InsertItems(_folder, _type, items, items.ConvertAll(i => i.Id.ToString()));
        }

        [Obsolete("Use Get()")]
        public Programme Find(Guid id) => Get(id);

        public void DeleteRange(IEnumerable<Guid> ids)
        {
            DeleteAllItems<Programme>(_folder, _type, b => ids.Contains(b.Id));
        }

        public Programme Get(Guid id)
        {
            return GetItemByID<Programme>(_folder, _type, id.ToString());
        }

        public IEnumerable<Programme> Search(DateTime datefrom, DateTime dateto, string salesarea)
        {
            var items = GetAllByType<Programme>(_folder, _type, currentItem => currentItem.SalesArea == salesarea && currentItem.StartDateTime >= datefrom && currentItem.StartDateTime <= dateto);
            return items.ToList();
        }

        public PagedQueryResult<ProgrammeNameModel> Search(ProgrammeSearchQueryModel searchQuery)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Programme> GetAll()
        {
            return GetAllByType<Programme>(_folder, _type);
        }

        public int CountAll => CountAll(_folder, _type);

        [Obsolete("Use Delete()")]
        public void Remove(Guid uid) => Delete(uid);

        public void Delete(Guid uid)
        {
            DeleteItem(_folder, _type, uid.ToString());
        }

        public void Truncate()
        {
            DeleteAllItems(_folder, _type);
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.CompletedTask;
        }

        public IEnumerable<Programme> FindByExternal(string externalRef)
        {
            return GetAllByType<Programme>(_folder, _type, p => p.ExternalReference == externalRef);
        }

        public IEnumerable<Programme> FindByExternal(List<string> externalRefs)
        {
            return GetAllByType<Programme>(_folder, _type, p => externalRefs.Contains(p.ExternalReference));
        }

        public int Count(Expression<Func<Programme, bool>> query)
        {
            return GetAllByType(_folder, _type, query).Count;
        }

        public void SaveChanges()
        {
        }

        public bool Exists(Expression<Func<Programme, bool>> condition) => throw new NotImplementedException();
    }
}
