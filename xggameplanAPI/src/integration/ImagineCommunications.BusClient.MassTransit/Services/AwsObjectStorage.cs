using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.BusClient.Abstraction.Models;

namespace ImagineCommunications.BusClient.Implementation.Services
{
    /// <inheritdoc />
    public class AwsObjectStorage : IObjectStorage
    {
        private const int AWS_PART_SIZE = 6291456;
        private const string URI_SCHEME_NAME = "s3";
        private readonly Lazy<IAmazonS3> _client;
        private readonly ObjectStorageConfiguration _configuration;

        public AwsObjectStorage(ObjectStorageConfiguration configuration)
        {
            _configuration = configuration;
            _client = new Lazy<IAmazonS3>(AmazonS3ClientFactory);
        }

        private IAmazonS3 AmazonS3ClientFactory()
        {
            if (String.IsNullOrEmpty(_configuration.ProfilesLocation) || !File.Exists(_configuration.ProfilesLocation))
            {
                if (!String.IsNullOrEmpty(_configuration.AccessKeyId) && !String.IsNullOrEmpty(_configuration.AccessKey))
                {
                    return new AmazonS3Client(_configuration.AccessKeyId, _configuration.AccessKey,
                        RegionEndpoint.GetBySystemName(_configuration.Region));
                }
                else
                {
                    return  new AmazonS3Client();
                }
            }

            var chain = new CredentialProfileStoreChain(_configuration.ProfilesLocation);
            if (!chain.TryGetAWSCredentials(_configuration.ProfileName, out AWSCredentials awsCredentials))
            {
                throw new ArgumentException("AWS Credential profile missing in configured profile location");
            }

            return new AmazonS3Client(awsCredentials, RegionEndpoint.GetBySystemName(_configuration.Region));
        }

        /// <inheritdoc />
        public async Task<Uri> StoreAsync(Stream stream)
        {
            var name = Guid.NewGuid().ToString("D");
            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                BucketName = _configuration.BucketName,
                InputStream = stream,
                PartSize = AWS_PART_SIZE,
                Key = name,
                CannedACL = S3CannedACL.AuthenticatedRead
            };

            using (var transferUtility = new TransferUtility(_client.Value))
            {
                await transferUtility.UploadAsync(fileTransferUtilityRequest).ConfigureAwait(false);
            }

            return new UriBuilder(URI_SCHEME_NAME, _configuration.BucketName) { Path = name }.Uri;
        }

        /// <inheritdoc />
        public async Task<Stream> GetAsync(Uri uri)
        {
            var request = new GetObjectRequest
            {
                BucketName = uri.Host,
                Key = uri.LocalPath.Trim('/')
            };

            var response = await _client.Value.GetObjectAsync(request).ConfigureAwait(false);

            return response.ResponseStream;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(Uri uri)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = uri.Host,
                Key = uri.LocalPath.Trim('/')
            };

            await _client.Value.DeleteObjectAsync(request).ConfigureAwait(false);
        }

        public void Dispose()
        {
            if (_client.IsValueCreated)
            {
                _client.Value?.Dispose();
            }
        }
    }
}
