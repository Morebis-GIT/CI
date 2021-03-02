using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessTypes;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class BusinessTypeRepositoryAdapter : RepositoryTestAdapter<BusinessType, IBusinessTypeRepository, int>
    {
        public BusinessTypeRepositoryAdapter(IScenarioDbContext dbContext, IBusinessTypeRepository repository) : base(dbContext, repository)
        {
        }

        protected override BusinessType Add(BusinessType model)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<BusinessType> AddRange(params BusinessType[] models)
        {
            throw new NotImplementedException();
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        protected override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<BusinessType> GetAll()
        {
            return Repository.GetAll();
        }

        protected override BusinessType GetById(int id)
        {
            throw new NotImplementedException();
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override BusinessType Update(BusinessType model)
        {
            throw new NotImplementedException();
        }
    }
}
