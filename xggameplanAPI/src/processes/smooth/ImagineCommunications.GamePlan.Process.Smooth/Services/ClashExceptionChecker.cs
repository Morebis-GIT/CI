using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Clash exception checker
    /// </summary>
    public class ClashExceptionChecker
    {
        private readonly IImmutableList<ClashException> _clashExceptions;
        private readonly IImmutableList<Clash> _clashes;
        private readonly IImmutableList<Product> _products;
        private readonly bool _enabled;

        public ClashExceptionChecker(
            IImmutableList<ClashException> clashExceptions,
            IImmutableList<Product> products,
            IImmutableList<Clash> clashes,
            bool enabled
            )
        {
            _clashExceptions = clashExceptions;
            _products = products;
            _clashes = clashes;
            _enabled = enabled;
        }

        /// <summary>
        /// Gets all restrictions that the spot conflicts with.
        /// </summary>
        public List<CheckClashExceptionResult> CheckClashExceptions(
            SmoothBreak smoothBreak,
            Spot spot)
        {
            var results = new List<CheckClashExceptionResult>();

            if (!_enabled)
            {
                return results;
            }

            foreach (ClashException clashException in _clashExceptions)
            {
                var action = CheckClashException(smoothBreak, spot, clashException);

                if (action == CheckClashExceptionActions.NoAction)
                {
                    continue;
                }

                results.Add(new CheckClashExceptionResult(clashException));
            }

            return results;
        }

        /// <summary>
        /// Checks the clash exception
        /// </summary>
        /// <param name="smoothBreak"></param>
        /// <param name="spotToPlace"></param>
        /// <param name="clashException"></param>
        /// <returns></returns>
        private CheckClashExceptionActions CheckClashException(
            SmoothBreak smoothBreak,
            Spot spotToPlace,
            ClashException clashException)
        {
            // Check date & time
            Break theBreak = smoothBreak.TheBreak;

            if (smoothBreak.TheBreak.ScheduledDate.Date < clashException.StartDate.Date)
            {
                // Outside of date range
                return CheckClashExceptionActions.NoAction;
            }

            if (clashException.EndDate != null && theBreak.ScheduledDate.Date > clashException.EndDate)
            {
                // Outside of date range
                return CheckClashExceptionActions.NoAction;
            }

            // Assume that to time & DOWs means any time Check if outside of
            // time range
            if (clashException.TimeAndDows?.Count > 0)
            {
                var daysOfWeek = new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };
                bool isWithinTimeRange = false;
                foreach (var timeAndDow in clashException.TimeAndDows)
                {
                    if (timeAndDow.DaysOfWeek.Substring(daysOfWeek.IndexOf(theBreak.ScheduledDate.DayOfWeek), 1) == "1")
                    {
                        if ((timeAndDow.StartTime == null || theBreak.ScheduledDate.TimeOfDay >= timeAndDow.StartTime) &&
                            (timeAndDow.EndTime == null || theBreak.ScheduledDate.TimeOfDay <= timeAndDow.EndTime))
                        {
                            isWithinTimeRange = true;
                            break;
                        }
                    }
                }

                if (!isWithinTimeRange)
                {
                    return CheckClashExceptionActions.NoAction;
                }
            }

            // Get From products
            var fromProducts = new List<Product>();
            switch (clashException.FromType)
            {
                case ClashExceptionType.Advertiser:
                    fromProducts.AddRange(_products.Where(p => p.AdvertiserIdentifier == clashException.FromValue));
                    break;
                case ClashExceptionType.Clash:
                    var clashExternalRefs = _clashes.Where(c => c.Externalref == clashException.FromValue || c.ParentExternalidentifier == clashException.FromValue).Select(c => c.Externalref).Distinct().ToList();
                    fromProducts.AddRange(_products.Where(p => clashExternalRefs.Contains(p.ClashCode)));
                    break;
                case ClashExceptionType.Product:
                    var productExternalRefs = _products.Where(p => p.Externalidentifier == clashException.FromValue || p.ParentExternalidentifier == clashException.FromValue).Select(p => p.Externalidentifier).Distinct().ToList();
                    fromProducts.AddRange(_products.Where(p => productExternalRefs.Contains(p.Externalidentifier)));
                    break;
            }

            // Get To products
            var toProducts = new List<Product>();

            var clashExceptionToValue = clashException.ToValue;

            switch (clashException.ToType)
            {
                case ClashExceptionType.Advertiser:
                    toProducts.AddRange(
                        _products.Where(p => p.AdvertiserIdentifier == clashExceptionToValue)
                        );

                    break;

                case ClashExceptionType.Clash:
                    var clashExternalRefs = _clashes
                        .Where(c => c.Externalref == clashExceptionToValue || c.ParentExternalidentifier == clashExceptionToValue)
                        .Select(c => c.Externalref)
                        .ToList();

                    toProducts.AddRange(
                        _products.Where(p => clashExternalRefs.Contains(p.ClashCode))
                        );

                    break;

                case ClashExceptionType.Product:
                    var productExternalRefs = _products
                        .Where(p => p.Externalidentifier == clashExceptionToValue || p.ParentExternalidentifier == clashExceptionToValue)
                        .Select(p => p.Externalidentifier)
                        .Distinct()
                        .ToList();

                    toProducts.AddRange(
                        _products.Where(p => productExternalRefs.Contains(p.Externalidentifier))
                        );

                    break;
            }

            // Check include/exclude for spots
            bool hasIncludes = false;
            bool hasExcludes = false;
            foreach (var breakSmoothSpot in smoothBreak.SmoothSpots)
            {
                // Check From spot -> To break spot to place
                var spotToPlaceFromProducts = fromProducts.Where(p => p.Externalidentifier == spotToPlace.Product);
                var spotInBreakToProducts = toProducts.Where(p => p.Externalidentifier == breakSmoothSpot.Spot.Product);

                if (spotToPlaceFromProducts.Any() && spotInBreakToProducts.Any())
                {
                    switch (clashException.IncludeOrExclude)
                    {
                        case IncludeOrExclude.I:
                            hasIncludes = true;
                            break;
                        case IncludeOrExclude.E:
                            hasExcludes = true;
                            break;
                    }
                }

                // Check From break spots -> To spot to place
                var spotToPlaceToProducts = toProducts.Where(p => p.Externalidentifier == spotToPlace.Product);
                var spotsInBreakFromProducts = fromProducts.Where(p => p.Externalidentifier == breakSmoothSpot.Spot.Product);
                if (spotToPlaceToProducts.Any() && spotsInBreakFromProducts.Any())
                {
                    switch (clashException.IncludeOrExclude)
                    {
                        case IncludeOrExclude.I:
                            hasIncludes = true;
                            break;
                        case IncludeOrExclude.E:
                            hasExcludes = true;
                            break;
                    }
                }

                if (hasIncludes || (hasIncludes && hasExcludes))
                {
                    // No point in checking further spots
                    break;
                }
            }

            if (hasIncludes)
            {
                return CheckClashExceptionActions.Includes;
            }

            if (hasExcludes)
            {
                return CheckClashExceptionActions.Excludes;
            }

            return CheckClashExceptionActions.NoAction;
        }
    }
}
