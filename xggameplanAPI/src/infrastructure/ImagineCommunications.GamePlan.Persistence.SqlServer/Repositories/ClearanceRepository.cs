using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Shared.ClearanceCodes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ClearanceRepository : IClearanceRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public ClearanceRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int CountAll => _dbContext.Query<Entities.Tenant.ClearanceCode>().Count();

        public void Add(ClearanceCode item)
        {
            var entity = _dbContext.Find<Entities.Tenant.ClearanceCode>(
                (item ?? throw new ArgumentNullException(nameof(item))).Id);
            if (entity == null)
            {
                _dbContext.Add(_mapper.Map<Entities.Tenant.ClearanceCode>(item), post => post.MapTo(item), _mapper);
            }
            else
            {
                _mapper.Map(item, entity);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }

        public void Add(IEnumerable<ClearanceCode> item) =>
            _dbContext.BulkInsertEngine.BulkInsertOrUpdate(_mapper.Map<List<Entities.Tenant.ClearanceCode>>(item),
                new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true },
                post => post.TryToUpdate(item), _mapper);

        public IEnumerable<ClearanceCode> GetAll() =>
            _dbContext.Query<Entities.Tenant.ClearanceCode>().ProjectTo<ClearanceCode>(_mapper.ConfigurationProvider).ToList();

        public int Count(Expression<Func<ClearanceCode, bool>> query) =>
            _dbContext.Query<Entities.Tenant.ClearanceCode>().ProjectTo<ClearanceCode>(_mapper.ConfigurationProvider).Count(query);

        public IEnumerable<ClearanceCode> FindByExternal(string externalRef) =>
            _dbContext.Query<Entities.Tenant.ClearanceCode>().Where(cc => cc.Code == externalRef)
                .ProjectTo<ClearanceCode>(_mapper.ConfigurationProvider).ToList();

        public IEnumerable<ClearanceCode> FindByExternal(List<string> externalRefs)
        {
            return _dbContext.Query<Entities.Tenant.ClearanceCode>().Where(cc => externalRefs.Contains(cc.Code))
                .ProjectTo<ClearanceCode>(_mapper.ConfigurationProvider).ToList();
        }

        public void Truncate() => _dbContext.Truncate<Entities.Tenant.ClearanceCode>();

        public ClearanceCode Find(int id) =>
            _mapper.Map<ClearanceCode>(_dbContext.Find<Entities.Tenant.ClearanceCode>(id));

        public void Remove(int id, out bool isDeleted)
        {
            isDeleted = false;
            var entity = _dbContext.Find<Entities.Tenant.ClearanceCode>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
                isDeleted = true;
            }
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        ClearanceCode IRepository<ClearanceCode>.Find(Guid uid) => throw new NotImplementedException();

        void IRepository<ClearanceCode>.Remove(Guid uid) => throw new NotImplementedException();
    }
}
