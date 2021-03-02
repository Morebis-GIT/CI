using xggameplan.common.ActionProcessing;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing
{
    public interface IPostActionBuilder
    {
        IAction Build();
    }
}
