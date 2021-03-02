using System;
using System.Collections.Generic;
using FluentValidation;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared;
using ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.EventTypes.Interfaces;
using ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure.Validators;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.GroupTransaction.Infrastructure
{
    public class MockContractValidatorService : IContractValidatorService
    {
        private static Dictionary<Type, Func<IValidator>> contractValidatorMapping { get; set; }

        public MockContractValidatorService()
        {
            contractValidatorMapping = new Dictionary<Type, Func<IValidator>>();

            contractValidatorMapping.Add(typeof(IMockEventOne), () => new MockEventValidator());
            contractValidatorMapping.Add(typeof(IMockEventTwo), () => new MockEventValidator());
            contractValidatorMapping.Add(typeof(IMockEventThree), () => new MockEventValidator());
            contractValidatorMapping.Add(typeof(IMockEventFour), () => new MockEventValidator());
        }

        public void Validate<T>(T contract)
        {
            Func<IValidator> validatorConstructor = null;
            _ = contractValidatorMapping.TryGetValue(typeof(T), out validatorConstructor);
            if (validatorConstructor != null)
            {
                var validationResult = validatorConstructor().Validate(contract);
                if (!validationResult.IsValid)
                {
                    throw new ContractValidationException(validationResult.Errors);
                }
            }
        }
    }
}
