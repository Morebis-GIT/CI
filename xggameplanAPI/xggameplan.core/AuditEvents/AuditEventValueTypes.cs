using System;

namespace xggameplan.AuditEvents
{
    public static class AuditEventValueTypes
    {
        public const int Exception = 1;
        public const int ClientIPAddress = 2;
        public const int UserEmailAddress = 3;
        public const int RecipientEmailAddress = 24;
        public const int AccessToken = 6;
        public const int Message = 7;
        public const int LoginFailureReason = 8;
        public const int TenantID = 15;
        public const int UserID = 16;
        public const int TenantPrevious = 20;
        public const int TenantCurrent = 21;
        public const int UserPrevious = 22;
        public const int UserCurrent = 23;
        public const int RequestUserAgent = 25;
        public const int Link = 27;

        public const int GamePlanRunID = 29;
        public const int GamePlanScenarioID = 30;
        public const int GamePlanAutoBookID = 31;
        public const int GamePlanAutoBookMessage = 32;
        public const int GamePlanPipelineEventID = 33;
        public const int GamePlanPipelineEventErrorMessage = 34;
        public const int GamePlanSalesAreaName = 35;
        public const int GamePlanAutoBookEventID = 36;
        public const int GamePlanAutoBookLog = 37;
        public const int GamePlanSystemState = 38;

        /// <summary>
        /// Legacy values. These value cannot be reassigned and must not be used!
        /// </summary>
        [Obsolete("Legacy values. Do not use in new code", true)]
        private static class LegacyAuditEventValueTypes
        {
            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int Request = 4;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int Response = 5;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int PlayoutPlaylistCurrent = 9;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int PlayoutPlaylistPrevious = 26;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int ScheduleDayPrevious = 10;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int ScheduleDayCurrent = 11;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int EmailAttachment = 12;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int ScheduleImportFilename = 13;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int ChannelID = 14;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int ScheduleDayID = 17;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int ChannelPrevious = 18;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int ChannelCurrent = 19;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int Email = 28;
        }
    }
}
