using System.IO;

namespace xggameplan.Model
{
    /// <summary>
    /// Input for S3 process
    /// </summary>
    public class S3UploadComment
    {
        public string BucketName { get; set; }
        /// <summary>
        /// File name in S3
        /// </summary>
        public string DestinationFilePath { get; set; }
        public Stream FileStream { get; set; }
        /// <summary>
        /// file name in local disk
        /// </summary>
        public string SourceFilePath { get; set; }
    }
}
