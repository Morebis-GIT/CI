using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;

namespace ImagineCommunications.GamePlan.Domain.Shared.Schedules
{
    /// <summary>
    /// Indexed schedule to enable fast break lookup.
    /// </summary>
    /// <typeparam name="T1">Break details (E.g. Break, BreakSimple)</typeparam>
    /// <typeparam name="T2">Key (E.g. CustomId, ExternalBreakRef)</typeparam>
    public class ScheduleIndexed<T1, T2>
    {
        /// <summary>
        /// The Schedule's sales area.
        /// </summary>
        public string SalesArea { get; set; }

        /// <summary>
        /// The Schedule.
        /// </summary>
        public Schedule Schedule { get; set; }

        /// <summary>
        /// The date of the Schedule. Does not contain the Schedule time.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Stop using this to get the number of indexed breaks. Use <see cref="BreaksCount"/>.
        /// </summary>
        public Dictionary<T2, T1> BreaksByKey = new Dictionary<T2, T1>();

        /// <summary>
        /// Gets the number of unique <see cref="Break"/> objects in the <see cref="Schedule"/>.
        /// </summary>
        public int BreaksCount => BreaksByKey.Count;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="schedule">Schedule to index</param>
        /// <param name="convertBreakFunction">Converts Break to target object</param>
        /// <param name="getKeyFunction">Generates key for Break</param>
        public ScheduleIndexed(Schedule schedule, Func<Break, T1> convertBreakFunction, Func<Break, T2> getKeyFunction)
        {
            SalesArea = schedule.SalesArea;
            Date = schedule.Date.Date;
            Schedule = schedule;

            // Index breaks
            try
            {
                if (schedule.Breaks != null)
                {
                    foreach (var @break in schedule.Breaks)
                    {
                        T2 breakKey = getKeyFunction(@break);
                        if (!BreaksByKey.ContainsKey(breakKey))
                        {
                            BreaksByKey.Add(breakKey, convertBreakFunction(@break));
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Returns break by key
        /// </summary>
        /// <param name="breakKey"></param>
        /// <returns></returns>
        public T1 GetBreak(T2 breakKey)
        {
            _ = BreaksByKey.TryGetValue(breakKey, out var res);
            return res;
        }
    }
}
