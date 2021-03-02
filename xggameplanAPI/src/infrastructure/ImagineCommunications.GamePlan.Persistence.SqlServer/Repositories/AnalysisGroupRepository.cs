using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class AnalysisGroupRepository : IAnalysisGroupRepository
    {
        private const int MaxClauseCount = 1000;
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public AnalysisGroupRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(AnalysisGroup analysisGroup)
        {
            analysisGroup.IsDeleted = false;
            _dbContext.Add(_mapper.Map<Entities.Tenant.AnalysisGroups.AnalysisGroup>(analysisGroup), post => post.MapTo(analysisGroup), _mapper);
        }

        public void Delete(int id)
        {
            var entity = _dbContext
                .Query<Entities.Tenant.AnalysisGroups.AnalysisGroup>()
                .IgnoreQueryFilters()
                .FirstOrDefault(x => x.Id == id);

            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public IEnumerable<AnalysisGroup> GetAll() =>
            _mapper.Map<List<AnalysisGroup>>(_dbContext.Query<Entities.Tenant.AnalysisGroups.AnalysisGroup>()
                .ToArray());

        public AnalysisGroup Get(int id) =>
            _mapper.Map<AnalysisGroup>(_dbContext.Query<Entities.Tenant.AnalysisGroups.AnalysisGroup>()
                .FirstOrDefault(x => x.Id == id));

        public AnalysisGroup GetIncludingSoftDeleted(int id) =>
            _mapper.Map<AnalysisGroup>(_dbContext.Query<Entities.Tenant.AnalysisGroups.AnalysisGroup>()
                .IgnoreQueryFilters()
                .FirstOrDefault(x => x.Id == id));

        public IEnumerable<AnalysisGroupNameModel> GetByIds(IEnumerable<int> ids, bool onlyActive = false)
        {
            var distinctIds = ids.Distinct().ToList();
            var result = new List<AnalysisGroupNameModel>();
            for (int i = 0, page = 0; i < distinctIds.Count; i += MaxClauseCount, page++)
            {
                var batch = distinctIds.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                var batchQuery = _dbContext.Query<Entities.Tenant.AnalysisGroups.AnalysisGroup>();

                if (!onlyActive)
                {
                    batchQuery = batchQuery.IgnoreQueryFilters();
                }

                result.AddRange(batchQuery.Where(x => batch.Contains(x.Id))
                    .ProjectTo<AnalysisGroupNameModel>(_mapper.ConfigurationProvider).ToArray());
            }

            return result;
        }

        public AnalysisGroup GetByName(string name) =>
            _mapper.Map<AnalysisGroup>(_dbContext.Query<Entities.Tenant.AnalysisGroups.AnalysisGroup>()
                .FirstOrDefault(x => x.Name == name));

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Update(AnalysisGroup analysisGroup)
        {
            var entity = _dbContext
                .Query<Entities.Tenant.AnalysisGroups.AnalysisGroup>()
                .FirstOrDefault(x => x.Id == analysisGroup.Id);

            if (entity != null)
            {
                _mapper.Map(analysisGroup, entity);
                _dbContext.Update(entity, post => post.MapTo(analysisGroup), _mapper);
            }
        }
    }
}
