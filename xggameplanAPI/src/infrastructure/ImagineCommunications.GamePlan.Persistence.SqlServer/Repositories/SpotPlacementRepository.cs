using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class SpotPlacementRepository : ISpotPlacementRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public SpotPlacementRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void Add(SpotPlacement item)
        {
            _dbContext.Add(_mapper.Map<Entities.Tenant.SpotPlacement>(item), post => post.MapTo(item), _mapper);
        }

        public void Delete(int id)
        {
            var entity = _dbContext.Find<Entities.Tenant.SpotPlacement>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void Delete(string externalSpotRef)
        {
            var entity = _dbContext.Query<Entities.Tenant.SpotPlacement>()
                                .FirstOrDefault(x => x.ExternalSpotRef == externalSpotRef);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void DeleteBefore(DateTime modifiedTime)
        {
            const int size = 10000;
            var ids = _dbContext.Query<Entities.Tenant.SpotPlacement>()
                .Where(x => x.ModifiedTime <= modifiedTime)
                .Select(e => e.Id).ToList();
            if (!ids.Any())
            {
                return;
            }

            using (var transaction = _dbContext.Specific.Database.BeginTransaction())
            {
                for (int i = 0, page = 0; i < ids.Count; i += size, page++)
                {
                    _dbContext.Specific.RemoveByIdentityIds<Entities.Tenant.SpotPlacement>(ids.Skip(size * page).Take(size)
                        .ToArray());
                }

                transaction.Commit();
            }
        }

        public SpotPlacement GetByExternalSpotRef(string externalSpotRef) =>
            _mapper.Map<SpotPlacement>(_dbContext.Query<Entities.Tenant.SpotPlacement>()
                    .FirstOrDefault(x => x.ExternalSpotRef == externalSpotRef));

        public List<SpotPlacement> GetByExternalSpotRefs(IEnumerable<string> externalSpotRefs) =>
            _dbContext.Query<Entities.Tenant.SpotPlacement>()
                    .Where(x => externalSpotRefs.Contains(x.ExternalSpotRef))
                    .OrderBy(x => x.ExternalSpotRef)
                    .ProjectTo<SpotPlacement>(_mapper.ConfigurationProvider)
                    .ToList();

        public void Insert(IEnumerable<SpotPlacement> spotPlacements) =>
            _dbContext.BulkInsertEngine.BulkInsertOrUpdate(
                _mapper.Map<List<Entities.Tenant.SpotPlacement>>(spotPlacements),
                new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true },
                post => post.TryToUpdate(spotPlacements),
                _mapper);

        public void SaveChanges() => _dbContext.SaveChanges();

        public void Update(SpotPlacement spotPlacement)
        {
            var entity = _dbContext.Find<Entities.Tenant.SpotPlacement>(
                        (spotPlacement ?? throw new ArgumentNullException(nameof(spotPlacement))).Id);
            if (entity != null)
            {
                _mapper.Map(spotPlacement, entity);
                _dbContext.Update(entity, post => post.MapTo(spotPlacement), _mapper);
            }
        }
    }
}
