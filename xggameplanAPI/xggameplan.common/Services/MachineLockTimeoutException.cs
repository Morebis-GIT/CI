using System;

namespace xggameplan.common.Services
{
    /// <summary>
    /// Exception indicating the machine lock could not be obtained within timeout
    /// </summary>
    public class MachineLockTimeoutException : Exception
    {
        private string _lockName { get; }

        public MachineLockTimeoutException(string lockName)
        {
            _lockName = lockName;
        }

        public MachineLockTimeoutException(string lockName, string message) : base(message)
        {
            _lockName = lockName;
        }

        public MachineLockTimeoutException(string lockName, string message, Exception inner) : base(message, inner)
        {
            _lockName = lockName;
        }

        public string LockName => _lockName;
    }
}
