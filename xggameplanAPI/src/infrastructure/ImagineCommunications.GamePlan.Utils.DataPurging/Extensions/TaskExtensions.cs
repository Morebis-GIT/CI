using System;
using System.Threading.Tasks;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Waits the with cancellation exception suppression.
        /// </summary>
        public static async Task WaitWithCancellationSuppression(this Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
