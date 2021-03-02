using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.LandmarkRunQueues;
using ImagineCommunications.GamePlan.Domain.LandmarkRunQueues.Objects;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using ImagineCommunications.GamePlan.Domain.RunTypes.Objects;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.model.External;
using xggameplan.Model;
using xggameplan.Services;
using xggameplan.Validations.RunTypes;

namespace xggameplan.Controllers
{
    [RoutePrefix("RunTypes")]
    [FeatureFilter(nameof(ProductFeature.RunType))]
    public class RunTypesController : ApiController
    {
        private readonly IMapper _mapper;
        private readonly IRunTypeRepository _runTypeRepository;
        private readonly IFeatureManager _featureManager;
        private readonly IAnalysisGroupRepository _analysisGroupRepository;
        private readonly ILandmarkRunQueueRepository _landmarkRunQueueRepository;

        public RunTypesController(
            IRunTypeRepository runTypeRepository,
            IFeatureManager featureManager,
            IMapper mapper,
            IAnalysisGroupRepository analysisGroupRepository,
            ILandmarkRunQueueRepository landmarkRunQueueRepository)
        {
            _mapper = mapper;
            _analysisGroupRepository = analysisGroupRepository;
            _runTypeRepository = runTypeRepository;
            _featureManager = featureManager;
            _landmarkRunQueueRepository = landmarkRunQueueRepository;
        }

        /// <summary>
        /// Create Run Type
        /// </summary>
        /// <param name="command">Run Type Input Object. Valid KPI values are:
        /// <list type="bullet">
        /// <item> ratingsDelivery </item>
        /// <item> deliveryPercentage </item>
        /// <item> revenueBooked </item>
        /// <item> poolValue </item>
        /// <item> spots </item>
        /// <item> zeroRatedSpots </item>
        /// </list>
        /// </param>
        [Route("")]
        [AuthorizeRequest("RunTypes")]
        [HttpPost]
        public IHttpActionResult Post([FromBody] CreateRunTypeModel command)
        {
            if (command == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            (bool isValid, string message) = RunTypesValidations.ValidateRunTypeName(command.Name);
            if (!isValid)
            {
                return this.Error().BadRequest(message);
            }

            var runType = _runTypeRepository.FindByName(command.Name);
            if (runType != null)
            {
                return this.Error().BadRequest("Run type name already exists: " + command.Name);
            }

            (bool kpiIsValid, string kpiErrorMessage) = RunTypesValidations.ValidateDefaultKPIName(command.DefaultKPI);
            if (!kpiIsValid)
            {
                return this.Error().BadRequest(kpiErrorMessage);
            }

            var allAnalysisGroups = _analysisGroupRepository.GetAll();
            (bool analysisGroupsIsValid, string errorMessage) = RunTypesValidations.ValidateRunTypeAnalysisGroupList(command.RunTypeAnalysisGroups, allAnalysisGroups);
            if (!analysisGroupsIsValid)
            {
                return this.Error().BadRequest(errorMessage);
            }

            var runTypeToCreate = _mapper.Map<RunType>(command);
            runTypeToCreate.ModifiedDate = DateTime.UtcNow;
            _runTypeRepository.Add(runTypeToCreate);
            _runTypeRepository.SaveChanges();

            return Ok(_mapper.Map<RunTypeModel>(runTypeToCreate));
        }

        /// <summary>
        /// Get All Run Types
        /// </summary>
        [Route("")]
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            return Ok(Mappings.MapToRunTypes(_runTypeRepository.GetAll(), _mapper));
        }

        /// <summary>
        /// Update Run Type
        /// </summary>
        /// <param name="id">Id of the Run Type that is to be updated</param>
        /// <param name="command">Run Type Input Object. Valid KPI values are:
        /// <list type="bullet">
        /// <item> ratingsDelivery </item>
        /// <item> deliveryPercentage </item>
        /// <item> revenueBooked </item>
        /// <item> poolValue </item>
        /// <item> spots </item>
        /// <item> zeroRatedSpots </item>
        /// </list>
        /// </param>
        [Route("{id}")]
        [AuthorizeRequest("RunTypes")]
        [HttpPut]
        [ResponseType(typeof(RunType))]
        public IHttpActionResult Put(int id, [FromBody] CreateRunTypeModel command)
        {
            if (command is null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            var runType = _runTypeRepository.Get(id);
            if (runType is null)
            {
                return this.Error().ResourceNotFound();
            }

            (bool isValid, string message) = RunTypesValidations.ValidateRunTypeName(command.Name);
            if (!isValid)
            {
                return this.Error().BadRequest(message);
            }

            var runTypeByName = _runTypeRepository.FindByName(command.Name);
            if (runTypeByName != null && runTypeByName.Id != id)
            {
                return this.Error().BadRequest("Run type name already exists: " + command.Name);
            }

            (bool kpiIsValid, string kpiErrorMessage) = RunTypesValidations.ValidateDefaultKPIName(command.DefaultKPI);
            if (!kpiIsValid)
            {
                return this.Error().BadRequest(kpiErrorMessage);
            }

            var allAnalysisGroups = _analysisGroupRepository.GetAll();
            (bool analysisGroupsIsValid, string errorMessage) = RunTypesValidations.ValidateRunTypeAnalysisGroupList(command.RunTypeAnalysisGroups, allAnalysisGroups);
            if (!analysisGroupsIsValid)
            {
                return this.Error().BadRequest(errorMessage);
            }

            _mapper.Map(command, runType);
            runType.ModifiedDate = DateTime.UtcNow;
            _runTypeRepository.Update(runType);
            _runTypeRepository.SaveChanges();

            return Ok(runType);
        }

        /// <summary>
        /// Gets the landmark run queues.
        /// </summary>
        /// <returns>The landmark run queues.</returns>
        [Route("landmark-run-queues")]
        [AuthorizeRequest("RunTypes")]
        [HttpGet]
        [ResponseType(typeof(IEnumerable<LandmarkRunQueue>))]
        public IEnumerable<LandmarkRunQueue> GetLandmarkRunQueues()
        {
            return _landmarkRunQueueRepository.GetAll();
        }
    }
}
