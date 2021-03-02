using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ImagineCommunications.GamePlan.Utils.DataPurging.Options;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Infrastructure
{
    public class BatchExecuteTargetBlock<TInput> : BatchExecuteTargetBlock<TInput, ConcurrencyOptions>
    {
        public BatchExecuteTargetBlock(Func<TInput[], Task> action, ConcurrencyOptions options,
            CancellationToken cancellationToken = default) : base(action, options, cancellationToken)
        {
        }
    }

    public class BatchExecuteTargetBlock<TInput, TOptions> : ITargetBlock<TInput>
        where TOptions : ConcurrencyOptions
    {
        private readonly ITargetBlock<TInput[]> _targetBlock;
        private readonly IPropagatorBlock<TInput, TInput[]> _propagatorBlock;

        public BatchExecuteTargetBlock(Func<TInput[], Task> action, TOptions options, CancellationToken cancellationToken = default)
        {
            _targetBlock = new ActionBlock<TInput[]>(action,
                new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = options.DegreeOfParallelism ?? Environment.ProcessorCount,
                    CancellationToken = cancellationToken
                });

            _propagatorBlock = new BatchBlock<TInput>(Math.Max(options.ItemsPerTask, 1),
                new GroupingDataflowBlockOptions { CancellationToken = cancellationToken });
            _ = _propagatorBlock.LinkTo(_targetBlock, new DataflowLinkOptions { PropagateCompletion = true });
        }

        public void Complete() => _propagatorBlock.Complete();

        public Task Completion => _targetBlock.Completion;

        void IDataflowBlock.Fault(Exception exception) => _propagatorBlock.Fault(exception);

        DataflowMessageStatus ITargetBlock<TInput>.OfferMessage(DataflowMessageHeader messageHeader,
            TInput messageValue, ISourceBlock<TInput> source,
            bool consumeToAccept) =>
            _propagatorBlock.OfferMessage(messageHeader, messageValue, source, consumeToAccept);
    }
}
