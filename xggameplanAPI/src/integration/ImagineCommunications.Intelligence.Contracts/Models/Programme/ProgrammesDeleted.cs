using System;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Programme
{
    public class ProgrammesDeleted : IProgrammesDeleted
    {
        public ProgrammesDeleted(string salesArea, DateTime fromDate, DateTime toDate)
        {
            SalesArea = salesArea;
            FromDate = fromDate;
            ToDate = toDate;
        }

        public string SalesArea { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }
    }
}
