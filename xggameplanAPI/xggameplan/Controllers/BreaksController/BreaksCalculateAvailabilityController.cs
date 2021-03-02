using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using xggameplan.Extensions;
using xggameplan.Filters;
using xggameplan.Model;

namespace xggameplan.Controllers
{
    public partial class BreaksController : ApiController
    {
        /// <summary>Calculates the break availabilities for a given date range
        /// and set of sales areas.</summary>
        /// <param name="dateRangeStart">The date range start.</param>
        /// <param name="dateRangeEnd">The date range end.</param>
        /// <param name="salesAreaNames">The sales area names.</param>
        /// <returns></returns>
        [Route("calculate-availabilities")]
        [AuthorizeRequest("Breaks")]
        [HttpPatch]
        public IHttpActionResult CalculateBreakAvailabilities(
            [FromBody] CalculateBreakAvailabilityModel values
            )
        {
            foreach (var message in ValidateParams(values))
            {
                ModelState.AddModelError(message.Key, message.Value);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var salesAreas = _salesAreaRepository.FindByNames(values.SalesAreaNames);
            if (salesAreas.Count == 0)
            {
                return NotFound();
            }

            _recalculateBreakAvailabilityService.Execute(
                (values.DateRangeStart, values.DateRangeEnd),
                salesAreas
                );

            return Ok();

            // Local functions
            static Dictionary<string, string> ValidateParams(CalculateBreakAvailabilityModel values)
            {
                var dateRangeValidationResults = ValidateDateRange(
                    values.DateRangeStart,
                    values.DateRangeEnd);

                var salesAreaValidationResults = ValidateSalesAreaNames(
                    values.SalesAreaNames);

                var messages = new Dictionary<string, string>();

                foreach (var item in dateRangeValidationResults)
                {
                    messages.AddDistinct(item);
                }

                foreach (var item in salesAreaValidationResults)
                {
                    messages.AddDistinct(item);
                }

                return messages;
            }
        }

        private static Dictionary<string, string> ValidateDateRange(
            DateTime dateRangeStart,
            DateTime dateRangeEnd)
        {
            var messages = new Dictionary<string, string>();

            if (dateRangeStart == default)
            {
                messages.Add(nameof(dateRangeStart), "Date range start is missing");
            }

            if (dateRangeEnd == default)
            {
                messages.Add(nameof(dateRangeEnd), "Date range end is missing");
            }

            if (dateRangeStart > dateRangeEnd)
            {
                messages.Add("Date range", "Date range start must be before or equal to the date range end");
            }

            return messages;
        }

        private static Dictionary<string, string> ValidateSalesAreaNames(
            IReadOnlyCollection<string> salesAreaNames
            )
        {
            var messages = new Dictionary<string, string>();

            if (salesAreaNames is null || !salesAreaNames.Any())
            {
                messages.Add(nameof(salesAreaNames), "Sales area names are missing");
            }

            return messages;
        }
    }
}
