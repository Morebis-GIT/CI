namespace xggameplan.AuditEvents.Sample
{
    /// <summary>
    /// List of audit event value types
    /// </summary>
    public static class AuditEventValueTypes
    {
        /// <summary>
        /// Free format message
        /// </summary>
        public const int Message = 1;

        /// <summary>
        /// Debug level
        /// </summary>
        public const int DebugLevel = 2;

        /// <summary>
        /// Exception
        /// </summary>
        public const int Exception = 3;

        /// <summary>
        /// Test values
        /// </summary>
        public const int TestValues = 4;

        /// <summary>
        /// Programme
        /// </summary>
        public const int Programme = 5;

        /// <summary>
        /// Channel
        /// </summary>
        public const int ChannelId = 6;

        /// <summary>
        /// User name
        /// </summary>
        public const int UserName = 7;
    }
}
