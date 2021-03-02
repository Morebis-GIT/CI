namespace xggameplan.Model
{
    /// <summary>
    /// Input for S3 process
    /// </summary>
    public class S3DownloadComment
    {
        public string BucketName { get; set; }
        public string FileName { get; set; }
        public string LocalFileFolder { get; set; }
    }
}
