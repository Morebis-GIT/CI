using ImagineCommunications.GamePlan.Domain.Maintenance.DatabaseDetail;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenDatabaseDetailsRepository : IDatabaseDetailsRepository
    {
        private readonly IRavenMasterDbContext _dbContext;

        public RavenDatabaseDetailsRepository(IRavenMasterDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public DatabaseDetails Find(int id) =>
            _dbContext.Find<DatabaseDetails>(id);

        public void Add(DatabaseDetails databaseDetails) =>
            _dbContext.Add(databaseDetails);

        public void Update(DatabaseDetails databaseDetails) =>
           _dbContext.Update(databaseDetails);


        public void Remove(int id)
        {
            var entity = _dbContext.Find<DatabaseDetails>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void SaveChanges() =>
            _dbContext.SaveChanges();
    }
}
