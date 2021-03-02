using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ProgrammeCategoryHierarchyRepository : IProgrammeCategoryHierarchyRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProgrammeCategoryHierarchyRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void AddRange(IEnumerable<ProgrammeCategoryHierarchy> programmeCategories) =>
            _dbContext.AddRange(_mapper.Map<Entities.Tenant.ProgrammeCategoryHierarchy[]>(programmeCategories),
                post => post.MapToCollection(programmeCategories), _mapper);

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Truncate() => _dbContext.Truncate<Entities.Tenant.ProgrammeCategoryHierarchy>();

        public IEnumerable<ProgrammeCategoryHierarchy> Search(IEnumerable<string> programmeCategoryNames) =>
            _dbContext.Query<Entities.Tenant.ProgrammeCategoryHierarchy>()
                .Where(x => programmeCategoryNames.Contains(x.Name))
                .ProjectTo<ProgrammeCategoryHierarchy>(_mapper.ConfigurationProvider)
                .ToList();

        public ProgrammeCategoryHierarchy Get(int id) => _mapper.Map<ProgrammeCategoryHierarchy>(_dbContext.Find<Entities.Tenant.ProgrammeCategoryHierarchy>(id));

        public IEnumerable<ProgrammeCategoryHierarchy> GetAll() =>
            _dbContext.Query<Entities.Tenant.ProgrammeCategoryHierarchy>()
                .ProjectTo<ProgrammeCategoryHierarchy>(_mapper.ConfigurationProvider)
                .ToList();
    }
}
