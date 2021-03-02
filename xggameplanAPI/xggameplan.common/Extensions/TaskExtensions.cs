using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace xggameplan.common.Extensions
{
    public static class TaskExtensions
    {
        private static IEnumerable<Exception> ExpandAggregateException(AggregateException aggregateException)
        {
            return aggregateException.InnerExceptions.SelectMany(ex =>
            {
                if (ex is AggregateException aggregate)
                {
                    return ExpandAggregateException(aggregate);
                }

                return new[] { ex };
            });
        }

        public static async Task<T> AggregateExceptions<T>(this Task<T> task)
        {
            try
            {
                return await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                if (task.Exception is null || task.Exception.InnerExceptions.Count == 1)
                {
                    throw;
                }

                throw task.Exception;
            }
        }

        public static async Task AggregateExceptions(this Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                if (task.Exception is null || task.Exception.InnerExceptions.Count == 1)
                {
                    throw;
                }

                throw task.Exception;
            }
        }

        public static async Task AggregateExceptions<TException>(this Task task, string message)
            where TException : Exception
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch
            {
                if (task.Exception is null)
                {
                    throw;
                }

                throw (TException)Activator.CreateInstance(typeof(TException), message,
                    task.Exception.InnerExceptions.Count == 1 ? task.Exception.InnerException : task.Exception);
            }
        }

        public static async Task<IReadOnlyCollection<Exception>> WaitWithTaskExceptionGatheringAsync(this Task task, bool skipCancelledException = true)
        {
            var res = new List<Exception>();
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (OperationCanceledException ex)
            {
                if (!skipCancelledException)
                {
                    res.Add(ex);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                if (!(task.Exception is null))
                {
                    res.AddRange(ExpandAggregateException(task.Exception));
                }
                else
                {
                    throw;
                }
            }

            return res;
        }
    }
}
