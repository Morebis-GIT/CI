using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Shared
{
    public static class Extensions
    {
        public static void Validate<TValidator, TObject>(this IEvent ev, TValidator validator) where TValidator : AbstractValidator<TObject> where TObject : IEvent
        {
            if (validator == null || !(ev is TObject))
            {
                throw new ContractValidationException();
            }

            var validationResult = validator.Validate((TObject)ev);

            if (!validationResult.IsValid)
            {
                throw new ContractValidationException(validationResult.Errors);
            }
        }

        public static bool IsUnique<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return source.Select(selector).Distinct().Count() == source.Count();
        }
    }
}
