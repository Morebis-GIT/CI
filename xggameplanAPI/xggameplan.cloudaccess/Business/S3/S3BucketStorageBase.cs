using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using xggameplan.cloudaccess.Exceptions;
using xggameplan.cloudaccess.Interfaces;
using xggameplan.common.Extensions;
using xggameplan.Model;

namespace xggameplan.cloudaccess.Business.S3
{
    public abstract class S3BucketStorageBase : ICloudStorageV2
    {
        private readonly AwsConfiguration _configuration;
        private readonly Lazy<IAmazonS3> _s3Client;
        private bool _disposed;

        protected static long PartSize = 6291456; // 6 MB.
        protected static int DeleteBatchSize = 1000;

        protected virtual S3CannedACL UploadCannedAcl => S3CannedACL.Private;

        protected virtual string AdjustKey(string key)
        {
            return key;
        }

        protected virtual Task EnsureBucketExistsAsync()
        {
            return S3Client.EnsureBucketExistsAsync(BucketName);
        }

        protected S3BucketStorageBase(AwsConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _s3Client = new Lazy<IAmazonS3>(CreateS3Client, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public abstract string BucketName { get; }

        public virtual Task<string> GetPreSignedUrlAsync(string fullFileName)
        {
            CheckDisposed();

            if (string.IsNullOrWhiteSpace(fullFileName))
            {
                throw new ArgumentNullException(nameof(fullFileName));
            }

            var request = new GetPreSignedUrlRequest
            {
                BucketName = BucketName,
                Key = AdjustKey(fullFileName),
                Expires = DateTime.UtcNow.AddMinutes(10)
            };

            return Task.FromResult(S3Client.GetPreSignedURL(request));
        }

        public async Task<string> DownloadAsync(string fullFileName, string localFileName,
            CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            if (string.IsNullOrWhiteSpace(fullFileName))
            {
                throw new ArgumentNullException(nameof(fullFileName));
            }

            if (string.IsNullOrWhiteSpace(localFileName))
            {
                throw new ArgumentNullException(nameof(localFileName));
            }

            if (File.Exists(localFileName))
            {
                throw new S3BucketException(
                    $"'{localFileName}' local file name defined for the requested '{fullFileName}' S3 file already exists.");
            }

            try
            {
                var dir = Path.GetDirectoryName(localFileName);
                if (!(dir is null))
                {
                    _ = Directory.CreateDirectory(dir);
                }

                fullFileName = AdjustKey(fullFileName);
                var request = new GetObjectRequest { BucketName = BucketName, Key = fullFileName };

                using (var response = await S3Client.GetObjectAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    await response.WriteResponseStreamToFileAsync(localFileName, false, cancellationToken)
                        .ConfigureAwait(false);

                    return localFileName;
                }
            }
            catch (Exception ex)
            {
                throw new S3BucketException(
                    $"Exception during s3 download BucketName = {BucketName} S3 FileName = {fullFileName} Local FileName = {localFileName}",
                    ex);
            }
        }

        public async Task UploadAsync(string localFileName, string fullFileName,
            CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            if (string.IsNullOrWhiteSpace(localFileName))
            {
                throw new ArgumentNullException(nameof(localFileName));
            }

            if (string.IsNullOrWhiteSpace(fullFileName))
            {
                throw new ArgumentNullException(nameof(fullFileName));
            }

            await EnsureBucketExistsAsync().ConfigureAwait(false);

            if (await InternalFileExistsAsync(fullFileName, cancellationToken).ConfigureAwait(false))
            {
                throw new Exception("Requested file to be uploaded to S3 bucket already exists in the bucket.");
            }

            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                BucketName = BucketName,
                FilePath = localFileName,
                StorageClass = S3StorageClass.ReducedRedundancy,
                PartSize = PartSize,
                Key = AdjustKey(fullFileName),
                CannedACL = UploadCannedAcl
            };

            using (var tranUtility = new TransferUtility(S3Client))
            {
                await tranUtility.UploadAsync(fileTransferUtilityRequest, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task UploadAsync(Stream stream, string fullFileName, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (string.IsNullOrWhiteSpace(fullFileName))
            {
                throw new ArgumentNullException(nameof(fullFileName));
            }

            await EnsureBucketExistsAsync().ConfigureAwait(false);

            if (await InternalFileExistsAsync(fullFileName, cancellationToken).ConfigureAwait(false))
            {
                throw new S3BucketException("Requested file to be uploaded to S3 bucket already exists in the bucket.");
            }

            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                BucketName = BucketName,
                InputStream = stream,
                StorageClass = S3StorageClass.ReducedRedundancy,
                PartSize = PartSize,
                Key = AdjustKey(fullFileName),
                CannedACL = UploadCannedAcl
            };

            using (var tranUtility = new TransferUtility(S3Client))
            {
                await tranUtility.UploadAsync(fileTransferUtilityRequest, cancellationToken).ConfigureAwait(false);
            }
        }

        public Task<bool> FileExistsAsync(string fullFileName, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            if (string.IsNullOrWhiteSpace(fullFileName))
            {
                throw new ArgumentNullException(nameof(fullFileName));
            }

            return InternalFileExistsAsync(fullFileName, cancellationToken);
        }

        public Task DeleteAsync(string fullFileName, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            if (string.IsNullOrWhiteSpace(fullFileName))
            {
                throw new ArgumentNullException(nameof(fullFileName));
            }

            var request = new DeleteObjectRequest { BucketName = BucketName, Key = AdjustKey(fullFileName) };
            return S3Client.DeleteObjectAsync(request, cancellationToken);
        }

        public async Task DeleteAsync(IReadOnlyCollection<string> fullFileNames,
            CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            if (fullFileNames is null)
            {
                throw new ArgumentNullException(nameof(fullFileNames));
            }

            if (fullFileNames.Count == 0)
            {
                return;
            }

            var remover = CreateS3BatchRemover(cancellationToken);

            foreach (var file in fullFileNames)
            {
                _ = await remover.SendAsync(AdjustKey(file), cancellationToken).ConfigureAwait(false);
            }

            remover.Complete();

            await remover.Completion.AggregateExceptions<S3BucketException>(
                $"Deletion of file list from '{BucketName}' bucket failed.");
        }

        public Task DeleteByPrefixAsync(string filePrefix, CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            if (string.IsNullOrWhiteSpace(filePrefix))
            {
                throw new ArgumentNullException(nameof(filePrefix));
            }

            return InternalDeleteFilesByPrefixesAsync(new[] { filePrefix }, cancellationToken);
        }

        public Task DeleteByPrefixesAsync(IReadOnlyCollection<string> filePrefixes,
            CancellationToken cancellationToken = default)
        {
            CheckDisposed();

            if (filePrefixes is null)
            {
                throw new ArgumentNullException(nameof(filePrefixes));
            }

            if (filePrefixes.Count == 0)
            {
                return Task.CompletedTask;
            }

            return InternalDeleteFilesByPrefixesAsync(filePrefixes, cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_s3Client.IsValueCreated)
                {
                    _s3Client.Value.Dispose();
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);

                _disposed = true;
            }
        }

        #region Internal Methods

        protected async Task<bool> InternalFileExistsAsync(string fullFileName, CancellationToken cancellationToken = default)
        {
            fullFileName = AdjustKey(fullFileName);
            var request = new ListObjectsV2Request { BucketName = BucketName, Prefix = fullFileName };
            ListObjectsV2Response response;

            do
            {
                response = await S3Client.ListObjectsV2Async(request, cancellationToken).ConfigureAwait(false);

                if (response.S3Objects.Any(x => string.Equals(x.Key, fullFileName, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }

                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return false;
        }

        public async Task InternalDeleteFilesByPrefixesAsync(IEnumerable<string> filePrefixes,
            CancellationToken cancellationToken = default)
        {
            var remover = CreateS3BatchRemover(cancellationToken);

            var producer = new TransformManyBlock<string, string>(prefix =>
            {
                return GetFiles();

                IEnumerable<string> GetFiles()
                {
                    var request = new ListObjectsV2Request { BucketName = BucketName, Prefix = AdjustKey(prefix) };
                    ListObjectsV2Response response;

                    do
                    {
                        // in case of C# 8.0 it might be async operation with async enumerator result
                        response = S3Client.ListObjectsV2Async(request, cancellationToken).GetAwaiter().GetResult();

                        foreach (var key in response.S3Objects.Select(x => x.Key))
                        {
                            yield return key;
                        }

                        request.ContinuationToken = response.NextContinuationToken;
                    } while (response.IsTruncated);
                }
            }, new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = Math.Max(Environment.ProcessorCount / 2, 1)
            });

            _ = producer.LinkTo(remover, new DataflowLinkOptions { PropagateCompletion = true });

            foreach (var prefix in filePrefixes)
            {
                _ = await producer.SendAsync(prefix, cancellationToken).ConfigureAwait(false);
            }

            producer.Complete();

            await remover.Completion.AggregateExceptions<S3BucketException>(
                $"Deletion files by prefix from '{BucketName}' bucket failed.");
        }

        protected virtual S3BatchRemover CreateS3BatchRemover(CancellationToken cancellationToken = default)
        {
            return new S3BatchRemover(S3Client, BucketName, DeleteBatchSize, Environment.ProcessorCount,
                cancellationToken);
        }

        protected void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        #endregion Internal Methods

        #region Internal Properties

        protected IAmazonS3 S3Client => _s3Client.Value;

        #endregion Internal Properties

        private IAmazonS3 CreateS3Client()
        {
            if (!string.IsNullOrWhiteSpace(_configuration.ProfilesLocation) && File.Exists(_configuration.ProfilesLocation))
            {
                var chain = new CredentialProfileStoreChain(_configuration.ProfilesLocation);
                var profileName = !string.IsNullOrWhiteSpace(_configuration.Profile) ? _configuration.Profile : "default";
                if (!chain.TryGetAWSCredentials(profileName, out AWSCredentials awsCredentials))
                {
                    throw new ArgumentException("AWS Credential profile missing in configured profile location");
                }

                return new AmazonS3Client(awsCredentials, RegionEndpoint.GetBySystemName(_configuration.Region));
            }

            return new AmazonS3Client();
        }
    }
}
