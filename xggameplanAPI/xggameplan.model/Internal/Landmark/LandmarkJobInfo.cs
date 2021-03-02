using System;

namespace xggameplan.model.Internal.Landmark
{
    public class LandmarkJobInfo
    {
        public Guid? ProcessingId { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; }
    }
}
