using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces
{
    public interface IPreviewable
    {
        PreviewFile Preview { get; set; }

    }
}
