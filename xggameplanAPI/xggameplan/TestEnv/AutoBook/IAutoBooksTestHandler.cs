using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;

namespace xggameplan.TestEnv.AutoBook
{
    public interface IAutoBooksTestHandler
    {
        AutoBookStatuses GetStatus(string autoBookId);

        string GetVersion(string autoBookId);

        void ChangeStatus(string autoBookId, AutoBookStatuses newStatus);
    }
}
