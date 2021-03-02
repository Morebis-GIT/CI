using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto.Internal;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using FunctionalArea = ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects.FunctionalArea;
using FaultType = ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects.FaultType;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class FunctionalAreaRepository : IFunctionalAreaRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        private Dictionary<int, Entities.Tenant.FunctionalAreas.FaultType> _faultTypes;

        protected Dictionary<int, Entities.Tenant.FunctionalAreas.FaultType> FaultTypes =>
            _faultTypes ?? (_faultTypes = _dbContext.Query<Entities.Tenant.FunctionalAreas.FaultType>().Include(x => x.Descriptions)
                .ToDictionary(k => k.Id, v => v));

        public FunctionalAreaRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(FunctionalArea functionalArea) =>
            _dbContext.Add(_mapper.Map<Entities.Tenant.FunctionalAreas.FunctionalArea>(functionalArea));

        public FunctionalArea Find(Guid id) =>
            _dbContext.Query<Entities.Tenant.FunctionalAreas.FunctionalArea>()
                .ProjectTo<FunctionalAreaDto>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Id == id)
                .ApplyFaultTypes(FaultTypes, _mapper);

        public FaultType FindFaultType(int faultTypeId) =>
            _dbContext.Query<Entities.Tenant.FunctionalAreas.FaultType>()
                .ProjectTo<FaultType>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Id == faultTypeId);

        public IEnumerable<FaultType> FindFaultTypes(List<int> faultTypeIds) =>
            _dbContext.Query<Entities.Tenant.FunctionalAreas.FaultType>()
                .Where(x => faultTypeIds.Contains(x.Id))
                .ProjectTo<FaultType>(_mapper.ConfigurationProvider)
                .ToList();

        public IEnumerable<FunctionalArea> GetAll() =>
            _dbContext.Query<Entities.Tenant.FunctionalAreas.FunctionalArea>()
                .ProjectTo<FunctionalAreaDto>(_mapper.ConfigurationProvider)
                .ApplyFaultTypes(FaultTypes, _mapper)
                .ToList();

        public IEnumerable<int> GetSelectedFailureTypeIds() =>
            _dbContext.Query<Entities.Tenant.FunctionalAreas.FunctionalAreaFaultType>()
                .Where(x => x.IsSelected)
                .Select(x => x.FaultTypeId)
                .Distinct()
                .ToList();

        public void UpdateFaultTypesSelections(FunctionalArea functionalArea)
        {
            if (functionalArea is null)
            {
                return;
            }

            var faultTypeSettings = functionalArea.FaultTypes.ToDictionary(x => x.Id, x => x.IsSelected);
            var functionalAreaFaultTypes = _dbContext.Query<Entities.Tenant.FunctionalAreas.FunctionalAreaFaultType>()
                .Where(x => x.FunctionalAreaId == functionalArea.Id)
                .ToList();

            foreach (var functionalAreaFaultType in functionalAreaFaultTypes)
            {
                if (faultTypeSettings.ContainsKey(functionalAreaFaultType.FaultTypeId) &&
                    faultTypeSettings[functionalAreaFaultType.FaultTypeId] != functionalAreaFaultType.IsSelected)
                {
                    functionalAreaFaultType.IsSelected = faultTypeSettings[functionalAreaFaultType.FaultTypeId];
                    _dbContext.Update(functionalAreaFaultType);
                }
            }
        }
    }
}
