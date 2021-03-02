using System;
using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using xggameplan.core.BRS;
using xggameplan.Filters;
using xggameplan.model.External;

namespace xggameplan.Controllers
{
    [RoutePrefix("brs-indicator")]
    public class BRSIndicatorController : ApiController
    {
        private readonly IRunRepository _runRepository;
        private readonly IBRSIndicatorManager _brsManager;
        private readonly IScenarioResultRepository _scenarioResultRepository;
        private readonly IBRSConfigurationTemplateRepository _brsConfigurationTemplateRepository;
        private readonly IMapper _mapper;

        public BRSIndicatorController(
            IBRSIndicatorManager brsManager,
            IRunRepository runRepository,
            IScenarioResultRepository scenarioResultRepository,
            IBRSConfigurationTemplateRepository brsConfigurationTemplateRepository,
            IMapper mapper)
        {
            _brsManager = brsManager;
            _runRepository = runRepository;
            _scenarioResultRepository = scenarioResultRepository;
            _brsConfigurationTemplateRepository = brsConfigurationTemplateRepository;
            _mapper = mapper;
        }

        [Route("Calculate")]
        [AuthorizeRequest("brs-indicator")]
        public IHttpActionResult GetBRSIndicator(Guid runId, int? brsConfigurationTemplateId = null)
        {
            var run = _runRepository.Find(runId);
            if (run == null)
            {
                return NotFound();
            }

            var scenarioResults = _brsManager.CalculateBRSIndicators(run, brsConfigurationTemplateId);
            return Ok(_mapper.Map<List<BRSIndicatorValueForScenarioResultModel>>(scenarioResults));
        }

        [HttpPost]
        [Route("for-test/calculate-and-save")]
        [AuthorizeRequest("brs-indicator")]
        public IHttpActionResult PostCalculateAndSave(Guid runId, int? brsConfigurationTemplateId = null)
        {
            var run = _runRepository.Find(runId);
            if (run == null)
            {
                return NotFound();
            }

            var scenarioResults = _brsManager.CalculateBRSIndicators(run, brsConfigurationTemplateId);
            _scenarioResultRepository.UpdateRange(scenarioResults);
            if (brsConfigurationTemplateId.HasValue)
            {
                run.BRSConfigurationTemplateId = brsConfigurationTemplateId.Value;
                _runRepository.Update(run);
            }
            _scenarioResultRepository.SaveChanges();

            return Ok(_mapper.Map<List<BRSIndicatorValueForScenarioResultModel>>(scenarioResults));
        }
    }
}
