using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace xggameplan.cloudaccess.Engine.Azure
{
    public class Azure
    {
        private const string Bucketname = "xg-schedule";
        private const string Keyname = "bbbbbbb";

        public Azure()
        {
          

        }

        public Task<bool> CreateBucket(string bucketName)
        {
            throw new NotImplementedException();
        }

        public bool CreateFile(MemoryStream memoryStream)
        {
            
                // Create file in memory
                UnicodeEncoding uniEncoding = new UnicodeEncoding();

                // Create the data to write to the stream.
                byte[] memstring = uniEncoding.GetBytes("you're file content here");

                var chain = new CredentialProfileStoreChain();
                AWSCredentials awsCredentials;
                if (chain.TryGetAWSCredentials("s3profile", out awsCredentials))
                {
                    // use awsCredentials
                }


                using (var memStream = new MemoryStream(100))
                {
                    memStream.Write(memstring, 0, memstring.Length);

                    // upload to s3
                    try
                    {
                        AmazonS3Client s3 = new AmazonS3Client(awsCredentials, RegionEndpoint.EUWest1);

                        TransferUtilityUploadRequest fileTransferUtilityRequest = new TransferUtilityUploadRequest
                        {
                            BucketName = Bucketname,
                            InputStream = memStream,
                          //  FilePath = filePath,
                            StorageClass = S3StorageClass.ReducedRedundancy,
                            PartSize = 6291456, // 6 MB.
                            Key = Keyname,
                            CannedACL = S3CannedACL.PublicRead
                        };
                  

                    using (var tranUtility =
                            new TransferUtility(s3))
                        {
                            tranUtility.Upload(fileTransferUtilityRequest);

                        return true;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }

            
        }

        public bool CreateFile(MemoryStream memStream, string bucketName, string fullFileName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteBucket(string bucketName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteFile(string bucketName, string fileName)
        {
            throw new NotImplementedException();
        }

        public Task<string> DownloadFile(string bucketName, string fileName, string localFilePath)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetBuckets()
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetFiles(string bucketName, string prefix = null, int maxFiles = 0)
        {
            throw new NotImplementedException();
        }

        public string GetPublicUrl(string bucketName, string fileName, string protocol)
        {
            throw new NotImplementedException();
        }

        public bool IsBucketExist(string bucketName, string fileName = null)
        {
            throw new NotImplementedException();
        }
        public bool IsFileExist(string bucketName, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
