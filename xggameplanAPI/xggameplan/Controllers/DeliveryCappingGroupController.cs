using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.DeliveryCappingGroup;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Runs;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.Controllers
{
    [RoutePrefix("DeliveryCappingGroups")]
    [FeatureFilter(nameof(ProductFeature.DeliveryCappingGroup))]
    public class DeliveryCappingGroupController : ApiController
    {
        private readonly IDeliveryCappingGroupRepository _repository;
        private readonly IRunRepository _runRepository;
        private readonly IModelDataValidator<DeliveryCappingGroupModel> _validator;
        private readonly IMapper _mapper;

        private readonly List<RunStatus> _notFinishedRunStatuses = new List<RunStatus>
        {
            RunStatus.InProgress,
            RunStatus.NotStarted
        };

        public DeliveryCappingGroupController(
            IDeliveryCappingGroupRepository repository,
            IRunRepository runRepository,
            IModelDataValidator<DeliveryCappingGroupModel> validator,
            IMapper mapper
        )
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _runRepository = runRepository;
        }

        /// <summary>
        /// Get all delivery capping groups
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("DeliveryCappingGroups")]
        [ResponseType(typeof(IEnumerable<DeliveryCappingGroupModel>))]
        public IHttpActionResult Get()
        {
            var items = _repository.GetAll()?.ToList();
            return Ok(_mapper.Map<IEnumerable<DeliveryCappingGroupModel>>(items));
        }

        /// <summary>
        /// Get delivery capping group by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("DeliveryCappingGroups")]
        [ResponseType(typeof(DeliveryCappingGroupModel))]
        public IHttpActionResult Get(int id)
        {
            var item = _repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<DeliveryCappingGroupModel>(item));
        }

        /// <summary>
        /// Create delivery capping group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("DeliveryCappingGroups")]
        public IHttpActionResult Post([FromBody] DeliveryCappingGroupModel model)
        {
            if (!_validator.IsValid(model))
            {
                return _validator.BadRequest();
            }

            var item = _mapper.Map<DeliveryCappingGroup>(model);
            _repository.Add(item);
            _repository.SaveChanges();

            return Ok(_mapper.Map<DeliveryCappingGroupModel>(item));
        }

        /// <summary>
        /// Update delivery capping group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("DeliveryCappingGroups")]
        [ResponseType(typeof(DeliveryCappingGroupModel))]
        public IHttpActionResult Put([FromUri] int id, [FromBody] DeliveryCappingGroupModel model)
        {
            if (model != null && id != model.Id)
            {
                ModelState.AddModelError(nameof(DeliveryCappingGroupModel.Id), "Id does not match");
                return BadRequest(ModelState);
            }

            if (!_validator.IsValid(model))
            {
                return _validator.BadRequest();
            }

            var entity = _repository.Get(model.Id);

            if (entity == null)
            {
                return NotFound();
            }

            entity.Description = model.Description;
            entity.Percentage = model.Percentage;
            entity.ApplyToPrice = model.ApplyToPrice;
            _repository.Update(entity);

            _repository.SaveChanges();

            return Ok(_mapper.Map<DeliveryCappingGroupModel>(entity));
        }

        /// <summary>
        /// Delete delivery capping group by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("DeliveryCappingGroups")]
        public IHttpActionResult Delete(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError(nameof(id), "Please specify positive delivery capping group id");
                return BadRequest(ModelState);
            }

            if (_repository.Get(id) is null)
            {
                return NotFound();
            }

            var runs = _runRepository.GetRunsByDeliveryCappingGroupId(id).ToList();

            var runsWithDCG = runs.Where(x => _notFinishedRunStatuses.Contains(x.RunStatus));
            if (runsWithDCG.Any())
            {
                ModelState.AddModelError(nameof(id),
                    $"Delivery capping group with id: {id} is used in runs with status 'In Progress' / 'Not Started'.{Environment.NewLine}" +
                    $"Run ids: {string.Join(", ", runsWithDCG.Select(x => x.Id))}"
                );
                return BadRequest(ModelState);
            }

            for (int i = 0; i< runs.Count; i++)
            {
                runs[i].CampaignsProcessesSettings = runs[i].CampaignsProcessesSettings
                    .Select(x =>
                    {
                        if (x.DeliveryCappingGroupId == id)
                        {
                            x.DeliveryCappingGroupId = 0;
                        }
                        return x;
                    })
                    .Where(x => !x.IsEmpty)
                    .ToList();
            }

            _runRepository.UpdateRange(runs);
            _repository.Delete(id);
            return this.NoContent();
        }
    }
}
