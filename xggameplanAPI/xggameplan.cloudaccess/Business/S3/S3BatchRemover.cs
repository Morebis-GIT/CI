using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Amazon.S3;
using Amazon.S3.Model;

namespace xggameplan.cloudaccess.Business.S3
{
    /// <summary>
    /// Represents dataflow target block for removing files in S3 bucket
    /// with batching.
    /// </summary>
    /// <seealso cref="System.Threading.Tasks.Dataflow.ITargetBlock{System.String}" />
    public class S3BatchRemover : ITargetBlock<string>
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly CancellationToken _cancellationToken;
        private readonly ITargetBlock<string[]> _targetBlock;
        private readonly IPropagatorBlock<string, string[]> _propagatorBlock;

        private async Task RemoveFiles(string[] files)
        {
            var request = new DeleteObjectsRequest { BucketName = _bucketName };
            request.Objects.AddRange(files.Select(f => new KeyVersion { Key = f }));

            try
            {
                _ = await _s3Client.DeleteObjectsAsync(request, _cancellationToken).ConfigureAwait(false);
            }
            catch (DeleteObjectsException)
            {
                //suppress it, we are not interested at the moment why some files could not be removed (permissions or something else)
            }
        }

        public S3BatchRemover(IAmazonS3 s3Client, string bucketName, int batchSize,
            int maxDegreeOfParallelism, CancellationToken cancellationToken = default)
        {
            _s3Client = s3Client ?? throw new ArgumentNullException(nameof(s3Client));
            _bucketName = bucketName ?? throw new ArgumentNullException(nameof(bucketName));
            _cancellationToken = cancellationToken;

            _targetBlock = new ActionBlock<string[]>(RemoveFiles,
                new ExecutionDataflowBlockOptions
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = maxDegreeOfParallelism
                });

            _propagatorBlock = new BatchBlock<string>(batchSize,
                new GroupingDataflowBlockOptions
                {
                    CancellationToken = cancellationToken,
                    BoundedCapacity = batchSize * Environment.ProcessorCount
                });

            _ = _propagatorBlock.LinkTo(_targetBlock, new DataflowLinkOptions { PropagateCompletion = true });
        }

        public void Complete()
        {
            _propagatorBlock.Complete();
        }

        public Task Completion => _targetBlock.Completion;

        DataflowMessageStatus ITargetBlock<string>.OfferMessage(DataflowMessageHeader messageHeader, string messageValue,
            ISourceBlock<string> source,
            bool consumeToAccept) =>
            _propagatorBlock.OfferMessage(messageHeader, messageValue, source, consumeToAccept);

        void IDataflowBlock.Fault(Exception exception) => _propagatorBlock.Fault(exception);
    }
}
