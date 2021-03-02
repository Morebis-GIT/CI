using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using EfficiencySettingsEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.EfficiencySettings;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class EfficiencySettingsDomainModelHandler : IDomainModelHandler<EfficiencySettings>
    {
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public EfficiencySettingsDomainModelHandler(ISqlServerDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public EfficiencySettings Add(EfficiencySettings model)
        {
            _ = _dbContext.Add(_mapper.Map<EfficiencySettingsEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params EfficiencySettings[] models) =>
            _dbContext.BulkInsertEngine.BulkInsert(_mapper.Map<List<EfficiencySettingsEntity>>(models),
                post => post.TryToUpdate(models), _mapper);

        public int Count() => _dbContext.Query<EfficiencySettingsEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<EfficiencySettingsEntity>();

        public IEnumerable<EfficiencySettings> GetAll() =>
            _dbContext.Query<EfficiencySettingsEntity>().ProjectTo<EfficiencySettings>(_mapper.ConfigurationProvider).AsEnumerable();
    }
}
