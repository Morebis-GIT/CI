using System;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Product
{
    public class Advertiser
    {
        public string Name { get; }
        public string ShortName { get; }
        public string AdvertiserIdentifier { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public Advertiser(string name, string shortName, string advertiserIdentifier, DateTime startDate, DateTime endDate)
        {
            Name = name;
            ShortName = shortName;
            AdvertiserIdentifier = advertiserIdentifier;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
