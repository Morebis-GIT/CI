using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class ProgrammeDictionaryAdapter : NoCrudRepositoryTestAdapter<ProgrammeDictionary, IProgrammeDictionaryRepository, int>
    {
        public ProgrammeDictionaryAdapter(IScenarioDbContext dbContext, IProgrammeDictionaryRepository repository) : base(dbContext, repository)
        {
        }

        protected override int Count()
        {
            return Repository.CountAll;
        }

        protected override IEnumerable<ProgrammeDictionary> GetAll()
        {
            return Repository.GetAll();
        }

        protected override ProgrammeDictionary GetById(int id)
        {
            return Repository.Find(id);
        }
    }
}
