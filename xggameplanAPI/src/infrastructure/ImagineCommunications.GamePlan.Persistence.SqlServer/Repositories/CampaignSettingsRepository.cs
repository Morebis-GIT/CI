using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using CampaignSettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns.CampaignSettings;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class CampaignSettingsRepository : ICampaignSettingsRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IFullTextSearchConditionBuilder _searchConditionBuilder;
        private readonly IMapper _mapper;

        public CampaignSettingsRepository(ISqlServerTenantDbContext dbContext,
            IFullTextSearchConditionBuilder searchConditionBuilder, IMapper mapper)
        {
            _dbContext = dbContext;
            _searchConditionBuilder = searchConditionBuilder;
            _mapper = mapper;
        }

        public void Add(CampaignSettings item) =>
            _dbContext.Add(_mapper.Map<CampaignSettingsEntity>(item));

        public void AddRange(IEnumerable<CampaignSettings> items)
        {
            var entities = _mapper.Map<List<CampaignSettingsEntity>>(items);

            _dbContext.BulkInsertEngine.BulkInsertOrUpdate(
                entities,
                new BulkInsertOptions { SetOutputIdentity = true, PreserveInsertOrder = true },
                post => post.TryToUpdate(items),
                _mapper);
        }

        public int Count() => _dbContext.Query<CampaignSettingsEntity>().Count();

        public void Delete(int id)
        {
            var entity = _dbContext.Query<CampaignSettingsEntity>()
                .FirstOrDefault(cs => cs.Id == id);

            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void DeleteByExternal(string externalId)
        {
            var entity = _dbContext.Query<CampaignSettingsEntity>()
                .FirstOrDefault(cs => cs.CampaignExternalId == externalId);

            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public bool Exists(string externalId) => _dbContext.Query<CampaignSettingsEntity>()
            .ProjectTo<CampaignSettings>(_mapper.ConfigurationProvider)
            .Any(cs => cs.CampaignExternalId == externalId);

        public CampaignSettings Get(int id) => _dbContext
            .Query<CampaignSettingsEntity>()
            .ProjectTo<CampaignSettings>(_mapper.ConfigurationProvider)
            .FirstOrDefault(cs => cs.Id == id);

        public IEnumerable<CampaignSettings> GetAll() => _dbContext
            .Query<CampaignSettingsEntity>()
            .ProjectTo<CampaignSettings>(_mapper.ConfigurationProvider)
            .ToArray();

        public CampaignSettings GetByExternalId(string externalId) => _dbContext
            .Query<CampaignSettingsEntity>()
            .ProjectTo<CampaignSettings>(_mapper.ConfigurationProvider)
            .FirstOrDefault(cs => cs.CampaignExternalId == externalId);

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Truncate() => _dbContext.Truncate<CampaignSettingsEntity>();

        public Task TruncateAsync() => Task.Run(Truncate);

        public void Update(CampaignSettings item)
        {
            var entity = _dbContext.Query<CampaignSettingsEntity>()
                .FirstOrDefault(cs => cs.Id == item.Id);

            if (entity != null)
            {
                _mapper.Map(item, entity);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }
    }
}
