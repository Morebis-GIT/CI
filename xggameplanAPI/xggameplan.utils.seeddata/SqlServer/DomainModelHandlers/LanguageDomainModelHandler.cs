using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Languages;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using LanguageEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Language;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class LanguageDomainModelHandler : IDomainModelHandler<Language>
    {
        private readonly ILanguageRepository _languageRepository;
        private readonly ISqlServerDbContext _dbContext;

        public LanguageDomainModelHandler(ILanguageRepository languageRepository, ISqlServerDbContext dbContext)
        {
            _languageRepository = languageRepository ?? throw new ArgumentNullException(nameof(languageRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Language Add(Language model)
        {
            _languageRepository.Add(model);
            return model;
        }

        public void AddRange(params Language[] models)
        {
            foreach (var model in models)
            {
                _languageRepository.Add(model);
            }
        }

        public int Count() => _dbContext.Query<LanguageEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<LanguageEntity>();

        public IEnumerable<Language> GetAll() => _languageRepository.GetAll();
    }
}
