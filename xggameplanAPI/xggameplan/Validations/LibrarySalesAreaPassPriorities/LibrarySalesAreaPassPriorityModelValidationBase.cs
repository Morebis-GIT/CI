using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Models;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public abstract class LibrarySalesAreaPassPriorityModelValidationBase<T> : AbstractValidator<T>
                    where T : LibrarySalesAreaPassPriorityModelBase
    {
        protected readonly ILibrarySalesAreaPassPrioritiesRepository _librarySalesAreaPassPrioritiesRepository;
        protected readonly ISalesAreaRepository _salesAreaRepository;
        protected readonly string NameAlreadyExistsMessage = "Library Name already exists, A unique Name is required.";
        private readonly string _acceptedPriorityValues = $"Accepted values: [ {string.Join(", ", Enum.GetNames(typeof(SalesAreaPriorityType)))} ]";
        private readonly Regex _validDaysOfWeekRegEx = new Regex("^(?!0{7})[0-1]{7}$",RegexOptions.Compiled);
        private readonly Regex _validTimeRegEx = new Regex("^(([0]{1}[0-9]{1})|([1]{1}[0-9]{1})|([2]{1}[0-3]{1})):([0-5]{1}[0-9]{1}:[0-5]{1}[0-9]{1})$", RegexOptions.Compiled);
        private readonly string _validTimeMessage = "Correct format is: 'hh:mm:ss' in 24 Hour clock. Example: '23:08:59' ";
        private List<string> _nonExistingSalesAreas;
        private List<string> _duplicateSalesAreas;
        private List<string> _invalidPriorities;

        public LibrarySalesAreaPassPriorityModelValidationBase(ILibrarySalesAreaPassPrioritiesRepository librarySalesAreaPassPrioritiesRepository,
                                                               ISalesAreaRepository salesAreaRepository)
        {
            _librarySalesAreaPassPrioritiesRepository = librarySalesAreaPassPrioritiesRepository;
            _salesAreaRepository = salesAreaRepository;

            RuleFor(a => a.Name).Must(ContainValidName).WithMessage("Name is required.");

            RuleFor(a => a.DaysOfWeek).NotNull().NotEmpty().Matches(_validDaysOfWeekRegEx)
            .WithMessage("DaysOfWeek is incorrect, Correct Format is: '1111111' which represents Monday to Sunday in 7 digits,use 1 to include the day or 0 to exclude the day");

            RuleFor(a => a.SalesAreaPriorities).Must(ContainSomeSalesAreaPriorities).WithMessage("SalesAreaPriorities is required.");

            When(ContainSomeSalesAreaPriorities, () =>
            {
                RuleFor(a => a.SalesAreaPriorities).Must(ContainOnlyValidSalesAreas)
                                                     .WithMessage("SalesAreaPriorities should contain a valid 'SalesArea' for each item.")
                                                   .Must(NotContainDuplicateSalesAreas)
                                                     .WithMessage("{0}", (c) => CreateDuplicateSalesAreasErrorMessage())
                                                   .Must(ContainOnlyExistingSalesAreas)
                                                     .WithMessage("{0}", (c) => CreateNonExistingSalesAreasErrorMessage())
                                                   .Must(ContainOnlyValidPriority)
                                                     .WithMessage("{0}", (c) => CreateInvalidPriorityErrorMessage());
            });

            When(IsNotAllDay, () =>
            {
                RuleFor(a => a.StartTime).Must(ContainValidTime).WithMessage($"StartTime is invalid, {_validTimeMessage}");
                RuleFor(a => a.EndTime).Must(ContainValidTime).WithMessage($"EndTime is invalid, {_validTimeMessage}");
            });
        }

        protected bool ContainsValidName(T model)
        {
            return ContainValidName(model.Name);
        }

        protected bool ContainValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name);
        }

        protected bool ContainSomeSalesAreaPriorities(T model)
        {
            return ContainSomeSalesAreaPriorities(model.SalesAreaPriorities);
        }

        protected bool ContainSomeSalesAreaPriorities(List<SalesAreaPriorityModel> salesAreaPriorities)
        {
            return salesAreaPriorities != null && salesAreaPriorities.Count > 0;
        }

        protected bool ContainOnlyValidSalesAreas(List<SalesAreaPriorityModel> salesAreaPriorities)
        {
            return !salesAreaPriorities.Any(a => string.IsNullOrWhiteSpace(a.SalesArea));
        }

        protected bool NotContainDuplicateSalesAreas(List<SalesAreaPriorityModel> salesAreaPriorities)
        {
            _duplicateSalesAreas = salesAreaPriorities.GroupBy(a => a.SalesArea).Where(g => g.Count() > 1).Select(a => a.Key).ToList();
            return !_duplicateSalesAreas.Any();
        }

        protected bool ContainValidTime(string time)
        {
            return !string.IsNullOrWhiteSpace(time) && _validTimeRegEx.IsMatch(time.Trim());
        }

        protected bool IsNotAllDay(T model)
        {
            return !string.IsNullOrWhiteSpace(model.StartTime) || !string.IsNullOrWhiteSpace(model.EndTime);
        }

        protected bool ContainOnlyExistingSalesAreas(List<SalesAreaPriorityModel> salesAreaPriorities)
        {
            _nonExistingSalesAreas = null;
            if (salesAreaPriorities != null && salesAreaPriorities.Count > 0)
            {
                var existingSalesAreaNames = _salesAreaRepository.GetListOfNames();
                var salesAreaNamesToValidate = salesAreaPriorities.Where(a => !string.IsNullOrWhiteSpace(a.SalesArea))
                                                                  .Select(a => a.SalesArea.Trim()).Distinct().ToList();

                if (existingSalesAreaNames != null && existingSalesAreaNames.Count > 0)
                {
                    _nonExistingSalesAreas = salesAreaNamesToValidate.Except(existingSalesAreaNames).ToList();
                }
            }

            return _nonExistingSalesAreas == null || _nonExistingSalesAreas.Count == 0;
        }

        protected bool ContainOnlyValidPriority(List<SalesAreaPriorityModel> salesAreaPriorities)
        {
            _invalidPriorities = null;
            if (salesAreaPriorities != null && salesAreaPriorities.Count > 0)
            {
                var typeOfPriority = typeof(SalesAreaPriorityType);
                _invalidPriorities = salesAreaPriorities.Where(a => !Enum.IsDefined(typeOfPriority,a.Priority))
                                                        .Select(a => a.Priority.ToString()).Distinct().ToList();
            }

            return _invalidPriorities == null || _invalidPriorities.Count == 0;
        }

        private string CreateDuplicateSalesAreasErrorMessage()
        {
                return (_duplicateSalesAreas != null && _duplicateSalesAreas.Count > 0) ?
                       $"Duplicate SalesArea(s): [ {string.Join(", ", _duplicateSalesAreas)} ] found." :
                       string.Empty;
        }

        private string CreateNonExistingSalesAreasErrorMessage()
        {
                return (_nonExistingSalesAreas != null && _nonExistingSalesAreas.Count > 0) ?
                       $"SalesArea(s): [ {string.Join(", ", _nonExistingSalesAreas)} ] do not exist." :
                       string.Empty;
        }

        private string CreateInvalidPriorityErrorMessage()
        {
            return (_invalidPriorities != null && _invalidPriorities.Count > 0) ?
                   $"Priority: [ {string.Join(", ", _invalidPriorities)} ] is invalid. {_acceptedPriorityValues}" :
                   string.Empty;
        }
    }
}

