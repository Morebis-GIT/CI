using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Infrastructure.TestModelConverters;
using xggameplan.specification.tests.Infrastructure.TestModels;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class TenantsRepositoryAdapter :
               ConvertibleRepositoryTestAdapter<Tenant, TenantTestModel, ITenantsRepository, int>
    {
        private ITestModelConverter<Tenant, TenantTestModel> _modelConverter;
        public TenantsRepositoryAdapter(
            IScenarioDbContext dbContext,
            ITenantsRepository repository
            ) : base(dbContext, repository) { }

        protected override ITestModelConverter<Tenant, TenantTestModel> ModelConverter =>
            _modelConverter ?? (_modelConverter = new TenantTestModelConverter());

        protected override Tenant Add(Tenant model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Tenant> AddRange(params Tenant[] models) =>
            throw new NotImplementedException();

        protected override int Count() =>
            throw new NotImplementedException();

        protected override void Delete(int id) =>
            throw new NotImplementedException();

        protected override IEnumerable<Tenant> GetAll() =>
            Repository.GetAll();

        protected override Tenant GetById(int id) =>
            Repository.GetById(id);

        protected override void Truncate() =>
            throw new NotImplementedException();

        protected override Tenant Update(Tenant model) =>
            throw new NotImplementedException();
    }
}
