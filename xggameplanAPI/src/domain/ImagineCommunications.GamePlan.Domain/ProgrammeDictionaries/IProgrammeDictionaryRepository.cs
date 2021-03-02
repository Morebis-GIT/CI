using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries
{
    public interface IProgrammeDictionaryRepository
    {
        ProgrammeDictionary Find(int id);

        IEnumerable<ProgrammeDictionary> FindByExternal(List<string> externalRefs);

        IEnumerable<ProgrammeDictionary> GetAll();

        int CountAll { get; }
    }
}
