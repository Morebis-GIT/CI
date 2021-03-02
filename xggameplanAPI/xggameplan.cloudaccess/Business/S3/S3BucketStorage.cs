using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using xggameplan.Model;

namespace xggameplan.cloudaccess.Business.S3
{
    /// <summary>
    /// Exposes functionality to work with the specified S3 bucket
    /// represented by <see cref="IS3BucketSettings"/> parameter.
    /// </summary>
    /// <seealso cref="xggameplan.cloudaccess.Business.S3.S3BucketStorageBase" />
    public class S3BucketStorage : S3BucketStorageBase
    {
        private static readonly char[] _filePathDelimiters = { '/', '\\' };

        private readonly IS3BucketSettings _bucketSettings;
        private readonly string[] _rootFolderParts;

        protected override string AdjustKey(string key)
        {
            var keyParts = key.Split(_filePathDelimiters, StringSplitOptions.None);

            return string.Join("/", _rootFolderParts.Union(keyParts));
        }

        protected override Task EnsureBucketExistsAsync() => _bucketSettings.EnsureBucketExists
            ? base.EnsureBucketExistsAsync()
            : Task.CompletedTask;

        protected override S3CannedACL UploadCannedAcl => _bucketSettings.UploadCannedAcl;

        public S3BucketStorage(IS3BucketSettings bucketSettings, AwsConfiguration configuration) : base(configuration)
        {
            _bucketSettings = bucketSettings;
            _rootFolderParts = bucketSettings.RootFolder?.Split(_filePathDelimiters, StringSplitOptions.RemoveEmptyEntries) ??
                               Array.Empty<string>();
        }

        public override string BucketName => _bucketSettings.BucketName;
    }
}
