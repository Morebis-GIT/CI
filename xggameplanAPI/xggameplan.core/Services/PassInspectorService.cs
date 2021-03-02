using xggameplan.core.Interfaces;
using xggameplan.Model;

namespace xggameplan.core.Services
{
    public class PassInspectorService : IPassInspectorService
    {
        public bool InspectPassSalesAreaPriorities(
            PassSalesAreaPriorityModel passSalesAreaPriorities,
            out string errorMessage)
        {
            errorMessage = null;

            if (passSalesAreaPriorities.StartTime is null ||
                passSalesAreaPriorities.EndTime is null)
            {
                errorMessage = "Pass sales area priorities start and end time are required";
            }
            else if (string.IsNullOrWhiteSpace(passSalesAreaPriorities.DaysOfWeek))
            {
                errorMessage = "Pass sales area priorities day of week is required";
            }

            return errorMessage != null;
        }
    }
}
