using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using xggameplan.AuditEvents;
using xggameplan.Errors;
using xggameplan.Filters;

namespace xggameplan.Controllers
{
    /// <summary>
    /// API for audit events
    /// </summary>
    [RoutePrefix("AuditEvent")]
    public class AuditEventsController : ApiController
    {
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IMapper _mapper;

        public AuditEventsController(IAuditEventRepository auditEventRepository, IMapper mapper)
        {
            _auditEventRepository = auditEventRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns audit events meeting the criteria.
        /// </summary>
        /// <returns>Audit events matching the criteria</returns>
        [Route("Search")]
        [AuthorizeRequest("AuditEvents")]
        [ResponseType(typeof(IEnumerable<AuditEventModel>))]
        public List<AuditEventModel> PostGet([FromBody] AuditEventFilterModel command)
        {
            AuditEventFilter auditEventFilter = _mapper.Map<AuditEventFilter>(command);

            // Convert values
            List<AuditEventValueType> valueTypes = new AuditEventValueTypeRepository().GetAll();

            foreach (AuditEventValueFilter valueFilter in auditEventFilter.ValueFilters)
            {
                valueFilter.Value = AuditEventValue.ConvertValue(valueFilter.ValueTypeID, valueFilter.Value, valueTypes.Find(vt => vt.ID == valueFilter.ValueTypeID));
            }

            // Get audit events
            var auditEvents = _auditEventRepository.Get(auditEventFilter);
            return _mapper.Map<List<AuditEventModel>>(auditEvents);
        }

        /// <summary>
        /// Creates audit event
        /// </summary>
        /// <param name="command">New audit event values</param>
        [Route("")]
        [AuthorizeRequest("AuditEvents")]
        public IHttpActionResult Post([FromBody] CreateAuditEventModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Audit event body parameters missing");
            }

            List<AuditEventValueType> valueTypes = new AuditEventValueTypeRepository().GetAll();

            var auditEvent = new AuditEvent()
            {
                ID = Guid.NewGuid().ToString(),
                TimeCreated = (command.TimeCreated == null ? DateTime.UtcNow : (DateTime)command.TimeCreated),
                EventTypeID = command.EventTypeID,
                TenantID = 0,
                UserID = 0,
            };

            command.Values.ForEach(item => auditEvent.Values.Add(new AuditEventValue() { TypeID = item.TypeID, Value = AuditEventValue.ConvertValue(item.TypeID, item.Value, valueTypes.Find(vt => vt.ID == item.TypeID)) }));

            _auditEventRepository.Insert(auditEvent);

            return Ok();
        }
    }
}
