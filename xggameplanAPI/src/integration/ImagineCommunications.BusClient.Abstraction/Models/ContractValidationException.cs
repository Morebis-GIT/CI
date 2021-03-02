using System;

namespace ImagineCommunications.BusClient.Abstraction.Models
{
    public class ContractValidationException<T> : Exception where T : class
    {
        public T Errors { get; }

        public ContractValidationException(T errors, Func<T, string> errorProcessor) : base(errorProcessor(errors))
        {
            Errors = errors;
        }

        public ContractValidationException(string message, T errors, Func<T, string> errorProcessor) : base(message + "; " + errorProcessor(errors))
        {
            Errors = errors;
        }

        public ContractValidationException(string message, Exception innerException, T errors, Func<T, string> errorProcessor) : base(message + "; " + errorProcessor(errors), innerException)
        {
            Errors = errors;
        }
    }
}
