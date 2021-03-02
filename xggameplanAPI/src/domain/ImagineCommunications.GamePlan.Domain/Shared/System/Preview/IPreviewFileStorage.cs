using System.IO;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Preview
{
    public interface IPreviewFileStorage
    {
        PreviewFile GetPreviewFile(int entityId, out Stream previewFileStream);
        void SetPreviewFile(int entityId, PreviewFile previewFile, Stream previewFileStream);
        void DeletePreviewFile(int entitiId);
        void Flush();
    }
}
