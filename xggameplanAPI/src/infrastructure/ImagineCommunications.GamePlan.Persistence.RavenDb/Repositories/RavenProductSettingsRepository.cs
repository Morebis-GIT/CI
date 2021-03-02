using System;
using ImagineCommunications.GamePlan.Domain.Shared.System.Products;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    [Obsolete("Repository is not used")]
    public class RavenProductSettingsRepository : IProductSettingsRepository
    {
        private readonly IRavenMasterDbContext _dbContext;

        public RavenProductSettingsRepository(IRavenMasterDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Add(ProductSettings productSettings)
        {
            _dbContext.Add(productSettings);
        }

        public ProductSettings Get(int id) =>
            _dbContext.Find<ProductSettings>(id);

        public void Update(ProductSettings productSettings) =>
            _dbContext.Update(productSettings);
    }
}
