using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.Languages
{
    public interface ILanguageRepository
    {
        /// <summary>
        /// Gets all languages
        /// </summary>
        /// <returns></returns>
        IEnumerable<Language> GetAll();

        /// <summary>
        /// Adds language
        /// </summary>
        /// <param name="language"></param>
        void Add(Language language);
    }
}
