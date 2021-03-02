using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.LandmarkRunQueues
{
    public interface ILandmarkRunQueueRepository
    {
        IEnumerable<Objects.LandmarkRunQueue> GetAll();
    }
}
