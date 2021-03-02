using System;
using System.Diagnostics;

namespace xggameplan.common.Helpers
{
    public static class StopwatchHelper
    {
        public static Stopwatch StopwatchAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var watch = Stopwatch.StartNew();
            try
            {
                action();
            }
            finally
            {
                watch.Stop();
            }

            return watch;
        }

        public static TResult StopwatchFunc<TResult>(Func<TResult> func, out Stopwatch stopwatch)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            stopwatch = Stopwatch.StartNew();
            try
            {
                return func();
            }
            finally
            {
                stopwatch.Stop();
            }
        }
    }
}
