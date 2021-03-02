using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenUpdateDetailsRepository : IUpdateDetailsRepository
    {
        private readonly IRavenMasterDbContext _dbContext;

        public RavenUpdateDetailsRepository(IRavenMasterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public UpdateDetails Find(Guid id) =>
            _dbContext.Find<UpdateDetails>(id);

        public List<UpdateDetails> GetAll() =>
            _dbContext.Specific.GetAll<UpdateDetails>().ToList();

        public void Add(UpdateDetails update) =>
            _dbContext.Add(update);

        public void Update(UpdateDetails update) =>
            _dbContext.Update(update);

        public void Remove(Guid id)
        {
            var entity = _dbContext.Find<UpdateDetails>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void SaveChanges() =>
            _dbContext.SaveChanges();
    }
}
