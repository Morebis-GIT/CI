using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using FacilityEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Facility;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class FacilityRepository : IFacilityRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public FacilityRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(Facility facility) => _dbContext.Add(_mapper.Map<FacilityEntity>(facility), post => post.MapTo(facility), _mapper);

        public void Delete(int id)
        {
            var entity = _dbContext.Find<FacilityEntity>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public Facility Get(int id) => _mapper.Map<Facility>(_dbContext.Find<FacilityEntity>(id));

        public Facility GetByCode(string code) =>
            _dbContext.Query<FacilityEntity>()
                .ProjectTo<Facility>(_mapper.ConfigurationProvider)
                .FirstOrDefault(x => x.Code == code);

        public IEnumerable<Facility> GetAll() =>
            _dbContext.Query<FacilityEntity>()
                .ProjectTo<Facility>(_mapper.ConfigurationProvider)
                .ToList();

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Update(Facility facility)
        {
            var existingEntity = _dbContext.Find<FacilityEntity>(facility.Id);
            if (existingEntity is null)
            {
                return;
            }

            _mapper.Map(facility, existingEntity);
            _dbContext.Update(existingEntity, post => post.MapTo(facility), _mapper);
        }
    }
}
