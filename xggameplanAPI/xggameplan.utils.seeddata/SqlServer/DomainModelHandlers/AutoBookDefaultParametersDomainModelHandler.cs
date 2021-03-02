using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using AutoBookDefaultParametersEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters.AutoBookDefaultParameters;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class AutoBookDefaultParametersDomainModelHandler : IDomainModelHandler<AutoBookDefaultParameters>
    {
        private readonly IAutoBookDefaultParametersRepository _repository;
        private readonly ISqlServerTenantDbContext _dbContext;

        public AutoBookDefaultParametersDomainModelHandler(
            IAutoBookDefaultParametersRepository repository,
            ISqlServerTenantDbContext dbContext
        )
        {
            _repository = repository;
            _dbContext = dbContext;
        }

        public AutoBookDefaultParameters Add(AutoBookDefaultParameters model)
        {
            _repository.AddOrUpdate(model);

            return model;
        }

        public void AddRange(params AutoBookDefaultParameters[] models)
        {
            foreach (var model in models)
            {
                _repository.AddOrUpdate(model);
            }
        }

        public int Count() => _dbContext.Query<AutoBookDefaultParametersEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<AutoBookDefaultParametersEntity>();

        public IEnumerable<AutoBookDefaultParameters> GetAll()
        {
            var model = (AutoBookDefaultParameters) _repository.Get();

            return new AutoBookDefaultParameters[] { model };
        }
    }
}
