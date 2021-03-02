using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using xggameplan.core.Services;
using xggameplan.Errors;
using xggameplan.Extensions;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;
using xggameplan.Validations;

namespace xggameplan.Controllers
{
    [RoutePrefix("analysis-groups")]
    public class AnalysisGroupsController : ApiController
    {
        private readonly IAnalysisGroupRepository _analysisGroupRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IClashRepository _clashRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IModelDataValidator<CreateAnalysisGroupModel> _validator;
        private readonly IAnalysisGroupCampaignQuery _groupCampaignQuery;
        private readonly IMapper _mapper;

        public AnalysisGroupsController(IAnalysisGroupRepository analysisGroupRepository,
            ICampaignRepository campaignRepository,
            IClashRepository clashRepository,
            IProductRepository productRepository,
            IUsersRepository usersRepository,
            IModelDataValidator<CreateAnalysisGroupModel> validator,
            IAnalysisGroupCampaignQuery groupCampaignQuery,
            IMapper mapper)
        {
            _analysisGroupRepository = analysisGroupRepository;
            _campaignRepository = campaignRepository;
            _clashRepository = clashRepository;
            _productRepository = productRepository;
            _usersRepository = usersRepository;
            _validator = validator;
            _groupCampaignQuery = groupCampaignQuery;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all analysis groups
        /// </summary>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("analysis-groups")]
        [ResponseType(typeof(IEnumerable<AnalysisGroupModel>))]
        public IHttpActionResult Get()
        {
            var analysisGroups = _analysisGroupRepository.GetAll().ToList();
            return Ok(Mappings.MapToAnalysisGroupModels(analysisGroups, _campaignRepository, _clashRepository,
                _productRepository, _usersRepository, _mapper));
        }

        /// <summary>
        /// Get analysis group by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("analysis-groups")]
        [ResponseType(typeof(AnalysisGroupModel))]
        public IHttpActionResult Get(int id)
        {
            var analysisGroup = _analysisGroupRepository.Get(id);
            if (analysisGroup is null)
            {
                return this.Error().ResourceNotFound($"Analysis Group with id {id} does not exist");
            }

            return Ok(Mappings.MapToAnalysisGroupModel(analysisGroup, _campaignRepository, _clashRepository,
                _productRepository, _usersRepository, _mapper));
        }

        /// <summary>
        /// Create analysis group
        /// </summary>
        /// <param name="newAnalysisGroup"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("analysis-groups")]
        public IHttpActionResult Post([FromBody] CreateAnalysisGroupModel newAnalysisGroup)
        {
            if (!_validator.IsValid(newAnalysisGroup))
            {
                return _validator.BadRequest();
            }

            var item = _mapper.Map<AnalysisGroup>(newAnalysisGroup);
            item.Id = 0;
            item.CreatedBy = GetCurrentUserId();
            item.ModifiedBy = item.CreatedBy;

            _analysisGroupRepository.Add(item);
            _analysisGroupRepository.SaveChanges();

            return Ok(Mappings.MapToAnalysisGroupModel(item, _campaignRepository, _clashRepository, _productRepository,
                _usersRepository, _mapper));
        }

        /// <summary>
        /// Update analysis group
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedAnalysisGroup"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("analysis-groups")]
        [ResponseType(typeof(DeliveryCappingGroupModel))]
        public IHttpActionResult Put([FromUri] int id, [FromBody] CreateAnalysisGroupModel updatedAnalysisGroup)
        {
            if (updatedAnalysisGroup != null && updatedAnalysisGroup.Id != id)
            {
                return this.Error().InvalidParameters("Id does not match");
            }

            if (!_validator.IsValid(updatedAnalysisGroup))
            {
                return _validator.BadRequest();
            }

            var existingAnalysisGroup = _analysisGroupRepository.Get(id);
            if (existingAnalysisGroup is null)
            {
                return this.Error().ResourceNotFound($"Analysis Group with id {id} does not exist");
            }

            _mapper.Map(updatedAnalysisGroup, existingAnalysisGroup);
            existingAnalysisGroup.ModifiedBy = GetCurrentUserId();

            _analysisGroupRepository.Update(existingAnalysisGroup);
            _analysisGroupRepository.SaveChanges();

            return Ok(Mappings.MapToAnalysisGroupModel(existingAnalysisGroup, _campaignRepository, _clashRepository,
                _productRepository, _usersRepository, _mapper));
        }

        /// <summary>
        /// Delete analysis group by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("analysis-groups")]
        public IHttpActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return this.Error().InvalidParameters("Analysis Group id must be a positive number");
            }

            var existingAnalysisGroup = _analysisGroupRepository.Get(id);
            if (existingAnalysisGroup is null)
            {
                return this.Error().ResourceNotFound($"Analysis Group with id {id} does not exist");
            }

            existingAnalysisGroup.IsDeleted = true;
            existingAnalysisGroup.ModifiedBy = GetCurrentUserId();
            _analysisGroupRepository.Update(existingAnalysisGroup);

            return this.NoContent();
        }

        [Route("{id}/campaigns"), HttpGet, AuthorizeRequest("analysis-groups")]
        public IHttpActionResult GetCampaignsForAnalysisGroup(int id)
        {
            var existingAnalysisGroup = _analysisGroupRepository.Get(id);

            if (existingAnalysisGroup is null)
            {
                return this.Error().ResourceNotFound($"Analysis Group with id {id} does not exist");
            }

            var campaignIds = _groupCampaignQuery.GetAnalysisGroupCampaigns(existingAnalysisGroup.Filter);

            return Ok(campaignIds);
        }

        private static int GetCurrentUserId() => HttpContext.Current.GetCurrentUser().Id;
    }
}
