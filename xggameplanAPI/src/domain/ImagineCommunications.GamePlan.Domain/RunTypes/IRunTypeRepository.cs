using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.RunTypes.Objects;

namespace ImagineCommunications.GamePlan.Domain.RunTypes
{
    public interface IRunTypeRepository
    {
        void Add(RunType runType);
        RunType Get(int id);
        IEnumerable<RunType> GetByIds(IEnumerable<int> ids);
        IEnumerable<RunType> GetAll();
        void Delete(int id);
        RunType FindByName(string name);
        void SaveChanges();
        void Update(RunType runType);
    }
}
