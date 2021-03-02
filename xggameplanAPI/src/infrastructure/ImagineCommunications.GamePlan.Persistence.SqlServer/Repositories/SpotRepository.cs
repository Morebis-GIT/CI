using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using xggameplan.Common;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class SpotRepository : ISpotRepository
    {
        private const int _spotBatchSize = 1000;
        private const int _spotDeleteBatchSize = 10000;

        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public SpotRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int CountAll => _dbContext.Query<Entities.Tenant.Spot>().Count();

        public decimal GetNominalPriceByCampaign(string campaignExternalId) => _dbContext.Query<Entities.Tenant.Spot>()
            .Where(s => s.ExternalCampaignNumber == campaignExternalId &&
                        s.ClientPicked == false &&
                        !string.IsNullOrEmpty(s.ExternalBreakNo) &&
                        s.ExternalBreakNo != Globals.UnplacedBreakString)
            .Select(s => s.NominalPrice)
            .Sum();

        public void Add(Spot item) => _dbContext.Add(
            _mapper.Map<Entities.Tenant.Spot>(item),
            post => post.MapTo(item),
            _mapper);

        public void Add(IEnumerable<Spot> items)
        {
            var entities = _mapper.Map<List<Entities.Tenant.Spot>>(items);

            _dbContext.BulkInsertEngine.BulkInsertOrUpdate(
                entities,
                new BulkInsertOptions { SetOutputIdentity = true, PreserveInsertOrder = true },
                post => post.TryToUpdate(items),
                _mapper);
        }

        public void InsertOrReplace(IEnumerable<Spot> items)
        {
            var entities = _mapper.Map<List<Entities.Tenant.Spot>>(items);
            using (var transaction = _dbContext.Specific.Database.BeginTransaction())
            {
                var ids = entities.Select(s => s.ExternalSpotRef);
                var dataCount = ids.Count();
                for (int i = 0; i <= dataCount / _spotBatchSize; i++)
                {
                    var externalRefs = ids.Skip(i * _spotBatchSize).Take(_spotBatchSize);
                    var spotIds = _dbContext.Query<Entities.Tenant.Spot>()
                        .Where(x => externalRefs.Contains(x.ExternalSpotRef)).Select(r => r.Id)
                        .ToArray();

                    _dbContext.Specific.RemoveByIdentityIds<Entities.Tenant.Spot>(spotIds);
                }

                _dbContext.BulkInsertEngine.BulkInsert(entities,
                    new BulkInsertOptions { BatchSize = 100000 });

                transaction.Commit();
            }
        }

        public int Count(Expression<Func<Spot, bool>> query) => _dbContext
            .Query<Entities.Tenant.Spot>()
            .ProjectTo<Spot>(_mapper.ConfigurationProvider)
            .Where(query)
            .Count();

        public void Delete(IEnumerable<Guid> rawUids)
        {
            if (!rawUids.Any())
            {
                return;
            }

            var uids = rawUids.ToArray();
            using (var transaction = _dbContext.Specific.Database.BeginTransaction())
            {
                for (int i = 0, page = 0; i < uids.Length; i += _spotDeleteBatchSize, page++)
                {
                    var batchUIds = uids.Skip(_spotDeleteBatchSize * page).Take(_spotDeleteBatchSize).ToArray();

                    var idsToDelete = _dbContext.Query<Entities.Tenant.Spot>()
                        .Where(e => batchUIds.Contains(e.Uid))
                        .Select(x => x.Id)
                        .ToArray();

                    _ = _dbContext.Specific.RemoveByIdentityIds<Entities.Tenant.Spot>(idsToDelete);
                }

                transaction.Commit();
            }
        }

        public Spot Find(Guid uid) => _dbContext
            .Query<Entities.Tenant.Spot>()
            .ProjectTo<Spot>(_mapper.ConfigurationProvider)
            .FirstOrDefault(e => e.Uid == uid);

        public IEnumerable<Spot> FindByExternal(string externalRef) => _dbContext
            .Query<Entities.Tenant.Spot>()
            .Where(e => e.ExternalCampaignNumber == externalRef)
            .ProjectTo<Spot>(_mapper.ConfigurationProvider)
            .ToArray();

        public IEnumerable<Spot> FindByExternal(List<string> externalRefs) => _dbContext
            .Query<Entities.Tenant.Spot>()
            .Where(e => externalRefs.Contains(e.ExternalSpotRef))
            .ProjectTo<Spot>(_mapper.ConfigurationProvider)
            .ToArray();

        public IEnumerable<Spot> FindByExternalBreakNumbers(IEnumerable<string> externalBreakNumbers) => _dbContext
            .Query<Entities.Tenant.Spot>()
            .Where(e => externalBreakNumbers.Contains(e.ExternalBreakNo))
            .ProjectTo<Spot>(_mapper.ConfigurationProvider)
            .ToArray();

        public Spot FindByExternalSpotRef(string externalSpotRef) => _dbContext
            .Query<Entities.Tenant.Spot>()
            .ProjectTo<Spot>(_mapper.ConfigurationProvider)
            .FirstOrDefault(e => e.ExternalSpotRef == externalSpotRef);

        public IEnumerable<Spot> GetAll() => _dbContext
            .Query<Entities.Tenant.Spot>()
            .ProjectTo<Spot>(_mapper.ConfigurationProvider)
            .ToArray();

        public IEnumerable<Spot> GetAllMultipart() => _dbContext
            .Query<Entities.Tenant.Spot>()
            .Where(e => MultipartSpotTypes.All.Contains(e.MultipartSpot))
            .ProjectTo<Spot>(_mapper.ConfigurationProvider)
            .ToArray();

        public IEnumerable<Spot> GetAllByCampaign(string campaignExternalId) => _dbContext
            .Query<Entities.Tenant.Spot>()
            .Where(e => e.ExternalCampaignNumber == campaignExternalId)
            .ProjectTo<Spot>(_mapper.ConfigurationProvider)
            .ToArray();

        public void Remove(Guid uid)
        {
            var entity = _dbContext.Query<Entities.Tenant.Spot>()
                .FirstOrDefault(e => e.Uid == uid);

            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void SaveChanges() => _dbContext.SaveChanges();

        public IEnumerable<Spot> Search(DateTime datefrom, DateTime dateto, string salesarea) => _dbContext
            .Query<Entities.Tenant.Spot>()
            .Where(e =>
                e.SalesArea == salesarea &&
                e.StartDateTime >= datefrom &&
                e.StartDateTime <= dateto)
            .ProjectTo<Spot>(_mapper.ConfigurationProvider)
            .ToArray();

        public IEnumerable<Spot> Search(DateTime datefrom, DateTime dateto, List<string> salesareas) => _dbContext
            .Query<Entities.Tenant.Spot>()
            .Where(e =>
                salesareas.Contains(e.SalesArea) &&
                e.StartDateTime >= datefrom &&
                e.StartDateTime <= dateto)
            .ProjectTo<Spot>(_mapper.ConfigurationProvider)
            .ToArray();

        public void Truncate() => _dbContext.Truncate<Entities.Tenant.Spot>();

        public async Task TruncateAsync() => await Task
            .Run(() => _dbContext.Truncate<Entities.Tenant.Spot>())
            .ConfigureAwait(false);

        public void Update(Spot item)
        {
            var entity = _dbContext.Query<Entities.Tenant.Spot>()
                .FirstOrDefault(e => e.Uid == item.Uid);

            if (entity != null)
            {
                _mapper.Map(item, entity);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }
    }
}
