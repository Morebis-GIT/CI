using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects
{
    public class AgTimeSlices : List<AgTimeSlice>
    {
        public AgTimeSlices()
        {
        }

        public AgTimeSlices(IEnumerable<AgTimeSlice> collection) : base(collection)
        {
        }
    }
}
