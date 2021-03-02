using System;

namespace xggameplan.AuditEvents
{
    public static class AuditEventTypes
    {
        public const int Exception = 1;
        public const int InformationMessage = 2;
        public const int BackendMethod = 3;
        public const int SystemTestsSuccess = 4;
        public const int SystemTestsFailed = 5;
        public const int UserLoginSuccess = 6;
        public const int UserLoginFailed = 7;
        public const int TenantCreate = 11;
        public const int UserCreate = 12;
        public const int UserLogout = 15;
        public const int UserUpdate = 17;
        public const int TenantUpdate = 18;
        public const int ChannelUpdate = 19;
        public const int WarningMessage = 23;

        public const int GamePlanAutoBookRun = 24;
        public const int GamePlanRun = 25;
        public const int GamePlanSmoothRun = 26;
        public const int GamePlanAutoBookEvent = 27;
        public const int GamePlanAutoBookLogs = 28;
        public const int GamePlanRunValidationFailure = 29;
        public const int GamePlanScheduleDataUploadStarted = 30;
        public const int GamePlanRunStarted = 31;
        public const int GamePlanRunStartFailed = 32;
        public const int GamePlanRunCompleted = 33;
        public const int GamePlanRunCreated = 34;
        public const int GamePlanSystemState = 35;
        public const int GamePlanRunDeleted = 36;
        public const int GamePlanRecalculateBreakAvailability = 37;

        /// <summary>
        /// Legacy values. These value cannot be reassigned and must not be used!
        /// </summary>
        [Obsolete("Legacy values. Do not use in new code", true)]
        private static class LegacyAuditEventTypes
        {
            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int ScheduleImportSuccess = 8;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int ScheduleImportFailed = 9;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int ChannelCreate = 10;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int ScheduleDayUpdate = 13;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int ScheduleDayStateUpdate = 14;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int PlaylistCreate = 16;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int ScheduleDayCreate = 20;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int PlaylistExport = 21;

            [Obsolete("Legacy value. Do not use in new code", true)]
            public const int SimpleEmail = 22;
        }
    }
}
