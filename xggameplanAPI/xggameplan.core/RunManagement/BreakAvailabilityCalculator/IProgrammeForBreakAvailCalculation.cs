using System;
using NodaTime;

namespace xggameplan.core.RunManagement.BreakAvailabilityCalculator
{
    public interface IProgrammeForBreakAvailCalculation
    {
        Guid ProgrammeId { get; }
        
        DateTime StartDateTime { get; }
        
        Duration Duration { get; }
        
        string ExternalReference { get; }
        
        string SalesArea { get; }
    }
}
