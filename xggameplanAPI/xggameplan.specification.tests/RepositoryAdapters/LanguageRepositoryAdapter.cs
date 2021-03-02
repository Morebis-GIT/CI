using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Languages;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class LanguageRepositoryAdapter : RepositoryTestAdapter<Language, ILanguageRepository, int>
    {
        public LanguageRepositoryAdapter(IScenarioDbContext dbContext, ILanguageRepository repository) :
            base(dbContext, repository)
        {
        }

        protected override Language Add(Language model)
        {
            Repository.Add(model);
            return model;
        }

        protected override IEnumerable<Language> GetAll()
        {
            return Repository.GetAll();
        }

        protected override IEnumerable<Language> AddRange(params Language[] models)
        {
            foreach(var language in models)
            {
                Repository.Add(language);
            }
            return models;
        }

        protected override int Count()
        {
            throw new NotImplementedException();
        }

        protected override void Delete(int id)
        {
            throw new NotImplementedException();
        }

        protected override Language GetById(int id)
        {
            throw new NotImplementedException();
        }

        protected override void Truncate()
        {
            throw new NotImplementedException();
        }

        protected override Language Update(Language model)
        {
            throw new NotImplementedException();
        }
    }
}
