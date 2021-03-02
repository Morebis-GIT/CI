using xggameplan.AuditEvents;

namespace xggameplan.core.Services.OptimiserInputFilesSerialisers
{
    /// <summary>
    /// Exposes base functionality for serializer into xml files
    /// </summary>
    public class SerializerBase
    {
        private readonly IAuditEventRepository _auditEventRepository;

        /// <summary>Initializes a new instance of the <see cref="SerializerBase" /> class.</summary>
        /// <param name="auditEventRepository">The audit event repository.</param>
        public SerializerBase(IAuditEventRepository auditEventRepository)
        {
            _auditEventRepository = auditEventRepository;
        }

        /// <summary>Writes the specified message into audit event log.</summary>
        /// <param name="message">The message.</param>
        protected void RaiseInfo(string message) =>
            _auditEventRepository.Insert(AuditEventFactory.CreateAuditEventForInformationMessage(0, 0, message));
    }
}
