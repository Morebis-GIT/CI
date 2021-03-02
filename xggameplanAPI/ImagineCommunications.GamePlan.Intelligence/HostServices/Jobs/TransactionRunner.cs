using System.Threading.Tasks;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using Quartz;

namespace ImagineCommunications.GamePlan.Intelligence.HostServices.Jobs
{
    public class TransactionRunner : IJob
    {
        private readonly IGroupTransactionExecutionService _transactionExecutionService;

        public TransactionRunner(IGroupTransactionExecutionService transactionExecutionService)
        {
            _transactionExecutionService = transactionExecutionService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _transactionExecutionService.Execute();

            return Task.CompletedTask;
        }
    }
}
