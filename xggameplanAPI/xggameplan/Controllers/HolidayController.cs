using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    [RoutePrefix("Holidays")]
    public class HolidayController : ApiController
    {
        private readonly ISalesAreaRepository _salesAreaRepository;

        public HolidayController(ISalesAreaRepository salesAreaRepository)
        {
            _salesAreaRepository = salesAreaRepository;
        }

        /// <summary>
        /// Add /Update Holidays with sales area
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Holidays")]
        public IHttpActionResult Post([FromBody] CreateHoliday command)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            List<SalesArea> salesAreas;
            if (command.SalesAreaNames == null || !command.SalesAreaNames.Any())
            {
                salesAreas = _salesAreaRepository.GetAll().ToList(); // Apply to all
            }
            else
            {
                salesAreas = _salesAreaRepository.FindByNames(command.SalesAreaNames);
            }

            if (salesAreas == null || !salesAreas.Any())
            {
                return NotFound();
            }

            ValidateHolidays(command.HolidayDateRanges);
            salesAreas.ForEach(salesArea =>
            {
                switch (command.HolidayType)
                {
                    case HolidayType.PublicHoliday:
                        salesArea.PublicHolidays = Add(salesArea.PublicHolidays, command.HolidayDateRanges);
                        break;

                    case HolidayType.SchoolHoliday:
                        salesArea.SchoolHolidays = Add(salesArea.SchoolHolidays, command.HolidayDateRanges);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(command.HolidayType), command.HolidayType,
                            "Invalid holiday type");
                }
            });
            _salesAreaRepository.Update(salesAreas);
            return Ok();
        }

        private static void ValidateHolidays(List<DateRange> dateRanges)
        {
            if (dateRanges != null && dateRanges.Any())
            {
                dateRanges.ForEach(d =>
                {
                    if (d.End.Date < d.Start.Date)
                    {
                        throw new Exception("holiday end date (" + d.End.Date +
                                            ") should be greater than or equal to holiday start date (" + d.Start.Date +
                                            ")");
                    }
                }
            );
            }
        }

        /// <summary>
        /// Delete Holidays from sales area
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Holidays")]
        public IHttpActionResult Delete(HolidayType holidayType, string salesAreaName)
        {
            if (string.IsNullOrEmpty(salesAreaName) || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            var salesArea = _salesAreaRepository.FindByName(salesAreaName);
            if (salesArea == null)
            {
                return NotFound();
            }

            switch (holidayType)
            {
                case HolidayType.PublicHoliday:
                    salesArea.PublicHolidays.Clear();
                    break;

                case HolidayType.SchoolHoliday:
                    salesArea.SchoolHolidays.Clear();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(holidayType), holidayType, null);
            }
            _salesAreaRepository.Update(salesArea);
            return Ok();
        }

        /// <summary>
        /// Get Holidays from sales area
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Holidays")]
        public IHttpActionResult Get(string salesAreaName, HolidayType? holidayType = null)
        {
            if (string.IsNullOrEmpty(salesAreaName) || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            var salesArea = _salesAreaRepository.FindByName(salesAreaName);
            if (salesArea == null)
            {
                return NotFound();
            }

            switch (holidayType)
            {
                case HolidayType.PublicHoliday:
                    return Ok(new HolidayModel()
                    {
                        HolidayType = HolidayType.PublicHoliday,
                        HolidayDateRanges = salesArea.PublicHolidays
                    });

                case HolidayType.SchoolHoliday:
                    return Ok(new HolidayModel()
                    {
                        HolidayType = HolidayType.SchoolHoliday,
                        HolidayDateRanges = salesArea.SchoolHolidays
                    });

                case null:
                    return Ok(new List<HolidayModel>()
                    {
                        new HolidayModel()
                        {
                            HolidayType = HolidayType.PublicHoliday,
                            HolidayDateRanges = salesArea.PublicHolidays
                        },
                        new HolidayModel()
                        {
                            HolidayType = HolidayType.SchoolHoliday,
                            HolidayDateRanges = salesArea.SchoolHolidays
                        }
                    });

                default:
                    throw new ArgumentOutOfRangeException(nameof(holidayType), holidayType, null);
            }
        }

        /// <summary>
        /// Add the new dates if list is empty or Append the new list with
        /// existing list
        /// </summary>
        /// <param name="existingDates"></param>
        /// <param name="newDateRanges"></param>
        /// <returns></returns>
        private List<DateRange> Add(List<DateRange> existingDates, List<DateRange> newDateRanges)
        {
            existingDates?.Clear();
            var newDates = newDateRanges?.Select(d =>
            {
                d.Start = d.Start.Date;
                d.End = d.End.Date;
                return d;
            })?.ToList(); // Trim time value in date
            existingDates = newDates;
            return existingDates?.ToList();
        }
    }
}
