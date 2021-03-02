using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ProgrammeClassificationRepository : IProgrammeClassificationRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProgrammeClassificationRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public int CountAll => _dbContext.Query<Entities.Tenant.ProgrammeClassification>().Count();

        public IEnumerable<ProgrammeClassification> GetAll() => _dbContext.Query<Entities.Tenant.ProgrammeClassification>()
            .ProjectTo<ProgrammeClassification>(_mapper.ConfigurationProvider).ToList();

        public void Add(IEnumerable<ProgrammeClassification> programmeClassifications) =>
            _dbContext.BulkInsertEngine.BulkInsertOrUpdate(
                _mapper.Map<List<Entities.Tenant.ProgrammeClassification>>(programmeClassifications),
                new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true },
                post => post.TryToUpdate(programmeClassifications), _mapper);

        public ProgrammeClassification GetByCode(string code) =>
            _mapper.Map<ProgrammeClassification>(_dbContext.Query<Entities.Tenant.ProgrammeClassification>()
                .FirstOrDefault(e => e.Code == code));

        public ProgrammeClassification GetById(int id) =>
            _mapper.Map<ProgrammeClassification>(_dbContext.Query<Entities.Tenant.ProgrammeClassification>()
                .FirstOrDefault(e => e.Uid == id));

        public void Delete(int id)
        {
            var entity = _dbContext.Query<Entities.Tenant.ProgrammeClassification>()
                .FirstOrDefault(e => e.Uid == id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void Update(ProgrammeClassification programmeClassification)
        {
            var entity = _dbContext.Query<Entities.Tenant.ProgrammeClassification>()
                .FirstOrDefault(e => e.Uid == programmeClassification.Uid);
            if (entity != null)
            {
                _mapper.Map(programmeClassification, entity);
                _dbContext.Update(entity, post => post.MapTo(programmeClassification), _mapper);
            }
        }

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Truncate() => _dbContext.Truncate<Entities.Tenant.ProgrammeClassification>();
    }
}
