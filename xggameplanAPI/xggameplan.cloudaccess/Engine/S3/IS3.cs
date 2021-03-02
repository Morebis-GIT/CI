using System.Collections.Generic;
using System.IO;

namespace xggameplan.cloudaccess.Engine.S3
{
    public interface IS3
    {
        bool CreateBucket(string bucketName);

        bool CreateFile(Stream memStream, string bucketName, string fullFileName);

        bool CreateFile(string localFilePath, string bucketName, string fullFileName);

        bool DeleteBucket(string bucketName);

        bool DeleteFile(string bucketName, string fileName);

        string DownloadFile(string bucketName, string fileName, string localFilePath);

        List<string> GetBuckets();

        List<string> GetFiles(string bucketName, string prefix = null, int maxFiles = 0);

        string GetPublicUrl(string bucketName, string fileName, string protocol);

        string GetPreSignedUrl(string bucketName, string fileName);

        bool IsBucketExist(string bucketName, string fileName = null);

        bool IsFileExist(string bucketName, string fileName);
    }
}
