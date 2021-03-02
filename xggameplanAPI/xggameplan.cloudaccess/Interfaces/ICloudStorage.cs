using xggameplan.Model;

namespace xggameplan.cloudaccess.Business
{
    public interface ICloudStorage
    {
        string GetPreSignedUrl(S3FileComment s3File);
        string Download(S3DownloadComment s3Input);
        bool Upload(S3UploadComment s3Input);
        bool FileExists(S3FileComment s3File);
        void DeleteFile(S3FileComment s3File);
        void DeleteFilesByPrefix(S3DeleteByPrefixComment s3Prefix);
    }
}
