using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Languages;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    /// <summary>
    /// Language repository.
    /// </summary>
    public class RavenLanguageRepository : ILanguageRepository
    {
        private readonly IDocumentSession _session;

        public RavenLanguageRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<Language> GetAll() =>
            _session.Query<Language>().Take(int.MaxValue).ToList();

        public void Add(Language language) =>
            _session.Store(language);
    }
}
