using xggameplan.Model;

namespace xggameplan.core.Interfaces
{
    public interface IPassInspectorService
    {
        bool InspectPassSalesAreaPriorities(
            PassSalesAreaPriorityModel passSalesAreaPriorities,
            out string errorMessage);
    }
}
