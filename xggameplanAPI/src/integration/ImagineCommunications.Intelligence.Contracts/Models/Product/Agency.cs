using System;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Product
{
    public class Agency
    {
        public string Name { get; }
        public string ShortName { get; }
        public string AgencyIdentifier { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public string AgencyGroupShortName { get; }
        public string AgencyGroupCode { get; }

        public Agency(string name, string shortName, string agencyIdentifier, DateTime startDate, DateTime endDate, string agencyGroupShortName, string agencyGroupCode)
        {
            Name = name;
            ShortName = shortName;
            AgencyIdentifier = agencyIdentifier;
            StartDate = startDate;
            EndDate = endDate;
            AgencyGroupShortName = agencyGroupShortName;
            AgencyGroupCode = agencyGroupCode;
        }
    }
}
