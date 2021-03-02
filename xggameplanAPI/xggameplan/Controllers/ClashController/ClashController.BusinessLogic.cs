using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    public partial class ClashController
    {
        #region Delete Clash by External Reference

        public (bool Success, string Message) DeleteClashByExternalId(string externalReference)
        {
            var clash = _clashRepository.FindByExternal(externalReference).FirstOrDefault();

            if (clash is null)
            {
                return Failure($"Clash with external reference '{externalReference}' was not found");
            }

            return ValidateClashAndDelete(clash);
        }

        public (bool Success, string Message) DeleteClashById(Guid id)
        {
            var clash = _clashRepository.Get(id);

            if (clash is null)
            {
                return Failure($"Clash with uid '{id}' was not found");
            }

            return ValidateClashAndDelete(clash);
        }

        private (bool Success, string Message) ValidateClashAndDelete(Clash clash)
        {
            var validationResult = ValidateClashBeforeDelete(clash);

            if (validationResult)
            {
                _clashRepository.Delete(clash.Uid);
                _clashRepository.SaveChanges();
                return Success();
            }

            return Failure("Clash cannot be deleted – in use");
        }

        private bool ValidateClashBeforeDelete(Clash clash)
        {
            return _clashValidator.ValidateClashHasNoChildClashes(clash.Externalref).Successful &&
                   _clashValidator.ValidateClashIsNotLinkedToActiveProduct(clash.Externalref).Successful;
        }

        #endregion

        #region Clash Update

        public (bool Success, string Message) ValidateAndSaveClash(Clash clash, UpdateClashModel model, bool applyGlobally)
        {
            if (clash is null)
            {
                throw new ArgumentException("Clash, which is being updated, cannot be null", nameof(clash));
            }

            if (applyGlobally)
            {
                clash.Differences = new List<ClashDifference>(0);
            }

            var clashToValidate = (Clash)clash.Clone();

            clashToValidate = UpdateClash(clashToValidate, model, applyGlobally);

            var allClashes = _clashRepository.GetAll();

            var validationResult = ValidateClashDifferencesForSave(clashToValidate, allClashes);
            if (!validationResult.Successful)
            {
                return Failure(validationResult.Message);
            }

            clash = UpdateClash(clash, model, applyGlobally); ;

            // validate clash new values
            //Removing current clash from all clashes while validate
            Validation(new List<Clash> { clash }, allClashes?.Where(c => c.Uid != clash.Uid).ToList());

            _clashRepository.Add(clash);
            _clashRepository.SaveChanges();

            return Success();
        }

        private Clash UpdateClash(Clash clash, UpdateClashModel model, bool applyGlobally)
        {
            clash.Description = model.Description;
            clash.ParentExternalidentifier = model.ParentExternalidentifier;
            clash.DefaultPeakExposureCount = model.DefaultPeakExposureCount;
            clash.DefaultOffPeakExposureCount = model.DefaultOffPeakExposureCount;

            if (!applyGlobally)
            {
                clash.Differences = model.Differences;
            }

            return clash;
        }

        #endregion

        private CustomValidationResult ValidateClashDifferencesForSave(Clash clash, IEnumerable<Clash> allClashes)
        {
            var acceptableDaysOfTheWeek = new List<string>()
            {
                "MONDAY", "TUESDAY", "WEDNESDAY", "THURSDAY", "FRIDAY", "SATURDAY", "SUNDAY",
                "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"
            };

            if (clash.Differences is null)
            {
                return CustomValidationResult.Failed("Clash differences can not be null");
            }

            var defaultEndTime = new TimeSpan(5,59,59);

            var clashDifferences = clash.Differences;

            if (clashDifferences.Count == 0)
            {
                return CustomValidationResult.Success();
            }

            foreach (var difference in clashDifferences)
            {
                if (difference.PeakExposureCount == null && difference.OffPeakExposureCount == null)
                {
                    return CustomValidationResult.Failed("At least one exposure count should be specified");
                }

                if (difference.StartDate.HasValue && difference.EndDate.HasValue && difference.StartDate >= difference.EndDate)
                {
                    return CustomValidationResult.Failed("Clash difference start date must be before the end date");
                }

                var timeAndDow = difference.TimeAndDow;
                if (timeAndDow.StartTime.HasValue && timeAndDow.EndTime.HasValue)
                {
                    var startTime = timeAndDow.StartTime.Value;
                    var endTime = timeAndDow.EndTime.Value;
                    endTime = endTime > defaultEndTime ? endTime : endTime.Add(TimeSpan.FromHours(24));

                    if (startTime >= endTime)
                    {
                        return CustomValidationResult.Failed("Clash difference start time must be before the end time");
                    }
                }

                if (timeAndDow.DaysOfWeek is null || !timeAndDow.DaysOfWeek.Any())
                {
                    return CustomValidationResult.Failed("Clash difference should contain minimum one day of the week");
                }

                if (timeAndDow.DaysOfWeek.Count > 7)
                {
                    return CustomValidationResult.Failed("Clash difference should contain maximum 7 days of the week");
                }

                var checkList = new List<string>();
                foreach (var dayOfWeek in timeAndDow.DaysOfWeek)
                {
                    var day = dayOfWeek.ToUpper(CultureInfo.InvariantCulture);
                    if (acceptableDaysOfTheWeek.Contains(day))
                    {
                        var index = acceptableDaysOfTheWeek.IndexOf(day);
                        day = acceptableDaysOfTheWeek[index < 7 ? index : index - 7];
                        if (!checkList.Contains(day))
                        {
                            checkList.Add(day);
                        }
                        else
                        {
                            return CustomValidationResult.Failed("Clash difference should not contain a duplicate of days of the week");
                        }
                    }
                    else
                    {
                        return CustomValidationResult.Failed("Clash difference should contain only acceptable days of the week");
                    }
                }
            }

            var validationResult = _clashValidator.ValidateTimeRanges(clashDifferences);

            if (!validationResult.Successful)
            {
                return validationResult;
            }

            validationResult = _clashValidator.ValidateExposureCountDifferences(clash, allClashes);

            if (!validationResult.Successful)
            {
                return validationResult;
            }

            return CustomValidationResult.Success();
        }

        private static (bool Success, string Message) Success() => (true, string.Empty);
        private static (bool Success, string Message) Failure(string message) => (false, message);
    }
}
