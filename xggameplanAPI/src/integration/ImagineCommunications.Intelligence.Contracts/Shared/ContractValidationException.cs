using System;
using System.Collections.Generic;
using FluentValidation.Results;
using System.Linq;
using ImagineCommunications.BusClient.Abstraction.Models;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Shared
{
    public class ContractValidationException : ContractValidationException<IList<ValidationFailure>>
    {
        public ContractValidationException(IList<ValidationFailure> errors = null) : base(errors, ConcatenateErrorMessages) { }

        public ContractValidationException(string message, IList<ValidationFailure> errors = null) : base(message, errors, ConcatenateErrorMessages) { }

        public ContractValidationException(string message, Exception innerException, IList<ValidationFailure> errors = null) : base(message, innerException, errors, ConcatenateErrorMessages) { }
        protected static string ConcatenateErrorMessages(IList<ValidationFailure> errors)
        {
            return errors != null
                ? string.Join("; ", errors.Select(e => "PropertyName: " + e.PropertyName + "- Error:" + e.ErrorMessage))
                : "";
        }
    }
}
