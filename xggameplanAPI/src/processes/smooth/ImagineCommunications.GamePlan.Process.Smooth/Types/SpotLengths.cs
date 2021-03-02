using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;

namespace ImagineCommunications.GamePlan.Process.Smooth.Types
{
    internal static class SpotLengths
    {
        /// <summary>
        /// <para>For the given spot get lengths of other spots to try and place at the same time.</para>
        /// <para>We try and place spots that total 30 secs (or multiples of 30 sec if default spot > 30
        /// secs) so that we leave the break availability as 30 sec multiples.</para>
        /// </summary>
        /// <param name="spot"></param>
        internal static ref readonly IReadOnlyList<TimeSpan[]> GetSpotLengthsCombinationsToFind(Spot spot)
        {
            switch ((int)spot.SpotLength.ToTimeSpan().TotalSeconds)
            {
                case 5:     // 30 sec total
                    return ref _spotLengths5SecondsForTotalOf30Seconds;

                case 10:    // 30 sec total
                    return ref _spotLengths10SecondsForTotalOf30Seconds;

                case 15:    // 30 sec total
                    return ref _spotLengths15SecondsForTotalOf30Seconds;

                case 20:    // 30 sec total
                    return ref _spotLengths20SecondsForTotalOf30Seconds;

                case 25:    // 30 sec total
                    return ref _spotLengths25SecondsForTotalOf30Seconds;

                case 30:    // 30 sec total
                    goto default;

                case 35:    // 60 sec total
                    return ref _spotLengths35SecondsForTotalOf60Seconds;

                case 40:    // 60 sec total
                    return ref _spotLengths40SecondsForTotalOf60Seconds;

                case 45:    // 60 sec total
                    return ref _spotLengths45SecondsForTotalOf60Seconds;

                case 50:    // 60 sec total
                    return ref _spotLengths50SecondsForTotalOf60Seconds;

                case 55:    // 60 sec total
                    return ref _spotLengths55SecondsForTotalOf60Seconds;

                default:
                    return ref _empty;
            }
        }

        private static readonly IReadOnlyList<TimeSpan[]> _empty = new List<TimeSpan[]>(0);

        private static readonly IReadOnlyList<TimeSpan[]> _spotLengths5SecondsForTotalOf30Seconds =
            new List<TimeSpan[]>()
            {
                new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15) },
                new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(20) },
                new[] { TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(10) },
                new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10) },
                new[] { TimeSpan.FromSeconds(25) }
            };

        private static readonly IReadOnlyList<TimeSpan[]> _spotLengths10SecondsForTotalOf30Seconds =
            new List<TimeSpan[]>()
            {
                new[] { TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10) },
                new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15) },
                new[] { TimeSpan.FromSeconds(20) }
            };

        private static readonly IReadOnlyList<TimeSpan[]> _spotLengths15SecondsForTotalOf30Seconds =
            new List<TimeSpan[]>()
            {
                new[] { TimeSpan.FromSeconds(15) },
                new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5) },
                new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) }
            };

        private static readonly IReadOnlyList<TimeSpan[]> _spotLengths20SecondsForTotalOf30Seconds =
            new List<TimeSpan[]>()
            {
                new[] { TimeSpan.FromSeconds(10)},
                new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5) }
            };

        private static readonly IReadOnlyList<TimeSpan[]> _spotLengths25SecondsForTotalOf30Seconds =
            new List<TimeSpan[]>()
            {
                new[] { TimeSpan.FromSeconds(5)}
            };

        private static readonly IReadOnlyList<TimeSpan[]> _spotLengths35SecondsForTotalOf60Seconds =
            new List<TimeSpan[]>()
            {
                new[] { TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15) },
                new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(20) },
                new[] { TimeSpan.FromSeconds(25) }
            };

        private static readonly IReadOnlyList<TimeSpan[]> _spotLengths40SecondsForTotalOf60Seconds =
            new List<TimeSpan[]>()
            {
                new[] { TimeSpan.FromSeconds(20) },
                new[] { TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10) },
                new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15) }
            };

        private static readonly IReadOnlyList<TimeSpan[]> _spotLengths45SecondsForTotalOf60Seconds =
            new List<TimeSpan[]>()
            {
                new[] { TimeSpan.FromSeconds(15) },
                new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10) },
                new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5) }
            };

        private static readonly IReadOnlyList<TimeSpan[]> _spotLengths50SecondsForTotalOf60Seconds =
            new List<TimeSpan[]>()
            {
                new[] { TimeSpan.FromSeconds(10) },
                new[] { TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5) }
            };

        private static readonly IReadOnlyList<TimeSpan[]> _spotLengths55SecondsForTotalOf60Seconds =
            new List<TimeSpan[]>()
            {
                new[] { TimeSpan.FromSeconds(5) }
            };
    }
}
