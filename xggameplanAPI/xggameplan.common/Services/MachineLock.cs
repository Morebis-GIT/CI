using System;
using System.Threading;

namespace xggameplan.common.Services
{
    /// <summary>
    /// Defines a machine wide lock
    /// </summary>
    public class MachineLock : IDisposable
    {
        private Mutex _mutex;
        private readonly bool _owned = false;

        public MachineLock(string name, TimeSpan timeToWait)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Invalid lock name");
            }

            try
            {
                _mutex = new Mutex(true, name, out _owned);
                if (!_owned)     // Wait for mutex
                {
                    _owned = _mutex.WaitOne(timeToWait);
                    if (!_owned)
                    {
                        throw new MachineLockTimeoutException(name, "Unable to obtain lock within timeout");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void Dispose()
        {
            if (_owned)
            {
                _mutex.ReleaseMutex();
            }
            _mutex = null;
        }

        public static bool IsLocked(string name)
        {
            bool isLocked = true;

            try
            {
                using (Create(name, TimeSpan.FromMilliseconds(1)))
                {
                    isLocked = false;
                }
            }
            catch (MachineLockTimeoutException)
            {
                // Ignore
            }

            return isLocked;
        }

        /// <summary>
        /// Creates machine wide lock on specified name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static IDisposable Create(string name, TimeSpan timeSpan) => new MachineLock(name, timeSpan);
    }
}
