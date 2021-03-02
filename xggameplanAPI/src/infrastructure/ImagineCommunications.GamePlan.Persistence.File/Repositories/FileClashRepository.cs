using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileClashRepository
        : FileRepositoryBase, IClashRepository
    {
        public FileClashRepository(string folder) : base(folder, "clash")
        {

        }

        public void Dispose()
        {

        }

        public void Add(IEnumerable<Clash> items)
        {
            InsertItems(_folder, _type, items.ToList(), items.Select(i => i.Uid.ToString()).ToList<string>());
        }

        public void Add(Clash item)
        {
            var items = new List<Clash>() { item };
            InsertItems(_folder, _type, items, items.Select(i => i.Uid.ToString()).ToList<string>());
        }

        public Clash Get(Guid id)
        {
            return GetItemByID<Clash>(_folder, _type, id.ToString());
        }

        public Clash Find(Guid id) => Get(id);

        public IEnumerable<Clash> FindByExternal(string externalref)
        {
            return GetAllByType<Clash>(_folder, _type, c => c.Externalref == externalref);
        }

        public IEnumerable<Clash> FindByExternal(List<string> externalRefs)
        {
            return GetAllByType<Clash>(_folder, _type, c => externalRefs.Contains(c.Externalref));
        }

        public IEnumerable<Clash> GetAll()
        {
            return GetAllByType<Clash>(_folder, _type);
        }

        public IEnumerable<ClashNameModel> GetDescriptionByExternalRefs(ICollection<string> externalRefs) => throw new NotImplementedException();

        public int Count() => CountAll<Clash>(_folder, _type);

        [Obsolete("Use Count()")]
        public int CountAll => Count();

        public void Delete(Guid uid)
        {
            DeleteItem<Clash>(_folder, _type, uid.ToString());
        }

        public void Remove(Guid uid) => Delete(uid);

        public void Remove(Guid uid, out bool isDeleted)
        {
            isDeleted = false;
            var item = Find(uid);
            if (item != null)
            {
                DeleteItem<Clash>(_folder, _type, uid.ToString());
                isDeleted = true;
            }
        }

        public void Truncate()
        {
            DeleteAllItems<Clash>(_folder, _type);
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.FromResult(true);
        }

        public PagedQueryResult<ClashNameModel> Search(ClashSearchQueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<Clash, bool>> query)
        {
            return GetAllByType(_folder, _type, query).Count;
        }

        public void SaveChanges() { }

        public bool Exists(Expression<Func<Clash, bool>> condition) => throw new NotImplementedException();
        public void DeleteRange(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<Clash> clashes)
        {
            throw new NotImplementedException();
        }
    }
}
