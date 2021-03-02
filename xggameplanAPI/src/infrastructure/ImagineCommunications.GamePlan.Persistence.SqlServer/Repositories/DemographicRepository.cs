using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class DemographicRepository : IDemographicRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public DemographicRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public IEnumerable<Demographic> GetAll()
        {
            return _dbContext.Query<Entities.Tenant.Demographic>().ProjectTo<Demographic>(_mapper.ConfigurationProvider).ToList();
        }

        public List<string> GetAllGameplanDemographics()
        {
            return _dbContext.Query<Entities.Tenant.Demographic>().Where(x => x.Gameplan).Select(x => x.ExternalRef)
                .ToList();
        }

        public void Add(IEnumerable<Demographic> channels) =>
            _dbContext.AddRange(_mapper.Map<Entities.Tenant.Demographic[]>(channels));

        public void UpdateRange(IEnumerable<Demographic> demographics) =>
            _dbContext.UpdateRange(_mapper.Map<Entities.Tenant.Demographic[]>(demographics),
                post => post.MapToCollection(demographics), _mapper);

        public Demographic GetByExternalRef(string externalRef)
        {
            return _mapper.Map<Demographic>(_dbContext.Query<Entities.Tenant.Demographic>()
                .FirstOrDefault(x => x.ExternalRef == externalRef));
        }

        public Demographic GetById(int id)
        {
            return _mapper.Map<Demographic>(_dbContext.Find<Entities.Tenant.Demographic>(id));
        }

        public void Delete(int id)
        {
            var entity = _dbContext.Find<Entities.Tenant.Demographic>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public int CountAll => _dbContext.Query<Entities.Tenant.Demographic>().Count();

        public void Update(Demographic demographic)
        {
            var entity = _dbContext.Find<Entities.Tenant.Demographic>(demographic.Id);
            if (entity != null)
            {
                _mapper.Map(demographic, entity);
                _dbContext.Update(entity, post => post.MapTo(demographic), _mapper);
            }
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public void Truncate()
        {
            _dbContext.Truncate<Entities.Tenant.Demographic>();
        }

        public IEnumerable<Demographic> GetByExternalRef(List<string> externalRefs)
        {
            return _dbContext.Query<Entities.Tenant.Demographic>()
                .Where(x => externalRefs.Contains(x.ExternalRef))
                .ProjectTo<Demographic>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            var demographics = _dbContext.Query<Entities.Tenant.Demographic>()
                .Where(x => externalRefs.Contains(x.ExternalRef))
                .ToArray();

            if (demographics.Any())
            {
                _dbContext.RemoveRange(demographics);
            }
        }

        public void InsertOrUpdate(IEnumerable<Demographic> items)
        {
            var dbItems = _mapper.Map<List<Entities.Tenant.Demographic>>(items);
            var existedDemographics = new HashSet<int>(GetAll().Select(e => e.Id));

            var demographicsToUpdate = dbItems.Where(e => existedDemographics.Contains(e.Id)).ToArray();
            var demographicsToAdd = dbItems.Except(demographicsToUpdate).ToArray();

            using (var transaction = _dbContext.Specific.Database.BeginTransaction())
            {
                _dbContext.AddRange(demographicsToAdd);
                _dbContext.UpdateRange(demographicsToUpdate);
                // _dbContext.Specific.IdentityInsertOn<Entities.Tenant.Demographic>();
                _dbContext.SaveChanges();
                // _dbContext.Specific.IdentityInsertOff<Entities.Tenant.Demographic>();
                transaction.Commit();
            }
        }
    }
}
