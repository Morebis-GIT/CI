using System.Diagnostics;
using xggameplan.cloudaccess.Business.S3;

namespace xggameplan.AutoBooks.AWS
{
    /// <summary>
    /// AWS S3 bucket settings
    /// </summary>
    [DebuggerDisplay("{string.Join(\"/\", S3Bucket, RootFolder)}")]
    public class AWSSettings : IS3BucketSettings
    {
        public string S3Bucket { get; set; }
        public string RootFolder { get; set; }
        public string UploadCannedAcl { get; set; } = "public-read";
        public bool EnsureBucketExists { get; set; } = true;

        string IS3BucketSettings.BucketName => S3Bucket;
    }
}
