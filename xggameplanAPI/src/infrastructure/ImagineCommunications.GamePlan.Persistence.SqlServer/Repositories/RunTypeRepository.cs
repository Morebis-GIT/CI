using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using ImagineCommunications.GamePlan.Domain.RunTypes.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using RunTypeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.RunType;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class RunTypeRepository : IRunTypeRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public RunTypeRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(RunType runType)
        {
            if (runType is null)
            {
                throw new ArgumentNullException(nameof(runType));
            }

            _dbContext.Add(_mapper.Map<RunTypeEntity>(runType), post => post.MapTo(runType), _mapper);
        }

        public void Delete(int id)
        {
            var entity = GetEntity(id);
            if (entity != null)
            {
                _dbContext.RemoveRange(entity);
            }
        }

        public RunType FindByName(string name) =>
            _mapper.Map<RunType>(_dbContext.Query<RunTypeEntity>()
                .ProjectTo<RunType>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));

        public RunType Get(int id)
        {
            var entity = GetEntity(id);
            return _mapper.Map<RunType>(entity);
        }

        public IEnumerable<RunType> GetByIds(IEnumerable<int> ids)
        {
            return _dbContext.Query<RunTypeEntity>()
                .Where(x => ids.Contains(x.Id))
                .ProjectTo<RunType>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public IEnumerable<RunType> GetAll() =>
            _dbContext.Query<RunTypeEntity>()
                .ProjectTo<RunType>(_mapper.ConfigurationProvider)
                .ToList();

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Update(RunType runType)
        {
            var entity = _dbContext
                .Query<RunTypeEntity>()
                .Include(o => o.RunTypeAnalysisGroups)
                .Include(o => o.RunLandmarkScheduleSettings)
                .FirstOrDefault(o => o.Id == runType.Id);
            if (entity != null)
            {
                if (entity.RunTypeAnalysisGroups != null && entity.RunTypeAnalysisGroups.Any())
                {
                    _dbContext.RemoveRange(entity.RunTypeAnalysisGroups.ToArray());
                    entity.RunTypeAnalysisGroups.Clear();
                }

                if (entity.RunLandmarkScheduleSettings != null)
                {
                    entity.RunLandmarkScheduleSettings.DaysOfWeek = null;
                }

                _mapper.Map(runType, entity);
                _dbContext.Update(entity, post => post.MapTo(runType), _mapper);
            }
        }

        private RunTypeEntity GetEntity(int id) => _dbContext.Find<RunTypeEntity>(id);
    }
}
