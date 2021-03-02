using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class KPIComparisonConfigRepositoryAdapter :
        NoCrudRepositoryTestAdapter<KPIComparisonConfig, IKPIComparisonConfigRepository, string>
    {
        public KPIComparisonConfigRepositoryAdapter(IScenarioDbContext dbContext, IKPIComparisonConfigRepository repository) : base(dbContext, repository)
        {
        }

        protected override KPIComparisonConfig GetById(string id)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<KPIComparisonConfig> GetAll()
        {
            return Repository.GetAll();
        }
    }
}
