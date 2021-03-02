namespace xggameplan.cloudaccess.Business.S3
{
    public interface IS3BucketSettings
    {
        /// <summary>
        /// The bucket name.
        /// </summary>
        string BucketName { get; }
        /// <summary>
        /// Root folder within a bucket.
        /// </summary>
        string RootFolder { get; }
        /// <summary>
        /// https://docs.aws.amazon.com/AmazonS3/latest/dev/acl-overview.html#canned-acl
        string UploadCannedAcl { get; }
        /// <summary>
        /// Gets a value indicating whether it's needed to check a bucket existence and create it if necessary
        /// </summary>
        bool EnsureBucketExists { get; }
    }
}
