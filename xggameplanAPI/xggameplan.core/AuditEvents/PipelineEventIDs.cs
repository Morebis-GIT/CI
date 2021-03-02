namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Pipeline events
    /// </summary>
    public static class PipelineEventIDs
    {
        public const int STARTED_GENERATING_INPUT_FILES = 1;
        public const int FINISHED_GENERATING_INPUT_FILES = 2;
        public const int STARTED_SMOOTHING_INPUT_FILES = 3;
        public const int FINISHED_SMOOTHING_INPUT_FILES = 4;
        public const int STARTED_ZIPPING_INPUT_FILES = 5;
        public const int FINISHED_ZIPPING_INPUT_FILES = 6;
        public const int STARTED_UPLOADING_INPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE = 7;
        public const int FINISHED_UPLOADING_INPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE = 8;
        public const int STARTED_NOTIFYING_AUTOBOOK_API = 9;
        public const int FINISHED_NOTIFYING_AUTOBOOK_API = 10;
        public const int STARTED_DOWNLOADING_INPUT_ZIP_ARCHIVE_FROM_CLOUD_STORAGE = 11;
        public const int FINISHED_DOWNLOADING_INPUT_ZIP_ARCHIVE_FROM_CLOUD_STORAGE = 12;
        public const int STARTED_UNZIPPING_INPUT_FILES = 13;
        public const int FINISHED_UNZIPPING_INPUT_FILES = 14;
        public const int STARTED_RUNNING_AUTOBOOK_CPP_OPTIMIZER = 15;
        public const int FINISHED_RUNNING_AUTOBOOK_CPP_OPTIMIZER = 16;
        public const int STARTED_ZIPPING_OUTPUT_FILES = 17;
        public const int FINISHED_ZIPPING_OUTPUT_FILES = 18;
        public const int STARTED_UPLOADING_OUTPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE = 19;
        public const int FINISHED_UPLOADING_OUTPUT_ZIP_ARCHIVE_TO_CLOUD_STORAGE = 20;
        public const int STARTED_NOTIFYING_GAMEPLAN_API_TASK_DONE = 21;
        public const int FINISHED_NOTIFYING_GAMEPLAN_API_TASK_DONE = 22;
        public const int STARTED_NOTIFYING_GAMEPLAN_API_STATUS_IDLE = 23;
        public const int FINISHED_NOTIFYING_GAMEPLAN_API_STATUS_IDLE = 24;
        public const int STARTED_DOWNLOADING_ZIP_ARCHIVE_FROM_CLOUD_STORAGE = 25;
        public const int FINISHED_DOWNLOADING_ZIP_ARCHIVE_FROM_CLOUD_STORAGE = 26;
        public const int STARTED_EXPORTING_DATA_TO_DATABASE = 27;
        public const int FINISHED_EXPORTING_DATA_TO_DATABASE = 28;
        public const int STARTED_NOTIFYING_MULE_SOFT_API = 29;
        public const int FINISHED_NOTIFYING_MULE_SOFT_API = 30;
        public const int STARTED_PRE_RUN_RECALCULATING_BREAK_AVAILABILITY = 31;
        public const int FINISHED_PRE_RUN_RECALCULATING_BREAK_AVAILABILITY = 32;
        public const int STARTED_POST_SMOOTH_RECALCULATING_BREAK_AVAILABILITY = 33;
        public const int FINISHED_POST_SMOOTH_RECALCULATING_BREAK_AVAILABILITY = 34;
    }
}
