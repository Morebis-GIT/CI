using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Queries;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Exceptions;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Generic.Validation;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.common.Utilities;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Interfaces;
using xggameplan.Errors;
using xggameplan.Extensions;
using xggameplan.Filters;
using xggameplan.KPIProcessing;
using xggameplan.Model;
using xggameplan.Reports.Common;
using xggameplan.Reports.ExcelReports.Campaigns;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    [RoutePrefix("Campaigns")]
    public partial class CampaignsController : ApiController
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IDataChangeValidator _dataChangeValidator;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly IMapper _mapper;
        private readonly IDemographicRepository _demographicRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICampaignExcelReportGenerator _campaignsReportGenerator;
        private readonly IReportColumnFormatter _reportColumnFormatter;
        private readonly IProgrammeCategoryHierarchyRepository _programmeCategoryRepository;
        private readonly IFeatureManager _featureManager;
        private readonly ICampaignFlattener _campaignFlattener;
        private readonly ICampaignCleaner _campaignCleaner;
        private readonly IProgrammeRepository _programmeRepository;
        private readonly IClashRepository _clashRepository;
        private readonly ICampaignPassPrioritiesService _campaignPassPrioritiesService;

        public CampaignsController(
            ICampaignRepository campaignRepository,
            IRecommendationRepository recommendationRepository,
            IDataChangeValidator dataChangeValidator,
            IMapper mapper,
            IDemographicRepository demographicRepository,
            ISalesAreaRepository salesAreaRepository,
            IProductRepository productRepository,
            ICampaignExcelReportGenerator campaignsReportGenerator,
            IReportColumnFormatter reportColumnFormatter,
            IProgrammeRepository programmeRepository,
            IClashRepository clashRepository,
            IProgrammeCategoryHierarchyRepository programmeCategoryRepository,
            IFeatureManager featureManager,
            ICampaignFlattener campaignFlattener,
            ICampaignCleaner campaignCleaner,
            ICampaignPassPrioritiesService campaignPassPrioritiesService)
        {
            _campaignRepository = campaignRepository;
            _dataChangeValidator = dataChangeValidator;
            _recommendationRepository = recommendationRepository;
            _mapper = mapper;
            _demographicRepository = demographicRepository;
            _clashRepository = clashRepository;
            _salesAreaRepository = salesAreaRepository;
            _productRepository = productRepository;
            _campaignsReportGenerator = campaignsReportGenerator;
            _reportColumnFormatter = reportColumnFormatter;
            _programmeCategoryRepository = programmeCategoryRepository;
            _featureManager = featureManager;
            _campaignFlattener = campaignFlattener;
            _campaignCleaner = campaignCleaner;
            _programmeRepository = programmeRepository;
            _campaignPassPrioritiesService = campaignPassPrioritiesService;
        }

        /// <summary>
        /// Gets all the Campaigns. Never going to production! Just for testing
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Campaigns")]
        [ResponseType(typeof(IEnumerable<CampaignModel>))]
        public IEnumerable<CampaignModel> Get()
        {
            var campaigns = _campaignRepository.GetAll().ToList();
            return _mapper.Map<List<CampaignModel>>(campaigns);
        }

        /// <summary>
        /// Gets the flattened view of all the campaigns
        /// </summary>
        [Route("Flatten")]
        [AuthorizeRequest("Campaigns")]
        [ResponseType(typeof(IEnumerable<CampaignFlattenedModel>))]
        public IEnumerable<CampaignFlattenedModel> GetFlattennedCampaigns() =>
            _campaignFlattener.Flatten(_campaignRepository.GetAll().ToList());

        /// <summary>
        /// Gets Campaigns as byte array for excel export.
        /// </summary>
        [Route("Export")]
        [AuthorizeRequest("Campaigns")]
        public IHttpActionResult Post([FromBody] IEnumerable<ColumnStatusModel> columnStatusModelList,
           [FromUri] CampaignSearchQueryModel queryModel)
        {
            if (queryModel != null)
            {
                queryModel.Skip = null;
                queryModel.Top = null;
            }

            var campaigns = _campaignRepository.GetWithProduct(queryModel).Items;
            var campaignModels = _mapper.Map<List<CampaignReportModel>>(campaigns);
            var excelFile = _campaignsReportGenerator.GetReportAsByteArray("Campaigns", campaignModels,
                columnStatusModelList, _reportColumnFormatter);
            var responseMsg = Request.CreateResponse(HttpStatusCode.OK);
            responseMsg.Content = new StreamContent(new MemoryStream(excelFile));
            responseMsg.Content.Headers.Add("IsCompressedContent", "true");
            return ResponseMessage(responseMsg);
        }

        /// <summary>
        /// Gets all the Campaigns filtered, sorted and paged using values
        /// supplied in the queryModel parameters
        /// </summary>
        /// <param name="queryModel">
        /// The queryModel containing the querystring parameters
        /// <see cref="CampaignSearchQueryModel"/> used to filtered, sorted and
        /// page the list of Campaigns
        /// </param>
        /// <returns>
        /// <see cref="OkNegotiatedContentResult{T}"/> containing a list of
        /// Campaigns as <see cref="SearchResultModel{TItem}"/> if successfull.
        /// </returns>
        /// <response code="200">
        /// Returns <see cref="OkNegotiatedContentResult{SearchResultModel}"/>
        /// containing a list of Campaigns as
        /// <see cref="SearchResultModel{CampaignWithProductFlatModel}"/> with a
        /// 200 OK Reponse code
        /// </response>
        /// <response code="500">Returns a 500 Internal Error for any failures</response>
        /// <remarks>
        /// Sample search parameters:
        ///
        /// startDate: 2018-07-01
        /// endDate: 2019-07-21
        /// top: 20
        /// skip: 0
        /// </remarks>
        [Route("Search")]
        [AuthorizeRequest("Campaigns")]
        [ResponseType(typeof(SearchResultModel<CampaignWithProductFlatModel>))]
        public IHttpActionResult GetCampaignsList([FromUri] CampaignSearchQueryModel queryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pagedQueryResult = _campaignRepository.GetWithProduct(queryModel);

            return Ok(new SearchResultModel<CampaignWithProductFlatModel>
            {
                Items = pagedQueryResult.Items?.ToList(),
                TotalCount = pagedQueryResult.TotalCount
            });
        }

        /// <summary>
        /// Gets a single Campaign
        /// </summary>
        /// <param name="id"></param>
        [Route("{id}")]
        [AuthorizeRequest("Campaigns")]
        [ResponseType(typeof(CampaignModel))]
        public IHttpActionResult Get(Guid id)
        {
            var item = _campaignRepository.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<CampaignModel>(item);
            FillProgrammeNames(result);

            return Ok(result);
        }

        /// <summary>
        /// Gets list of all Campaigns belonging to the CampaignGroup 'group' or
        /// empty list '[]' if none in 'group'.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="groupByGroupName"></param>
        [Route("Group/{group}")]
        [AuthorizeRequest("Campaigns")]
        [ResponseType(typeof(IEnumerable<CampaignModel>))]
        public IHttpActionResult GetGroup(string group, bool groupByGroupName = false)
        {
            var campaignList = _campaignRepository.GetByGroup(group).ToList();
            if (groupByGroupName && campaignList.Count > 1)
            {
                var campaign = (Campaign)campaignList[0].Clone();
                campaign.TargetRatings = campaignList.Sum(x => x.TargetRatings);
                campaign.ActualRatings = campaignList.Sum(x => x.ActualRatings);

                campaign.SalesAreaCampaignTarget = new List<SalesAreaCampaignTarget>();
                campaign.SalesAreaCampaignTarget.AddRange(campaignList.SelectMany(c => c.SalesAreaCampaignTarget));

                if (campaignList.Select(c => c.ExternalId).Distinct().Count() != 1)
                {
                    campaign.ExternalId = String.Empty;
                }

                campaignList = new List<Campaign>() { campaign };
            }

            return Ok(_mapper.Map<List<CampaignModel>>(campaignList));
        }

        /// <summary>
        /// Gets list of all Campaigns belonging to the CampaignGroup 'group' or
        /// empty list '[]' if none in 'group'. Performs returns projections of
        /// sales area, daypart campaign ratings based on a scenarios recommendations
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <param name="group"></param>
        [Route("Group/{group}/scenario/{scenarioId}")]
        [AuthorizeRequest("Campaigns")]
        [ResponseType(typeof(IEnumerable<CampaignModel>))]
        public IHttpActionResult GetGroupWithScenarioRecommendations(Guid scenarioId, String group)
        {
            var campaignRefList = _mapper.Map<List<CampaignModel>>(_campaignRepository.GetByGroup(group));

            //clone to get a deep level copy as the campaignRefList will only contain a shallow copy;
            //the Clone() methods for all the objects inside the campaign object have all been amended so they will be a copy and not a reference
            var campaignList = new List<CampaignModel>();
            foreach (var campRefDoc in campaignRefList)
            {
                var campDoc = campRefDoc.Clone();
                campaignList.Add((CampaignModel)campDoc);
            }

            var scenarioRecommendations = _recommendationRepository.GetByScenarioId(scenarioId);
            scenarioRecommendations = scenarioRecommendations.Where(r => r.ExternalCampaignNumber.Contains(group));
            decimal recommendationsTotalRatingsForDayPart = 0;
            Dictionary<NodaTime.Duration, decimal> lengthRatings = new Dictionary<NodaTime.Duration, decimal>();
            List<string> salesAreasList = new List<string>();

            campaignList.ForEach(campaignDoc =>
            {
                campaignDoc.SalesAreaCampaignTarget.ForEach(salesAreaCampaignTarget =>
                {
                    salesAreaCampaignTarget.CampaignTargets.ForEach(campaignTarget =>
                    {
                        salesAreasList.Clear();
                        foreach (var item in salesAreaCampaignTarget.SalesAreaGroup.SalesAreas)
                        {
                            salesAreasList.Add(item);
                        }
                        campaignTarget.StrikeWeights.ForEach(strikeWeight =>
                        {
                            strikeWeight.DayParts.ForEach(dayPart =>
                            {
                                recommendationsTotalRatingsForDayPart = 0;     //reset at beginning of each day part

                                //get dictionary list of time lengths and ratings for this dayPart..
                                lengthRatings = CampaignProjectionProcessing.ProjectRatingsForCampaignDayPart(dayPart, scenarioRecommendations, strikeWeight.StartDate, strikeWeight.EndDate, salesAreasList);

                                //if there are some ratingsPredictions for this day part section then add them to the appropriate parts of the camapaign document to be returned to api call
                                if (lengthRatings.Count > 0)
                                {
                                    //update different totals of this campaign document using the array of length counts passed back from each day part....
                                    //individual lengths in salesAreaCampaignTarget totals and campaigndoc actual totals..
                                    foreach (var multi in salesAreaCampaignTarget.Multiparts)
                                    {
                                        foreach (var len in multi.Lengths)
                                        {
                                            foreach (var item in lengthRatings)
                                            {
                                                if (len == item.Key)
                                                {
                                                    multi.CurrentPercentageSplit += item.Value;
                                                    campaignDoc.ActualRatings += item.Value;
                                                }
                                            }
                                        }
                                    }

                                    //individual lengths in the strikeweights totals..
                                    int lenCnt = 0;
                                    foreach (var len in strikeWeight.Lengths)
                                    {
                                        foreach (var item in lengthRatings)
                                        {
                                            if (len.length == item.Key)
                                            {
                                                strikeWeight.Lengths[lenCnt].CurrentPercentageSplit = strikeWeight.Lengths[lenCnt].CurrentPercentageSplit + (int)item.Value;
                                                //keep a running total for where there are diffrent lengths in doc; not the case for Nine
                                                recommendationsTotalRatingsForDayPart += item.Value;
                                            }
                                        }
                                        lenCnt++;
                                    }

                                    //strikeweight totals, from running total above..
                                    strikeWeight.CurrentPercentageSplit += recommendationsTotalRatingsForDayPart;

                                    //daypart totals, from running total above..
                                    dayPart.CurrentPercentageSplit += recommendationsTotalRatingsForDayPart;
                                }
                            });
                        });
                    });
                });
            });
            return Ok(campaignList);
        }

        /// <summary>
        /// Creates a set of Campaigns
        /// </summary>
        [Route("")]
        [AuthorizeRequest("Campaigns")]
        public IHttpActionResult Post([FromBody] IEnumerable<CreateCampaign> campaigns)
        {
            if (campaigns is null || !campaigns.Any() || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            foreach (var c in campaigns)
            {
                try
                {
                    c.Validate();
                }
                catch (BreakTypeException ex)
                {
                    ModelState.AddModelError(
                        nameof(CreateCampaign.BreakType), BreakTypeValueErrorMessage(ex)
                        );
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*
             *********************************
             * Caveat: THIS LOOKS LIKE A BUG!
             *********************************
             * Why is the campaign validation ONLY done if the feature flag is on?
             */
            if (_featureManager.IsEnabled(nameof(ProductFeature.IncludeChannelGroupFileForOptimiser)))
            {
                try
                {
                    ValidateCampaigns(campaigns.ToList());
                    ValidateTargetRatings(campaigns);
                }
                catch (ArgumentException exception)
                    when (exception.ParamName == ReflectionUtilities.PropertyName<CreateCampaign>(c => c.IncludeRightSizer))
                {
                    // Case for 400 Bad Request Code for IncludeRightSizer value validation
                    return this.Error().BadRequest(ApiError.BadRequest(exception.Message));
                }
            }

            AddCampaign(campaigns);

            _campaignPassPrioritiesService.AddNewCampaignPassPrioritiesToAllScenariosInLibrary();

            return Ok();
        }

        /// <summary>Validates the campaigns to create have valid data.</summary>
        /// <param name="campaigns">The campaigns to validate.</param>
        /// <param name="validateCampaignPassPriority">If set to <c>true</c>
        /// validate campaign pass priority.</param>
        private void ValidateCampaigns(List<CreateCampaign> campaigns, bool validateCampaignPassPriority = true)
        {
            var validateDeliveryType = _featureManager.IsEnabled(nameof(ProductFeature.CampaignDeliveryType));

            // Required and Regex Validation
            campaigns.ForEach(c =>
            {
                if (c?.ProgrammesList != null)
                {
                    _mapper.Map<List<CampaignProgramme>>(c.ProgrammesList);
                }

                List<CampaignProgramme> campaignProgrammesPlacebo = null;

                Validation(c.ExternalId, c.Name, c.DemoGraphic, c.StartDateTime, c.EndDateTime, c.Product, c.DeliveryType,
                    c.TargetRatings, c.SalesAreaCampaignTarget, c.TimeRestrictions, c.ProgrammeRestrictions, campaignProgrammesPlacebo, c.CampaignGroup,
                    c.IncludeRightSizer, c.IncludeOptimisation, c.CampaignPassPriority, c.CampaignSpotMaxRatings, validateCampaignPassPriority,
                    validateDeliveryType);
            });

            if (!_demographicRepository.ValidateDemographics(
                campaigns.Select(c => c.DemoGraphic).ToList(),
                out List<string> invalidDemographic)
                )
            {
                var msg = String.Concat(
                    "Invalid Demographic in Campaigns: ",
                    invalidDemographic is null ?
                        String.Empty :
                        String.Join(",", invalidDemographic));

                throw new InvalidDataException(msg);
            }

            _productRepository.ValidateProductExternalIds(campaigns.Select(c => c.Product).ToList());

            var salesAreaCampaignTargets = campaigns
                .Where(p => p.SalesAreaCampaignTarget != null && p.SalesAreaCampaignTarget.Count > 0)
                .SelectMany(b => b.SalesAreaCampaignTarget)
                .ToList();

            if (salesAreaCampaignTargets.Any())
            {
                var salesAreas = salesAreaCampaignTargets
                     .Where(s => s.SalesAreaGroup != null && s.SalesAreaGroup.SalesAreas != null &&
                                 s.SalesAreaGroup.SalesAreas.Any())
                     .SelectMany(s => s.SalesAreaGroup.SalesAreas)
                     .ToList();

                _salesAreaRepository.ValidateSaleArea(salesAreas);
            }

            var programmeRestrictions = campaigns
                .Where(c => c.ProgrammeRestrictions != null && c.ProgrammeRestrictions.Count > 0)
                .SelectMany(c => c.ProgrammeRestrictions)
                .ToList();

            if (programmeRestrictions.Any())
            {
                var saleAreas = programmeRestrictions.Where(p => p.SalesAreas != null && p.SalesAreas.Any())
                    .SelectMany(p => p.SalesAreas)
                    .ToList();

                if (saleAreas.Any())
                {
                    _salesAreaRepository.ValidateSaleArea(saleAreas);
                }

                var categoryNames = programmeRestrictions
                    .Where(p => p.IsCategoryOrProgramme.Equals(CategoryOrProgramme.C.ToString(), StringComparison.OrdinalIgnoreCase))
                    ?.SelectMany(p => p.CategoryOrProgramme)
                    ?.ToList();

                if (categoryNames.Any() && !_programmeCategoryRepository.IsValid(categoryNames, out List<string> invalidCategory))
                {
                    var msg = String.Concat(
                        "Invalid Category Name: ",
                        invalidCategory is null ?
                            String.Empty
                            : String.Join(",", invalidCategory)
                        );

                    throw new InvalidDataException(msg);
                }
            }

            var timeRestrictions = campaigns
                .Where(c => c.TimeRestrictions != null && c.TimeRestrictions.Count > 0)
                .SelectMany(c => c.TimeRestrictions)
                .ToList();

            if (timeRestrictions.Any())
            {
                var saleAreas = timeRestrictions.Where(p => p.SalesAreas != null && p.SalesAreas.Any())
                    .SelectMany(p => p.SalesAreas)
                    .ToList();

                if (saleAreas.Any())
                {
                    _salesAreaRepository.ValidateSaleArea(saleAreas);
                }
            }
        }

        private void Validation(
            string externalId,
            string name,
            string demographic,
            DateTime startDateTime,
            DateTime endDateTime,
            string product,
            string deliveryType,
            decimal? targetRatings,
            List<model.External.Campaign.SalesAreaCampaignTargetViewModel> salesAreaCampaignTargets,
            List<TimeRestriction> timeRestrictions,
            List<ProgrammeRestriction> programmeRestrictions,
            List<CampaignProgramme> campaignProgrammes,
            string campaignGroup,
            string includeRightSizer,
            bool includeOptimisation,
            int campaignPassPriority,
            int campaignSpotMaxRatings,
            bool validateCampaignPassPriority,
            bool validateDeliveryType)
        {
            IValidation validation = new RequiredFieldValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo() {FieldName = "External Id", FieldToValidate = externalId},
                    new ValidationInfo() {FieldName = "Campaign Name", FieldToValidate = name},
                    new ValidationInfo() {FieldName = "Demographic", FieldToValidate = demographic},
                    new ValidationInfo() {FieldName = "Start Date Time", FieldToValidate = startDateTime},
                    new ValidationInfo() {FieldName = "End Date Time", FieldToValidate = endDateTime},
                    new ValidationInfo() {FieldName = "Product", FieldToValidate = product},
                    new ValidationInfo() {FieldName = "Target Ratings", FieldToValidate = targetRatings},
                    new ValidationInfo()
                    {
                        FieldName = "Sales Area Campaign Targets",
                        FieldToValidate = salesAreaCampaignTargets
                    },
                    new ValidationInfo() {FieldName = "Include Right Sizer", FieldToValidate = includeRightSizer}
                }
            };

            if (validateDeliveryType)
            {
                validation.Field.Add(new ValidationInfo() { FieldName = "Delivery Type", FieldToValidate = deliveryType });
            }

            validation.Execute();

            if (validateCampaignPassPriority)
            {
                ValidateCampaignPassPriority(includeOptimisation, campaignPassPriority);
            }

            if (!EnumUtilities.ToDescriptionList<IncludeRightSizer>().Select(value => value.ToUpper())
                .Contains(includeRightSizer.ToUpper()))
            {
                throw new ArgumentException($"Invalid right sizer value: ${includeRightSizer}",
                    ReflectionUtilities.PropertyName<CreateCampaign>(c => c.IncludeRightSizer));
            }

            if (validateDeliveryType && !Enum.IsDefined(typeof(CampaignDeliveryType), deliveryType))
            {
                throw new ArgumentException($"Invalid delivery type value: ${deliveryType}", ReflectionUtilities.PropertyName<CreateCampaign>(c => c.DeliveryType));
            }

            if (campaignGroup != null)
            {
                validation = new LengthValidation()
                {
                    Field = new List<ValidationInfo>()
                    {
                        new ValidationInfo()
                        {
                            ErrorMessage = "Campaign Group can't be more than 20 characters: " + campaignGroup,
                            FieldToValidate = campaignGroup,
                            LengthToCompare = 20,
                            Operator = Operator.LessThanEqual
                        }
                    }
                };
                validation.Execute();
            }

            validation = new CompareValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo()
                    {
                        ErrorMessage = "Campaign Spot Max Ratings should be greater than or equal to 0",
                        FieldToValidate = campaignSpotMaxRatings,
                        FieldToCompare = 0,
                        Operator = Operator.GreaterThanEqual
                    }
                }
            };
            validation.Execute();

            validation = new CompareValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo()
                    {
                        ErrorMessage = "Campaign start date should be less than or equal to end date",
                        FieldToValidate = startDateTime,
                        FieldToCompare = endDateTime,
                        Operator = Operator.LessThanEqual
                    }
                }
            };
            validation.Execute();

            if (salesAreaCampaignTargets.Any())
            {
                salesAreaCampaignTargets.ForEach(s => s.Validation(s.SalesAreaGroup, s.Multiparts, s.CampaignTargets,
                    startDateTime, endDateTime));
            }

            if (timeRestrictions != null && timeRestrictions.Any())
            {
                timeRestrictions.ForEach(t =>
                    t.RequiredFieldValidation(t.StartDateTime, t.EndDateTime, t.DowPattern, t.IsIncludeOrExclude));
                timeRestrictions.ForEach(t => t.RegexValidation(t.DowPattern, t.IsIncludeOrExclude));
                timeRestrictions.ForEach(t => t.CompareValidation(t.StartDateTime, t.EndDateTime));
            }

            if (programmeRestrictions != null && programmeRestrictions.Any())
            {
                programmeRestrictions.ForEach(p =>
                    p.RequiredFieldValidation(p.IsIncludeOrExclude, p.IsCategoryOrProgramme));
                programmeRestrictions.ForEach(p => p.RegexValidation(p.IsIncludeOrExclude, p.IsCategoryOrProgramme));
            }

            if (campaignProgrammes != null && campaignProgrammes.Any())
            {
                campaignProgrammes.ForEach(x =>
                    x.RequiredFieldValidation(x.IsCategoryOrProgramme));
                campaignProgrammes.ForEach(p => p.RegexValidation(p.IsCategoryOrProgramme));
            }
        }

        private void ValidateCampaignPassPriority(bool includeOptimisation, int campaignPassPriority)
        {
            int minimumCampaignPassPriority = (int)PassPriorityType.Exclude;
            int maximumCampaignPassPriority = (int)PassPriorityType.Include;

            string campaignPassPriorityerrorMessage = "Invalid CampaignPassPriority, CampaignPassPriority must be '0'  when IncludeOptimisation is false.";
            if (includeOptimisation)
            {
                campaignPassPriorityerrorMessage = $"Invalid CampaignPassPriority, Accepted values from {minimumCampaignPassPriority} " +
                                                   $"to {maximumCampaignPassPriority}";
            }

            IValidation validation = new RangeValidation()
            {
                Field = new List<ValidationInfo>()
                {
                    new ValidationInfo()
                    {
                        ErrorMessage =campaignPassPriorityerrorMessage,
                        FieldToValidate = campaignPassPriority,
                        MinimumValue = minimumCampaignPassPriority,
                        MaximumValue = maximumCampaignPassPriority
                    }
                }
            };
            validation.Execute();
        }

        /// <summary>
        /// Deletes a single Campaign using the supplied id
        /// </summary>
        /// <param name="id">The Campaign Id <see cref="Guid"/></param>
        /// <returns>Status Code 204 NoContent Result if successfull.</returns>
        /// <response code="204">
        /// Returns Status Code 204 NoContent Result if Deletion is successfull
        /// </response>
        /// <response code="400">
        /// Returns Status Code 400 BadRequest Result if the supplied Campaign
        /// id is invalid
        /// </response>
        /// <response code="404">
        /// Returns Status Code 404 NotFound Result if no matching Campaign is
        /// found for the supplied Campaign id
        /// </response>
        /// <response code="500">Returns a 500 Internal Error for any failures</response>
        [Route("{id}")]
        [AuthorizeRequest("Campaigns")]
        public async Task<IHttpActionResult> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Campaign Id");
            }

            var campaign = _campaignRepository.Get(id);

            if (campaign == null)
            {
                return NotFound();
            }

            await _campaignCleaner.ExecuteAsync(campaign.Id).ConfigureAwait(false);

            return this.NoContent();
        }

        /// <summary>
        /// Deletes Campaigns using the supplied externalRefs
        /// </summary>
        /// <param name="externalRefs">
        /// One or more Campaign ExternalIds as a comma seperated value in Query
        /// String (OR) One or more Campaign ExternalIds as one or more Query
        /// String values
        /// </param>
        /// <returns>Status Code 204 NoContent Result if successfull.</returns>
        /// <response code="204">
        /// Returns Status Code 204 NoContent Result if Deletion is successfull
        /// </response>
        /// <response code="400">
        /// Returns Status Code 400 BadRequest Result if the supplied Campaign
        /// externalRefs is invalid
        /// </response>
        /// <response code="404">
        /// Returns Status Code 404 NotFound Result if no matching Campaigns are
        /// found for the supplied Campaign externalRefs
        /// </response>
        /// <response code="500">Returns a 500 Internal Error for any failures</response>
        [HttpDelete]
        [Route("")]
        [AuthorizeRequest("Campaigns")]
        public async Task<IHttpActionResult> DeleteAsync(IEnumerable<string> externalRefs)
        {
            if (externalRefs == null || !externalRefs.Any(e => !String.IsNullOrWhiteSpace(e)))
            {
                return BadRequest("Invalid Campaign externalRefs");
            }

            var campaignExternalIdsToDelete = _campaignRepository.GetAllActiveExternalIds().Intersect(externalRefs
                .Where(e => !String.IsNullOrWhiteSpace(e))
                .Select(e => e.Trim())).ToArray();

            if (campaignExternalIdsToDelete.Length == 0)
            {
                return NotFound();
            }

            await _campaignCleaner.ExecuteAsync(campaignExternalIdsToDelete).ConfigureAwait(false);

            return this.NoContent();
        }

        /// <summary>
        /// Gets Campaign by external
        /// </summary>
        /// <param name="externalId"></param>
        [Route("externalref/{externalId}")]
        [AuthorizeRequest("Campaigns")]
        [ResponseType(typeof(CampaignModel))]
        public IHttpActionResult Get(string externalId)
        {
            var item = _campaignRepository.FindByRef(externalId).FirstOrDefault();

            if (item is null)
            {
                return NotFound();
            }

            var result = _mapper.Map<CampaignModel>(item);
            FillProgrammeNames(result);

            return Ok(result);
        }

        /// <summary>
        /// Delete all Campaigns
        /// </summary>
        [Route("DeleteAll")]
        [AuthorizeRequest("Campaigns")]
        public async Task<IHttpActionResult> DeleteAsync()
        {
            // Validate that we can delete
            _dataChangeValidator.ThrowExceptionIfAnyErrors(
                _dataChangeValidator.ValidateChange<Campaign>(
                    ChangeActions.Delete,
                    ChangeTargets.AllItems,
                    null
                    ));

            await _campaignCleaner.ExecuteAsync().ConfigureAwait(false);

            return this.NoContent();
        }

        /// <summary>
        /// Update the Campaign by externalId
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="command"></param>
        [Route("externalref/{externalId}")]
        [AuthorizeRequest("Campaigns")]
        [ResponseType(typeof(CampaignModel))]
        public IHttpActionResult Put([FromUri] string externalId, [FromBody] CreateCampaign command)
        {
            if (command is null || String.IsNullOrWhiteSpace(externalId) || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (externalId != command.ExternalId)
            {
                ModelState.AddModelError(nameof(CreateCampaign.ExternalId), "ExternalId does not match");
            }

            try
            {
                command.Validate();
            }
            catch (BreakTypeException ex)
            {
                ModelState.AddModelError(
                    nameof(CreateCampaign.BreakType), BreakTypeValueErrorMessage(ex)
                    );
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var campaign = _campaignRepository.FindByRef(externalId).FirstOrDefault();

            if (_featureManager.IsEnabled(nameof(ProductFeature.IncludeChannelGroupFileForOptimiser)))
            {
                try
                {
                    ValidateCampaigns(new List<CreateCampaign> { command }, campaign == null);
                }
                catch (ArgumentException exception)
                    when (exception.ParamName == ReflectionUtilities.PropertyName<CreateCampaign>(c => c.IncludeRightSizer)) // Case for 400 Bad Request Code for IncludeRightSizer value validation
                {
                    return this.Error().BadRequest(ApiError.BadRequest(exception.Message));
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            if (campaign is null)
            {
                AddCampaign(new List<CreateCampaign>() { command });
            }
            else
            {
                var newCampaign = _mapper.Map<Campaign>(command);

                campaign.Update(newCampaign);
                ApplyDeliveryCurrency(newCampaign);
                ApplyBookingPositionAndSequencing(newCampaign);
                _campaignRepository.Update(campaign);
            }

            _campaignRepository.SaveChanges();

            _campaignPassPrioritiesService.AddNewCampaignPassPrioritiesToAllScenariosInLibrary();

            return Ok(_mapper.Map<CampaignModel>(campaign));
        }

        private void AddCampaign(IEnumerable<CreateCampaign> campaigns)
        {
            var campaignData = campaigns.Select(c =>
            {
                var campaign = _mapper.Map<Campaign>(c);
                campaign.Id = Guid.NewGuid();
                campaign.CampaignPassPriority = campaign.CampaignPassPriority == 0 && campaign.IncludeOptimisation
                    ? (int)PassPriorityType.Include
                    : campaign.CampaignPassPriority;

                ApplyDeliveryCurrency(campaign);
                ApplyBookingPositionAndSequencing(campaign);

                return campaign;
            }).ToList();
            _campaignRepository.Add(campaignData);
            _campaignRepository.SaveChanges();
        }

        /// <summary>
        /// Updates/Creates a set of campaigns
        /// </summary>
        /// <param name="command"></param>
        [Route("")]
        [AuthorizeRequest("Campaigns")]
        public IHttpActionResult Put([FromBody] IEnumerable<CreateCampaign> command)
        {
            if (command is null || !command.Any() || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                UpsertCampaigns(command);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        private void UpdateCampaign(IEnumerable<CreateCampaign> updateCampaigns, IEnumerable<Campaign> campaigns)
        {
            if (updateCampaigns != null || updateCampaigns.Any())
            {
                foreach (CreateCampaign createCampaign in updateCampaigns)
                {
                    var campaign = campaigns.FirstOrDefault(c => c.ExternalId == createCampaign.ExternalId);
                    if (campaign != null)
                    {
                        var newCampaign = _mapper.Map<Campaign>(createCampaign);
                        campaign.Update(newCampaign);
                        ApplyDeliveryCurrency(campaign);
                        ApplyBookingPositionAndSequencing(campaign);
                        _campaignRepository.Update(campaign);
                    }
                }
                _campaignRepository.SaveChanges();
            }
        }

        private (IEnumerable<Campaign> Campaigns, IEnumerable<CreateCampaign> NewCampaigns, IEnumerable<CreateCampaign> ExistingCampaigns)
            GetCampaignsByExistence(IEnumerable<CreateCampaign> createCampaigns)
        {
            var campaignExternalIds = createCampaigns.Select(x => x.ExternalId);
            var campaigns = _campaignRepository.FindByRefs(campaignExternalIds.ToList());
            campaignExternalIds = campaigns.Select(x => x.ExternalId).Distinct();
            var groupedCreateCampaignsByExistence = createCampaigns.ToLookup(x => campaignExternalIds.Contains(x.ExternalId));
            var newCampaigns = groupedCreateCampaignsByExistence[false];
            var existingCampaigns = groupedCreateCampaignsByExistence[true];

            return (campaigns, newCampaigns, existingCampaigns);
        }

        private void UpsertCampaigns(IEnumerable<CreateCampaign> createCampaigns)
        {
            var splitCampaigns = GetCampaignsByExistence(createCampaigns);

            foreach (var c in createCampaigns)
            {
                try
                {
                    c.Validate();
                }
                catch (BreakTypeException ex)
                {
                    ModelState.AddModelError(
                        nameof(CreateCampaign.BreakType), BreakTypeValueErrorMessage(ex)
                        );
                }
            }

            if (!ModelState.IsValid)
            {
                return;
            }

            if (_featureManager.IsEnabled(nameof(ProductFeature.IncludeChannelGroupFileForOptimiser)))
            {
                try
                {
                    if (splitCampaigns.NewCampaigns != null && splitCampaigns.NewCampaigns.Any())
                    {
                        ValidateCampaigns(splitCampaigns.NewCampaigns.ToList());
                    }

                    if (splitCampaigns.ExistingCampaigns != null && splitCampaigns.ExistingCampaigns.Any())
                    {
                        ValidateCampaigns(splitCampaigns.ExistingCampaigns.ToList(), false);
                    }
                }
                catch (ArgumentException exception)
                    when (exception.ParamName == nameof(CreateCampaign.IncludeRightSizer))
                {
                    throw new InvalidDataException(exception.Message);
                }
            }

            AddCampaign(splitCampaigns.NewCampaigns);
            UpdateCampaign(splitCampaigns.ExistingCampaigns, splitCampaigns.Campaigns);

            _campaignPassPrioritiesService.AddNewCampaignPassPrioritiesToAllScenariosInLibrary();
        }

        private void FillProgrammeNames(CampaignModel campaign)
        {
            var programmeRestrictionViewModels = campaign.ProgrammeRestrictions
                .Where(pr => pr.IsCategoryOrProgramme == "P" && pr.CategoryOrProgrammeName is null);

            var categoryOrProgrammes = programmeRestrictionViewModels.SelectMany(x => x.CategoryOrProgramme)
                .Distinct()
                .ToList();

            var programmes = _programmeRepository.FindByExternal(categoryOrProgrammes);

            foreach (var programmeRestriction in programmeRestrictionViewModels)
            {
                var categoryOrProgramme = programmeRestriction.CategoryOrProgramme;

                programmeRestriction.CategoryOrProgrammeName = categoryOrProgramme.Select(cop =>
                {
                    var programme = programmes.FirstOrDefault(p => p.ExternalReference == cop);
                    return programme != null ? programme.ProgrammeName : cop;
                }).ToList();
            }
        }

        private void ApplyDeliveryCurrency(Campaign campaign)
        {
            campaign.DeliveryCurrency = campaign.DeliveryType == CampaignDeliveryType.Rating
                ? DeliveryCurrency.FixedRating
                : DeliveryCurrency.FixedSchedule;
        }

        private void ApplyBookingPositionAndSequencing(Campaign campaign)
        {
            foreach (var ct in campaign.SalesAreaCampaignTarget)
            {
                foreach (var m in ct.Multiparts)
                {
                    var count = 1;
                    foreach (var l in m.Lengths)
                    {
                        l.BookingPosition = -1;
                        l.Sequencing = count++;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Campaign filter group
        /// </summary>
        [HttpGet]
        [Route("group-filter")]
        [AuthorizeRequest("Campaigns")]
        [ResponseType(typeof(CampaignFilterGroupModel))]
        public IHttpActionResult GetCampaignFilterGroup([FromUri] CampaignFilterGroupQueryModel queryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var campaigns = _campaignRepository.GetAllFlat();
            return Ok(GenerateCampaignFilterGroup(campaigns, queryModel));
        }
    }
}
