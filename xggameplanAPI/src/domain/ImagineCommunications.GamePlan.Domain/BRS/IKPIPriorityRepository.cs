using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.BRS
{
    public interface IKPIPriorityRepository
    {
        IEnumerable<KPIPriority> GetAll();
    }
}
