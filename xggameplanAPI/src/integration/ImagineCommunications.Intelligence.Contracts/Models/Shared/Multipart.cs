using System.Collections.Generic;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class Multipart
    {
        public Multipart(int multipartNumber, List<MultipartLength> lengths, decimal desiredPercentageSplit, decimal currentPercentageSplit)
        {
            MultipartNumber = multipartNumber;
            Lengths = lengths;
            DesiredPercentageSplit = desiredPercentageSplit;
            CurrentPercentageSplit = currentPercentageSplit;
        }

        public int MultipartNumber { get; }
        public decimal DesiredPercentageSplit { get; }
        public decimal CurrentPercentageSplit { get; }

        public List<MultipartLength> Lengths { get; }
    }
}
