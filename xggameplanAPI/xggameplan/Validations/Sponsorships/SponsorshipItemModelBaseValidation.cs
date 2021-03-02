using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public abstract class SponsorshipItemModelBaseValidation<T> : AbstractValidator<T>
    where T : SponsorshipItemModelBase
    {
        private readonly SponsorshipModelBase _sponsorshipModelBase;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IProgrammeRepository _programmeRepository;
        private IEnumerable<string> _existingSalesAreas;

        public SponsorshipItemModelBaseValidation(ISalesAreaRepository salesAreaRepository,
                                                  IProgrammeRepository programmeRepository,
                                                  IValidator<SponsoredDayPartModelBase> sponsoredDayPartModelValidation,
                                                  SponsorshipModelBase sponsorshipModelBase = null)
        {
            _sponsorshipModelBase = sponsorshipModelBase;
            _salesAreaRepository = salesAreaRepository;
            _programmeRepository = programmeRepository;

            RuleFor(model => model).NotNull()
                                   .WithMessage("A valid SponsorshipItemModelBase is required");

            When(model => model != null, () =>
            {
                RuleFor(model => model.SalesAreas)
                                      .Must(ContainSomeSalesAreas)
                                      .WithMessage("SalesAreas should contain a valid SalesArea for each item");

                When(ContainsSomeSalesAreas, () =>
                {
                    RuleFor(model => model.SalesAreas)
                                          .Must(ContainOnlyExistingSalesAreas)
                                          .WithMessage(model => CreateNonExistingSalesAreasErrorMessage(model.SalesAreas));
                });

                RuleFor(model => model.StartDate)
                                      .Must(BeGreaterThanOrEqualToToday)
                                      .WithMessage("StartDate must be greater than or equal to today");

                RuleFor(model => model.EndDate)
                                      .Must(BeGreaterThanOrEqualToToday)
                                      .WithMessage("EndDate must be greater than or equal to today");

                When(StartAndEndDatesAreValid, () =>
                {
                    RuleFor(model => model).Must(ContainAnEndDateGreaterThanOrEqualToStartDate)
                                           .WithName(model => nameof(model.EndDate))
                                           .WithMessage("EndDate must be greater than or equal to StartDate");
                });

                RuleFor(model => model.DayParts)
                                      .Must(ContainDayParts)
                                      .WithMessage("DayParts is required and should contain valid DayPart for each item");

                When(ContainsDayParts, () =>
                {
                    RuleFor(model => model.DayParts).SetCollectionValidator(sponsoredDayPartModelValidation);
                });

                When(ProgrammeNameIsRequired, () =>
                {
                    RuleFor(model => model.ProgrammeName)
                                          .NotEmpty()
                                          .WithMessage("ProgrammeName is required when RestrictionLevel Is 'Programme'");
                });
            });
        }

        private bool ContainsDayParts(SponsorshipItemModelBase model)
        {
            return ContainDayParts(model.DayParts);
        }

        private bool ContainDayParts(IEnumerable<CreateSponsoredDayPartModel> dayParts)
        {
            return dayParts?.Any() == true && !dayParts.Any(a => a == null);
        }

        private bool ProgrammeNameIsRequired(T model)
        {
            return RestrictionLevelIsProgramme(model);
        }

        private bool RestrictionLevelIsProgramme(T model)
        {
            return _sponsorshipModelBase?.RestrictionLevel == SponsorshipRestrictionLevel.Programme;
        }

        private bool ContainAnEndDateGreaterThanOrEqualToStartDate(T model)
        {
            return model.EndDate >= model.StartDate;
        }

        private bool StartAndEndDatesAreValid(T model)
        {
            return BeGreaterThanOrEqualToToday(model.StartDate) && BeGreaterThanOrEqualToToday(model.EndDate);
        }

        private bool BeGreaterThanOrEqualToToday(DateTime theDate)
        {
            return theDate >= DateTime.UtcNow.Date;
        }

        private bool ContainOnlyExistingSalesAreas(IEnumerable<string> salesAreas)
        {
            var existingSalesAreaNames = GetExistingSalesAreas();
            var salesAreaNamesToValidate = GetDistinct(salesAreas);
            var nonExistingSalesAreas = GetNonExistingSalesAreas(salesAreaNamesToValidate, existingSalesAreaNames);

            return nonExistingSalesAreas is null || !nonExistingSalesAreas.Any();
        }

        private IEnumerable<string> GetNonExistingSalesAreas(IEnumerable<string> salesAreaNamesToValidate, IEnumerable<string> existingSalesAreaNames)
        {
            return (salesAreaNamesToValidate?.Any() == true && existingSalesAreaNames?.Any() == true) ?
                   salesAreaNamesToValidate.Except(existingSalesAreaNames) :
                   salesAreaNamesToValidate;
        }

        private IEnumerable<string> GetExistingSalesAreas()
        {
            if (_existingSalesAreas == null || !_existingSalesAreas.Any(s => !string.IsNullOrWhiteSpace(s)))
            {
                _existingSalesAreas = _salesAreaRepository?.GetListOfNames();
            }

            return _existingSalesAreas;
        }

        private IEnumerable<string> GetDistinct(IEnumerable<string> items)
        {
            return items?.Where(a => !string.IsNullOrWhiteSpace(a)).Distinct();
        }

        private bool ContainSomeSalesAreas(IEnumerable<string> salesAreas)
        {
            return salesAreas?.Any() == true && !salesAreas.Any(s => string.IsNullOrWhiteSpace(s));
        }

        private bool ContainsSomeSalesAreas(T model)
        {
            return ContainSomeSalesAreas(model.SalesAreas);
        }

        private string CreateNonExistingSalesAreasErrorMessage(IEnumerable<string> salesAreaNamesToValidate)
        {
            var nonExistingSalesAreas = GetNonExistingSalesAreas(salesAreaNamesToValidate, GetExistingSalesAreas());

            return (nonExistingSalesAreas?.Any() == true) ?
                   $"SalesArea(s): [ {String.Join(", ", nonExistingSalesAreas)} ] do not exist" :
                   "SalesArea(s) supplied do not exist";
        }
    }
}
