using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Dto
{
    public class ReducedSpotDTO
    {
        public int Id { get; set; }
        public string MultipartSpot { get; set; }
        public string MultipartSpotPosition { get; set; }
        public string MultipartSpotRef { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string ExternalSpotRef { get; set; }
    }
}
