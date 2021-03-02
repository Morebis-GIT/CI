namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Message creator for MS Teams
    /// </summary>
    public interface IMSTeamsMessageCreator
    {
        /// <summary>
        /// Id
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Posts message to MS Teams for audit event
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <param name="auditEventType"></param>
        /// <param name="postMessageSettings"></param>
        void PostMessage(AuditEvent auditEvent, AuditEventType auditEventType, MSTeamsPostMessageSettings postMessageSettings);

        /// <summary>
        /// Whether message can be created for particular audit event
        /// </summary>
        /// <param name="auditEventTypeId"></param>
        /// <returns></returns>
        bool Handles(AuditEvent auditEvent);
    }
}
