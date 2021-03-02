using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ProgrammeCategoryHierarchyEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ProgrammeCategoryHierarchy;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ProgrammeCategoryHierarchyModelHandler : IDomainModelHandler<ProgrammeCategoryHierarchy>
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProgrammeCategoryHierarchyModelHandler(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public ProgrammeCategoryHierarchy Add(ProgrammeCategoryHierarchy model) => throw new NotImplementedException();

        public void AddRange(params ProgrammeCategoryHierarchy[] models)
        {
            _dbContext.AddRange(_mapper
                .Map<List<ProgrammeCategoryHierarchyEntity>>(models).ToArray());
        }

        public int Count() => _dbContext.Query<ProgrammeCategoryHierarchyEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<ProgrammeCategoryHierarchyEntity>();

        public IEnumerable<ProgrammeCategoryHierarchy> GetAll() =>
            _dbContext.Query<ProgrammeCategoryHierarchyEntity>()
                .ProjectTo<ProgrammeCategoryHierarchy>(_mapper.ConfigurationProvider)
                .AsEnumerable();
    }
}
