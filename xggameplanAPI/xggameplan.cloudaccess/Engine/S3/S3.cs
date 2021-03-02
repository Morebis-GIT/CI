using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using xggameplan.Model;

namespace xggameplan.cloudaccess.Engine.S3
{
    public class S3 : IS3
    {
        private const long PartSize = 6291456; // 6 MB.

        private static volatile IAmazonS3 _s3Client;
        private readonly AwsConfiguration _config;

        public S3(AwsConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// single instance property for amazon s3 client instance
        /// </summary>
        protected IAmazonS3 S3Client
        {
            get
            {
                if (_s3Client != null)
                {
                    return _s3Client;
                }

                if (!string.IsNullOrWhiteSpace(_config.ProfilesLocation) && File.Exists(_config.ProfilesLocation))
                {
                    var chain = new CredentialProfileStoreChain(_config.ProfilesLocation);
                    var profileName = !string.IsNullOrWhiteSpace(_config.Profile) ? _config.Profile : "default";
                    if (!chain.TryGetAWSCredentials(profileName, out AWSCredentials awsCredentials))
                    {
                        throw new ArgumentException("AWS Credential profile missing in configured profile location");
                    }

                    _s3Client = new AmazonS3Client(awsCredentials, RegionEndpoint.GetBySystemName(_config.Region));

                    return _s3Client;
                }

                _s3Client = new AmazonS3Client();

                return _s3Client;
            }
        }

        /// <summary>
        /// Create a new Bucket (folder)
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public bool CreateBucket(string bucketName)
        {
            var request = new PutBucketRequest() { BucketName = bucketName, UseClientRegion = true };
            _ = S3Client.PutBucketAsync(request).GetAwaiter().GetResult();

            return true;
        }

        /// <summary>
        /// Create file using stream
        /// </summary>
        /// <param name="memStream">file stream</param>
        /// <param name="bucketName">bucket name</param>
        /// <param name="fullFileName">folder name +file name</param>
        /// <returns></returns>
        public bool CreateFile(Stream memStream, string bucketName, string fullFileName)
        {
            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                InputStream = memStream,
                StorageClass = S3StorageClass.ReducedRedundancy,
                PartSize = PartSize,
                Key = fullFileName,
                CannedACL = S3CannedACL.PublicRead
            };

            using (var tranUtility = new TransferUtility(S3Client))
            {
                tranUtility.Upload(fileTransferUtilityRequest);

                return true;
            }
        }

        public bool CreateFile(string localPhysicalFilePath, string bucketName, string fullFileName)
        {
            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                FilePath = localPhysicalFilePath,
                StorageClass = S3StorageClass.ReducedRedundancy,
                PartSize = PartSize,
                Key = fullFileName,
                CannedACL = S3CannedACL.PublicRead
            };

            using (var tranUtility = new TransferUtility(S3Client))
            {
                tranUtility.Upload(fileTransferUtilityRequest);

                return true;
            }
        }

        /// <summary>
        /// Delete bucket
        /// </summary>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        public bool DeleteBucket(string bucketName)
        {
            var request = new DeleteBucketRequest() { BucketName = bucketName };

            _ = S3Client.DeleteBucketAsync(request).GetAwaiter().GetResult();

            return true;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool DeleteFile(string bucketName, string fileName)
        {
            var request = new DeleteObjectRequest { BucketName = bucketName, Key = fileName };

            _ = S3Client.DeleteObjectAsync(request).GetAwaiter().GetResult();

            return true;
        }

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public string DownloadFile(string bucketName, string fileName, string localFilePath)
        {
            localFilePath += "\\" + fileName;

            if (File.Exists(localFilePath))
            {
                throw new Exception("File already in local file path");
            }

            var request = new GetObjectRequest() { BucketName = bucketName, Key = fileName, };

            using (var response = S3Client.GetObjectAsync(request).GetAwaiter().GetResult())
            {
                response.WriteResponseStreamToFileAsync(localFilePath, false, CancellationToken.None).GetAwaiter()
                    .GetResult();

                return localFilePath;
            }
        }

        public string GetPreSignedUrl(string bucketName, string fileName)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = fileName,
                Expires = DateTime.Now.AddMinutes(10)
            };

            return _s3Client.GetPreSignedURL(request);
        }

        /// <summary>
        /// Get a list of existing buckets
        /// </summary>
        /// <returns></returns>
        public List<string> GetBuckets()
        {
            var response = S3Client.ListBucketsAsync().GetAwaiter().GetResult();
            var buckets = response.Buckets;

            if (buckets != null && buckets.Count != 0)
            {
                return buckets.Select(b => b.BucketName).ToList();
            }

            return null;
        }

        /// <summary>
        /// Query and get the file from bucket
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="prefix"></param>
        /// <param name="maxFiles"></param>
        /// <returns>list of filename</returns>
        public List<string> GetFiles(string bucketName, string prefix = null, int maxFiles = 0)
        {
            var request = new ListObjectsRequest() { BucketName = bucketName };

            if (!string.IsNullOrEmpty(prefix))
            {
                // list only things starting with prefix
                request.Prefix = prefix;
            }

            if (maxFiles != 0)
            {
                request.MaxKeys = maxFiles;
            }

            var response = S3Client.ListObjectsAsync(request).GetAwaiter().GetResult();

            if (response?.S3Objects != null && response.S3Objects.Count > 0)
            {
                return response.S3Objects.Select(f => f.Key).ToList();
            }

            return null;
        }

        /// <summary>
        /// Builds default Url to file
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public string GetPublicUrl(string bucketName, string fileName, string protocol)
        {
            return string.Format("{0}://{1}.s3.amazonaws.com/{2}", protocol, bucketName, fileName);
        }

        /// <summary>
        /// check is file/bucket exist
        /// </summary>
        /// <param name="bucketName">bucketName</param>
        /// <param name="fileName">fileName</param>
        /// <returns></returns>
        public bool IsBucketExist(string bucketName, string fileName = null)
        {
            var listobj = GetBuckets();

            if (listobj != null && listobj.Count > 0)
            {
                return listobj.Exists(s => s.Equals(bucketName, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        /// <summary>
        /// Give file name is exist or not
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool IsFileExist(string bucketName, string fileName)
        {
            var listobj = GetFiles(bucketName, fileName);

            if (listobj != null && listobj.Count > 0)
            {
                return listobj.Exists(s => s.Equals(fileName, StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }
    }
}
