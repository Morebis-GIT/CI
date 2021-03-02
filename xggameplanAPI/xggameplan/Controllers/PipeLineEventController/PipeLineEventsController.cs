using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using xggameplan.AuditEvents;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    [RoutePrefix("pipeline-events")]
    public class PipelineEventsController : ApiController
    {
        private readonly IPipelineAuditEventRepository _pipelineAuditEventRepository;
        private readonly IMapper _mapper;

        public PipelineEventsController(IPipelineAuditEventRepository pipelineAuditEventRepository, IMapper mapper)
        {
            _pipelineAuditEventRepository = pipelineAuditEventRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all PipeLineAuditEvents
        /// </summary>
        [Route("")]
        [HttpGet]
        [AuthorizeRequest("PipelineAuditEvents")]
        [ResponseType(typeof(IEnumerable<PipelineAuditEvent>))]
        public IEnumerable<PipelineAuditEvent> Get()
        {
            var pipelineAuditEvents = _pipelineAuditEventRepository.GetAll();

            return pipelineAuditEvents;
        }

        /// <summary>
        /// Get all PipeLineAuditEvents by scenarioID
        /// </summary>
        /// <param name = "id" ></param >
        [Route("{id}")]
        [HttpGet]
        [AuthorizeRequest("PipelineAuditEvents")]
        [ResponseType(typeof(List<PipelineAuditEvent>))]
        public IHttpActionResult GetAllByScenarioID(Guid id)
        {
            if (!ModelState.IsValid || id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            List<PipelineAuditEvent> pipelineAuditEvents =
                _pipelineAuditEventRepository.GetAllByScenarioId(id).ToList();

            if (pipelineAuditEvents.Count == 0)
            {
                return this.Error().ResourceNotFound($"No pipeline audit events found for scenario {id}");
            }

            pipelineAuditEvents.AddRange(FindEventsWithNoScenarioId(id, pipelineAuditEvents.First().RunId));

            return Ok(pipelineAuditEvents);

            List<PipelineAuditEvent> FindEventsWithNoScenarioId(Guid scenarioid, Guid runId)
            {
                var allPipelineAuditEventsForRun =
                                _pipelineAuditEventRepository.GetAllByRunId(runId);

                var pipelineAuditEventsWithNoScenarioId =
                    allPipelineAuditEventsForRun
                    .Where(a => a.EventId == PipelineEventIDs.STARTED_NOTIFYING_MULE_SOFT_API || a.EventId == PipelineEventIDs.FINISHED_NOTIFYING_MULE_SOFT_API)
                    .ToList();

                pipelineAuditEventsWithNoScenarioId.ForEach(p => p.ScenarioId = scenarioid);

                return pipelineAuditEventsWithNoScenarioId;
            }
        }

        /// <summary>
        /// Creates a set of PipeLineAuditEvents
        /// </summary>
        [Route("")]
        [AuthorizeRequest("PipelineAuditEvents")]
        public IHttpActionResult Post([FromBody] PipelineAuditEventModel pipelineAuditEvent)
        {
            if (!ModelState.IsValid || pipelineAuditEvent == null)
            {
                return BadRequest(ModelState);
            }

            _pipelineAuditEventRepository.Add(Mappings.MapToPipelineAuditEvent(pipelineAuditEvent, _mapper));
            _pipelineAuditEventRepository.SaveChanges();

            return Ok();
        }
    }
}
