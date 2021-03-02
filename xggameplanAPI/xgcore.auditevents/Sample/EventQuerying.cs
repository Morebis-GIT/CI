using System;
using System.Collections.Generic;

namespace xggameplan.AuditEvents.Sample
{
    /// <summary>
    /// Queries events
    /// </summary>
    internal class EventQuerying
    {
        /// <summary>
        /// Gets logged events by debug level
        /// </summary>
        /// <param name="auditEventRepository"></param>
        public List<AuditEvent> GetEventsForAllTime(IAuditEventRepository auditEventRepository)
        {
            // Set filter
            AuditEventFilter auditEventFilter = new AuditEventFilter()
            {
                MinTimeCreated = DateTime.UtcNow.AddYears(-1),
                MaxTimeCreated = DateTime.UtcNow.AddDays(1),
                IncludeValues = true,
                AllFiltersRequired = true
            };

            // Get events
            var auditEvents = auditEventRepository.Get(auditEventFilter);
            return auditEvents;
        }

        /// <summary>
        /// Gets logged events by debug level
        /// </summary>
        /// <param name="auditEventRepository"></param>
        public List<AuditEvent> GetEventsForSource(IAuditEventRepository auditEventRepository, string source)
        {
            // Set filter
            AuditEventFilter auditEventFilter = new AuditEventFilter()
            {
                MinTimeCreated = DateTime.UtcNow.AddHours(-1),
                MaxTimeCreated = DateTime.UtcNow.AddHours(1),
                IncludeValues = true,
                AllFiltersRequired = true,
                Source = source
            };

            // Get events
            var auditEvents = auditEventRepository.Get(auditEventFilter);
            return auditEvents;
        }

        /// <summary>
        /// Gets logged events by user name
        /// </summary>
        /// <param name="auditEventRepository"></param>
        public List<AuditEvent> GetEventsForUserName(IAuditEventRepository auditEventRepository, List<string> userNames)
        {
            // Set filter
            AuditEventFilter auditEventFilter = new AuditEventFilter()
            {
                MinTimeCreated = DateTime.UtcNow.AddHours(-1),
                MaxTimeCreated = DateTime.UtcNow.AddHours(1),
                IncludeValues = true,
                AllFiltersRequired = true,
                ValueFilters = new List<AuditEventValueFilter>()
            };
            userNames.ForEach(userName => auditEventFilter.ValueFilters.Add(new AuditEventValueFilter() { ValueTypeID = AuditEventValueTypes.UserName, Value = userName }));

            // Get events
            var auditEvents = auditEventRepository.Get(auditEventFilter);
            return auditEvents;
        }

        /// <summary>
        /// Gets logged events by debug level
        /// </summary>
        /// <param name="auditEventRepository"></param>
        public List<AuditEvent> GetEventsForEventTypeIds(IAuditEventRepository auditEventRepository, List<int> eventTypeIds)
        {
            // Set filter
            AuditEventFilter auditEventFilter = new AuditEventFilter()
            {
                MinTimeCreated = DateTime.UtcNow.AddHours(-1),
                MaxTimeCreated = DateTime.UtcNow.AddHours(1),
                EventTypeIds = eventTypeIds,
                IncludeValues = true,
                AllFiltersRequired = true
            };

            // Get events
            var auditEvents = auditEventRepository.Get(auditEventFilter);
            return auditEvents;
        }

        /// <summary>
        /// Gets logged events by debug level
        /// </summary>
        /// <param name="auditEventRepository"></param>
        public List<AuditEvent> GetEventsForDebugLevel(IAuditEventRepository auditEventRepository, List<int> debugLevels)
        {
            // Set filter
            AuditEventFilter auditEventFilter = new AuditEventFilter()
            {
                MinTimeCreated = DateTime.UtcNow.AddHours(-1),
                MaxTimeCreated = DateTime.UtcNow.AddHours(1),
                IncludeValues = true,
                AllFiltersRequired = false,     // OR logic for values
                ValueFilters = new List<AuditEventValueFilter>()
            };
            debugLevels.ForEach(debugLevel => auditEventFilter.ValueFilters.Add(new AuditEventValueFilter() { ValueTypeID = AuditEventValueTypes.DebugLevel, Value = debugLevel }));

            // Get events
            var auditEvents = auditEventRepository.Get(auditEventFilter);
            return auditEvents;
        }

        /// <summary>
        /// Gets logged events that reference specific ChannelId
        /// </summary>
        /// <param name="auditEventRepository"></param>
        public List<AuditEvent> GetEventsForChannelId(IAuditEventRepository auditEventRepository, Guid channelId)
        {
            // Set filter
            AuditEventFilter auditEventFilter = new AuditEventFilter()
            {
                MinTimeCreated = DateTime.UtcNow.AddHours(-1),
                MaxTimeCreated = DateTime.UtcNow.AddHours(1),
                IncludeValues = true,
                AllFiltersRequired = false,     // OR logic for values
                ValueFilters = new List<AuditEventValueFilter>()
                {
                    new AuditEventValueFilter() { ValueTypeID = AuditEventValueTypes.ChannelId, Value = channelId }
                }
            };

            // Get events
            var auditEvents = auditEventRepository.Get(auditEventFilter);
            return auditEvents;
        }
    }
}
