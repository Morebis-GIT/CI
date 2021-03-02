using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Queries;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.core.Interfaces;
using xggameplan.core.Services;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;
using xggameplan.Validations.Passes;
using xggameplan.Validations.Scenarios;

namespace xggameplan.Controllers
{
    [RoutePrefix("Scenarios")]
    public class ScenariosController : ApiController
    {
        private readonly IRunRepository _runRepository;
        private readonly IScenarioRepository _scenarioRepository;
        private readonly IPassRepository _passRepository;
        private readonly ITenantSettingsRepository _tenantSettingsRepository;
        private readonly IIdentityGeneratorResolver _identityGeneratorResolver;
        private readonly IMapper _mapper;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IPassInspectorService _passInspectorService;

        public ScenariosController(
            IRunRepository runRepository,
            IScenarioRepository scenarioRepository,
            IPassRepository passRepository,
            ITenantSettingsRepository tenantSettingsRepository,
            IIdentityGeneratorResolver identityGeneratorResolver,
            IMapper mapper,
            ICampaignRepository campaignRepository,
            ISalesAreaRepository salesAreaRepository,
            IPassInspectorService passInspectorService)
        {
            _runRepository = runRepository;
            _scenarioRepository = scenarioRepository;
            _passRepository = passRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _identityGeneratorResolver = identityGeneratorResolver;
            _mapper = mapper;
            _campaignRepository = campaignRepository;
            _salesAreaRepository = salesAreaRepository;
            _passInspectorService = passInspectorService;
        }

        /// <summary>
        /// Gets a Scenario for the supplied id
        /// </summary>
        /// <param name = "id" >The Scenario Id <see cref="Guid"/></param >
        /// <returns> Status Code 200 OkNegotiatedContent Result if successfull.</returns>
        /// <response code="200">Returns a <see cref="ScenarioModel"/> with Status Code 200 OkNegotiatedContent Result
        ///                      if a matching Scenario is found for the supplied id</response>
        /// <response code="400">Returns Status Code 400 BadRequest Result if the supplied Scenario id is invalid</response>
        /// <response code="404">Returns Status Code 404 NotFound Result if no matching Scenario
        ///                      is found for the supplied Scenario id</response>
        /// <response code="500">Returns a 500 Internal Error for any failures</response>
        [Route("{id}")]
        [AuthorizeRequest("Scenarios")]
        [ResponseType(typeof(ScenarioModel))]
        public IHttpActionResult Get(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid Scenario id");
            }

            var item = _scenarioRepository.Get(id);
            if (item == null)
            {
                return NotFound();
            }

            var scenarioModel = Mappings.MapToScenarioModel(item, _scenarioRepository, _passRepository,
                _tenantSettingsRepository, _mapper);

            return Ok(scenarioModel);
        }

        /// <summary>
        /// Returns all scenarios, optionally allows filter on IsLibraried
        /// </summary>
        /// <param name="isLibraried">Filter on IsLibraried (null=Any)</param>
        [Route("")]
        [AuthorizeRequest("Scenarios")]
        public IEnumerable<ScenarioModel> GetAll([FromUri] bool? isLibraried = null)
        {
            // Get all scenarios
            var scenarios = _scenarioRepository.GetAll().OrderBy(s => s.CustomId).ToList();
            var scenarioModels = Mappings.MapToScenarioModels(scenarios, _scenarioRepository, _passRepository,
                _tenantSettingsRepository, _mapper);

            scenarioModels = (isLibraried.HasValue) ? scenarioModels.Where(s => s.IsLibraried == isLibraried.Value).ToList() : scenarioModels;

            return scenarioModels;
        }

        [HttpGet]
        [Route("search-library-items")]
        [AuthorizeRequest("Scenarios")]
        [ResponseType(typeof(SearchResultModel<ScenarioDigestListItemModel>))]
        public IHttpActionResult SearchLibraryItems([FromUri] SearchQueryModel query)
        {
            var searchQuery = new SearchQueryDto(query);

            IEnumerable<PassDigestListItem> passesContainingTitleFromQuery = new List<PassDigestListItem>();

            if (!string.IsNullOrEmpty(searchQuery.Title))
            {
                var passesSearchQuery = new SearchQueryDto(new SearchQueryModel()
                {
                    OrderBy = query.OrderBy,
                    OrderDirection = query.OrderDirection,
                    Title = query.Title,
                    Top = int.MaxValue,
                    Skip = 0
                });

                passesContainingTitleFromQuery = _passRepository.MinimalDataSearch(passesSearchQuery, false)
                    .Items
                    .Where(p =>
                        p.Name.IndexOf(searchQuery.Title, StringComparison.CurrentCultureIgnoreCase) >= 0
                     );
            }

            var scenariosWithMinimalData = _scenarioRepository.MinimalDataSearch(
                searchQuery,
                true,
                passesContainingTitleFromQuery
                    .Select(p => p.Id)
                );

            if (scenariosWithMinimalData.TotalCount > 0)
            {
                var defaultScenarioId = _tenantSettingsRepository.GetDefaultScenarioId();
                var defaultScenario = scenariosWithMinimalData.Items.FirstOrDefault(c => c.Id == defaultScenarioId);

                if (defaultScenario != null)
                {
                    defaultScenario.IsDefault = true;
                }

                var extractedPasses = ExtractMinimalPassesFromScenarios(scenariosWithMinimalData)
                                            .ToList();

                var passesFromDb = _passRepository.FindByIds(extractedPasses.Select(pass => pass.Id))
                                                  .Select(p => (p.Id, p.Name, p.DateModified))
                                                  .ToDictionary(p => p.Id);

                List<int> passIdsToRemove = new List<int>();
                UpdatePassesWithMinimalData(extractedPasses, passesFromDb, passIdsToRemove);

                RemoveDeletedPasses(scenariosWithMinimalData, passIdsToRemove);

                var minimalDataSearchResult = _mapper.Map<SearchResultModel<ScenarioDigestListItemModel>>(scenariosWithMinimalData);

                return Ok(minimalDataSearchResult);
            }
            else
            {
                return NotFound();
            }

            void UpdatePassesWithMinimalData(
                List<PassDigestListItem> passesWithMinimalData,
                Dictionary<int, (int Id, string Name, DateTime? DateModified)> passesFromDb,
                List<int> passIdsToRemove)
            {
                passesWithMinimalData.ForEach(minimalDataPass =>
                {
                    var pass = passesFromDb.FirstOrDefault(pass => pass.Key == minimalDataPass.Id).Value;

                    if (pass.Id == minimalDataPass.Id)
                    {
                        minimalDataPass.DateModified = pass.DateModified.Value;
                        minimalDataPass.Name = pass.Name;
                    }
                    else
                    {
                        passIdsToRemove.Add(minimalDataPass.Id);
                    }
                });
            }

            void RemoveDeletedPasses(SearchResultModel<ScenarioDigestListItem> scenariosWithMinimalData, List<int> passIdsToRemove)
            {
                passIdsToRemove.ForEach(id =>
                {
                    scenariosWithMinimalData.Items.ForEach(scenario =>
                    {
                        var passToRemove = scenario.Passes.Find(p => p.Id == id);
                        if (passToRemove != null)
                        {
                            scenario.Passes.Remove(passToRemove);
                        }
                    });
                });
            }

            IEnumerable<PassDigestListItem> ExtractMinimalPassesFromScenarios(SearchResultModel<ScenarioDigestListItem> searchScenarios)
            {
                var passes = new List<PassDigestListItem>();

                foreach (var scenario in searchScenarios.Items)
                {
                    passes.AddRange(scenario.Passes);
                }

                return passes;
            }
        }

        /// <summary>
        /// Deletes scenario
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("Scenarios")]
        public IHttpActionResult Delete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid scenario parameters");
            }

            Scenario scenario = _scenarioRepository.Get(id);
            if (scenario == null)
            {
                return this.NotFound();
            }

            // Check that scenario isn't referenced by a run
            var runs = _runRepository.GetByScenarioId(id);
            if (runs.Any())
            {
                return this.Error().BadRequest("Cannot delete a scenario because a run has a dependency on it");
            }

            // Check that scenario being deleted isn't the default
            TenantSettings tenantSettings = _tenantSettingsRepository.Get();
            if (scenario.Id == tenantSettings.DefaultScenarioId)
            {
                return this.Error().BadRequest("Cannot delete the default scenario");
            }

            _passRepository.RemoveByScenarioId(id);
            _scenarioRepository.Delete(id);

            return this.NoContent();
        }

        /// <summary>
        /// Creates a scenario using passed details
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("")]
        [AuthorizeRequest("Scenarios")]
        [ResponseType(typeof(ScenarioModel))]
        public IHttpActionResult PostScenario([FromBody] CreateScenarioModel command)
        {
            // Check empty name
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return this.Error().InvalidParameters("No name was entered");
            }

            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid scenario parameters");
            }

            // Check that scenario doesn't exist
            Scenario scenario = _scenarioRepository.Get(command.Id);
            if (scenario != null)
            {
                return this.Error().InvalidParameters("Scenario already exists");
            }

            // Check that scenario with such name doesn't exist
            scenario = _scenarioRepository.FindByName(command.Name, command.IsLibraried);
            if (scenario != null)
            {
                return this.Error().InvalidParameters("Scenario with such name already exists");
            }

            foreach (var pass in command.Passes)
            {
                if (_passInspectorService.InspectPassSalesAreaPriorities(
                    pass.PassSalesAreaPriorities,
                    out string errorMessage))
                {
                    return this.Error().InvalidParameters(errorMessage);
                }

                List<string> errorMessages = new List<string>();

                if (!BreakExclusionsValidations.DateTimeRangeIsValid(pass.BreakExclusions, _salesAreaRepository.GetAll(), out errorMessages))
                {
                    return this.Error().InvalidParameters(errorMessages);
                }
            }

            ValidateCampaignPriorityRounds(command.CampaignPriorityRounds);

            // Add scenario
            scenario = _mapper.Map<Scenario>(command);

            IdUpdater.SetIds(scenario, _identityGeneratorResolver);

            // Update Pass repository
            UpdatePassRepository(scenario, command, null);
            _passRepository.SaveChanges();

            // Add Campaign Pass Priorities to Scenario
            var campaignsResult = _campaignRepository.GetWithProduct(null);
            var usingAllCampaigns = campaignsResult.Items?.Any() == true ? campaignsResult.Items.ToList() : null;
            var forScenarioPasses = _passRepository.FindByIds(scenario.Passes.Select(p => p.Id)).ToList();

            var allCampaigns = usingAllCampaigns?.ToDictionary(x => x.ExternalId, x => x);
            CampaignPassPrioritiesServiceMapper.AmendCampaignPassPrioritiesForNewCampaigns(
                scenario,
                forScenarioPasses,
                allCampaigns,
                _passRepository,
                _mapper);

            // Validate
            ValidateForSave(scenario);

            scenario.DateUserModified = DateTime.UtcNow;

            // Add scenario
            _scenarioRepository.Add(scenario);
            _scenarioRepository.SaveChanges();   // Do not remove this, need to persist changes now so that we can return ScenarioModel

            return Ok(Mappings.MapToScenarioModel(scenario, _scenarioRepository, _passRepository,
                _tenantSettingsRepository, _mapper));
        }

        /// <summary>
        /// Updates a scenario
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("Scenarios")]
        [ResponseType(typeof(ScenarioModel))]
        public IHttpActionResult PutScenario([FromUri] Guid id, [FromBody] ScenarioModel command)
        {
            // Check empty name
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return this.Error().InvalidParameters("No name was entered");
            }

            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid scenario parameters");
            }

            var scenario = _scenarioRepository.Get(id);
            if (scenario == null)
            {
                return NotFound();
            }

            var scenario2 = _scenarioRepository.FindByName(command.Name, command.IsLibraried);
            if (scenario2 != null && scenario.Id != scenario2.Id)
            {
                return this.Error().InvalidParameters("Scenario with such name already exists");
            }

            var salesAreas = _salesAreaRepository.GetAll();

            foreach (var pass in command.Passes)
            {
                if (_passInspectorService.InspectPassSalesAreaPriorities(
                    pass.PassSalesAreaPriorities,
                    out string errorMessage))
                {
                    return this.Error().InvalidParameters(errorMessage);
                }

                List<string> errorMessages = new List<string>();
                if (!BreakExclusionsValidations.DateTimeRangeIsValid(pass.BreakExclusions, salesAreas, out errorMessages))
                {
                    return this.Error().InvalidParameters(errorMessages);
                }
            }

            // Get PassIds to delete
            var deletedPassIds = scenario.Passes.Select(p => p.Id).Except(command.Passes.Select(p => p.Id)).Distinct().ToList();

            ValidateCampaignPriorityRounds(command.CampaignPriorityRounds);

            Mappings.ApplyToScenario(scenario, command, _mapper);
            IdUpdater.SetIds(scenario, _identityGeneratorResolver);

            // Update Pass repository
            UpdatePassRepository(scenario, command, deletedPassIds);
            _passRepository.SaveChanges();

            // update scenario with any changes to passes in that scenario.campaignpasspriorities
            UpdateScenarioWithEditedPassList(scenario, deletedPassIds);

            // Validate, update and save scenario
            ValidateForSave(scenario);

            scenario.DateUserModified = DateTime.UtcNow;

            _scenarioRepository.Update(new List<Scenario> { scenario }); // Used bulk update method as a temporary solution as it is optimized to update nested collections
                                                                         // Will be changed in scope of performance optimizations phaze 2
            _scenarioRepository.SaveChanges();   // Do not remove this, need to persist changes now so that we can return ScenarioModel

            return Ok(Mappings.MapToScenarioModel(scenario, _scenarioRepository, _passRepository,
                _tenantSettingsRepository, _mapper));
        }

        private void UpdateScenarioWithEditedPassList(Scenario scenario, IEnumerable<int> deletedPassIds)
        {
            if (scenario.CampaignPassPriorities == null
                || !scenario.CampaignPassPriorities.Any())
            {
                return;
            }

            var oldPasses = _passRepository.FindByScenarioId(scenario.Id);

            var oldPassesIds = oldPasses.Select(p => p.Id);

            var newPasses = _passRepository.FindByIds(scenario.Passes.Select(pp => pp.Id).Where(p => !oldPassesIds.Contains(p)));

            var anyNewPasses = newPasses.Any();

            foreach (var campaignPassPriority in scenario.CampaignPassPriorities)
            {
                var passPriorities = campaignPassPriority.PassPriorities;

                passPriorities.RemoveAll(pp => deletedPassIds.Contains(pp.PassId));

                foreach (var passPriority in passPriorities)
                {
                    passPriority.PassName = oldPasses.First(p => p.Id == passPriority.PassId).Name;
                }

                if (anyNewPasses)
                {
                    var defaultCampaignPassPriority = campaignPassPriority.Campaign.DefaultCampaignPassPriority;

                    foreach (var newPass in newPasses)
                    {
                        passPriorities.Add(new PassPriority
                        {
                            PassId = newPass.Id,
                            PassName = newPass.Name,
                            Priority = defaultCampaignPassPriority
                        });
                    }
                }

                OrderPassPriorities(passPriorities, scenario.Passes);
            }
        }

        private static void OrderPassPriorities(List<PassPriority> passPriorities, IEnumerable<PassReference> passReferences)
        {
            var orderedPassPriorities = new List<PassPriority>();

            foreach (var passReference in passReferences)
            {
                var passPriority = passPriorities.First(pp => pp.PassId == passReference.Id);

                orderedPassPriorities.Add(passPriority);
            }

            passPriorities.Clear();

            passPriorities.AddRange(orderedPassPriorities);
        }

        private List<PassPriority> SaveListOfPassPriorities(List<PassPriority> listOfPassPriorities)
        {
            List<PassPriority> retList = new List<PassPriority>();
            for (int i = 0; i < listOfPassPriorities.Count(); i++)
            {
                PassPriority apassprioritry = new PassPriority()
                {
                    PassId = listOfPassPriorities[i].PassId,
                    PassName = listOfPassPriorities[i].PassName,
                    Priority = listOfPassPriorities[i].Priority
                };
                retList.Add(apassprioritry);
            }
            return retList;
        }

        /// <summary>
        /// Gets default scenario Id
        /// </summary>
        /// <returns></returns>
        [Route("default")]
        [AuthorizeRequest("Scenarios")]
        public IHttpActionResult GetDefault()
        {
            TenantSettings tenantSettings = _tenantSettingsRepository.Get();
            return Ok(new { ScenarioId = tenantSettings.DefaultScenarioId });
        }

        /// <summary>
        /// Sets/clears default scenario Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("default/{id}")]
        [AuthorizeRequest("Scenarios")]
        public IHttpActionResult PutDefault([FromUri] Guid id = default(Guid))
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid scenario parameters");
            }

            if (id != Guid.Empty)
            {
                // Check that scenario exists
                var scenario = _scenarioRepository.Get(id);
                if (scenario == null)
                {
                    return NotFound();
                }

                // Check that scenario isn't referenced by a run
                var run = _runRepository.FindByScenarioId(id);
                if (run != null)
                {
                    return this.Error().InvalidParameters("Scenario cannot be the default because it is used by a run");
                }
            }

            TenantSettings tenantSettings = _tenantSettingsRepository.Get();
            tenantSettings.DefaultScenarioId = id;
            _tenantSettingsRepository.AddOrUpdate(tenantSettings);

            return Ok();
        }

        /// <summary>
        /// Apply Default Campaign Pass Priorities to Default Scenario
        /// </summary>
        /// <param name="campaignPassPriorities">
        /// The collection of Campaign Pass Priorities to apply to the Default Scenario <see cref="IEnumerable{CreateCampaignPassPriorityModel}"/>
        /// </param>
        /// <returns>200 OK Response, when the Default Campaign Pass Priorities is successfully applied to Default Scenario </returns>
        /// <response code="200">Returns 200 OK Response, when the Default Campaign Pass Priorities is successfully applied to Default Scenario</response>
        /// <response code="404">Returns 404 NOT Found response, when no Default Scenario is found to perform the update</response>
        /// <response code="400">Returns 400 Bad Request, when the supplied model is invalid/fails validation</response>
        /// <response code="500">Returns a 500 Internal Error for any failures</response>
        /// <remarks>
        /// Sample request:
        ///
        /// [
        ///   {
        ///      "campaignExternalId": "15G1555937",
        ///      "passPriorities": [
        ///         {
        ///            "PassId": 8632,
        ///            "PassName": "BM Pass 01",
        ///            "Priority": "Priority3"
        ///         }
        ///      ]
        ///   },
        ///   {
        ///      "campaignExternalId": "15G1557669",
        ///      "passPriorities": [
        ///         {
        ///            "PassId": 8632,
        ///            "PassName": "BM Pass 01",
        ///            "Priority": "Priority1"
        ///         }
        ///      ]
        ///   },
        ///   {
        ///      "campaignExternalId": "60G1556954",
        ///      "passPriorities": [
        ///         {
        ///            "PassId": 8632,
        ///            "PassName": "BM Pass 01",
        ///            "Priority": "Priority2"
        ///         }
        ///      ]
        ///   }
        /// ]
        ///
        /// </remarks>
        [HttpPut, Route("default/campaignpasspriorities")]
        [AuthorizeRequest("Scenarios")]
        [ResponseType(typeof(IHttpActionResult))]
        public IHttpActionResult PutDefaultCampaignPassPriorities([FromBody] IEnumerable<CreateCampaignPassPriorityModel> campaignPassPriorities)
        {
            if (campaignPassPriorities == null || !campaignPassPriorities.Any() || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var defaultScenarioId = _tenantSettingsRepository.GetDefaultScenarioId();
            Scenario defaultScenario;
            if (defaultScenarioId == Guid.Empty || (defaultScenario = _scenarioRepository.Get(defaultScenarioId)) == null)
            {
                return NotFound();
            }

            //get all the campaigns with product info. this will be used to validate CampaignPassPriorities and also to store Campaign Pass Priorities
            var campaignsResult = _campaignRepository.GetWithProduct(null);
            var allCampaigns = campaignsResult.Items != null && campaignsResult.Items.Any() ? campaignsResult.Items.ToList() : null;

            //validate the campaignPassPriorities and send BadRequest if any validation fails
            var validationErrorMsg = ValidateCampaignPassPriorities(campaignPassPriorities, defaultScenario.Passes, allCampaigns);
            if (!string.IsNullOrWhiteSpace(validationErrorMsg))
            {
                return BadRequest(validationErrorMsg);
            }

            ProcessCampaignPassPrioritiesAndApplyToScenario(defaultScenario, campaignPassPriorities.ToList(), allCampaigns);

            _scenarioRepository.Update(new[] { defaultScenario });
            _scenarioRepository.SaveChanges();

            return Ok();
        }

        private void ProcessCampaignPassPrioritiesAndApplyToScenario(Scenario existingScenario, List<CreateCampaignPassPriorityModel> campaignPassPriorities,
                                                                     List<CampaignWithProductFlatModel> allCampaigns)
        {
            // this check is already done in the validation but if incase this method is called from elsewhere
            // if contains any CampaignPassPriorities then reset the pass ids and names with the information from the existingScenario passes
            if (campaignPassPriorities != null && campaignPassPriorities.Any())
            {
                // get the pass deatils for the existing Scenario passes
                var existingScenarioPassIds = existingScenario.Passes.Select(p => p.Id).ToList();
                var existingScenarioPasses = _passRepository.FindByIds(existingScenarioPassIds).ToList();
                var numberofPassesInScenario = existingScenarioPassIds.Count;

                campaignPassPriorities.ForEach(c =>
                 {
                     // this check is already done in the validation but if incase this method is called from elsewhere
                     // hence, an extra check to ensure that the scenario and campaignPassPriorities contains the same number of passes
                     if (c.PassPriorities.Count == numberofPassesInScenario)
                     {
                         // priorities are assigned passes in the order it exists in the scenario
                         for (int i = 0; i < numberofPassesInScenario; i++)
                         {
                             c.PassPriorities[i].PassId = existingScenario.Passes[i].Id;
                             c.PassPriorities[i].PassName = existingScenarioPasses.Where(p => p.Id == c.PassPriorities[i].PassId)
                                                                                  .Select(p => p.Name).FirstOrDefault();
                         }
                     }
                 });

                // create the CampaignPassPriorities for the campaigns which are not in the campaignPassPriorities
                var campaignPassPrioritiesForMissingCampaigns = CreateCampaignPassPriorities(allCampaigns, campaignPassPriorities, existingScenarioPasses);
                if (campaignPassPrioritiesForMissingCampaigns != null && campaignPassPrioritiesForMissingCampaigns.Any())
                {
                    campaignPassPriorities.AddRange(campaignPassPrioritiesForMissingCampaigns);
                }

                //apply Campaign Pass Priorities to existing Scenario
                existingScenario.CampaignPassPriorities =
                    CampaignPassPrioritiesServiceMapper.MapToCampaignPassPriorities(
                        campaignPassPriorities,
                        allCampaigns,
                        _mapper);
            }
        }

        private List<CreateCampaignPassPriorityModel> CreateCampaignPassPriorities(List<CampaignWithProductFlatModel> usingAllCampaigns,
                                                                                   IEnumerable<CreateCampaignPassPriorityModel> excludingExistingCampaignPassPriorities,
                                                                                   List<Pass> forScenarioPasses)
        {
            List<CreateCampaignPassPriorityModel> campaignPassPrioritiesForNewCampaigns = new List<CreateCampaignPassPriorityModel>();

            var excludingExternalIdList = (excludingExistingCampaignPassPriorities != null && excludingExistingCampaignPassPriorities.Any()) ?
                                           excludingExistingCampaignPassPriorities.Select(a => a.CampaignExternalId) : null;

            //get the new campaigns which are not in the CampaignPassPriorities
            var campaignsNotInExistingCampaignPassPriorities = usingAllCampaigns != null ?
                                                               usingAllCampaigns.Where(c => excludingExternalIdList == null ||
                                                               (excludingExternalIdList != null &&
                                                               !excludingExternalIdList.Contains(c.ExternalId))).ToList()
                                                               : null;

            //create CampaignPassPriorities with new campaigns for passes in the scenario
            if (campaignsNotInExistingCampaignPassPriorities != null && campaignsNotInExistingCampaignPassPriorities.Any())
            {
                campaignPassPrioritiesForNewCampaigns = ProduceCreateCampaignPassPriorityModels(campaignsNotInExistingCampaignPassPriorities, forScenarioPasses);
            }

            return campaignPassPrioritiesForNewCampaigns;
        }

        private List<CreateCampaignPassPriorityModel> ProduceCreateCampaignPassPriorityModels(List<CampaignWithProductFlatModel> forCampaigns, List<Pass> withPasses)
        {
            List<CreateCampaignPassPriorityModel> result = null;

            if (forCampaigns != null && forCampaigns.Any() && withPasses != null && withPasses.Any())
            {
                result = CampaignPassPrioritiesServiceMapper.CreateCampaignPassPriorityModels(
                    forCampaigns,
                    withPasses,
                    _mapper);
            }

            return result;
        }

        /// <summary>
        /// Validates for saving scenario
        /// </summary>
        /// <param name="scenario"></param>
        private void ValidateForSave(Scenario scenario)
        {
            // Basic validation
            Scenario.ValidateForSave(scenario);

            // Prevent change if scenario scheduled, running or completed
            var runsForScenarioId = _runRepository.GetByScenarioId(scenario.Id);
            if (runsForScenarioId.Any(r => r.Scenarios.Any(s => s.IsScheduledOrRunning)))
            {
                throw new Exception("Cannot modify a scenario while it is scheduled or running");
            }
            if (runsForScenarioId.Any(r => r.Scenarios.Any(s => s.IsCompleted)))
            {
                throw new Exception("Cannot modify a scenario that is completed");
            }

            // Get scenarios & passes
            var scenariosWithPassIds = _scenarioRepository.GetScenariosWithPassId();

            // Check passes
            foreach (var passReference in scenario.Passes)
            {
                if (passReference.Id > 0 && scenario.Passes.Where(p => p.Id == passReference.Id).ToList().Count > 1)
                {
                    throw new Exception("Scenario cannot contain multiple instances of the same pass");
                }

                //check that pass names are not the same as scenario names
                if (_passRepository.FindByIds(scenario.Passes.Select(k => k.Id)).Any(p => p.Name.ToLower().Trim() == scenario.Name.ToLower().Trim()))
                {
                    throw new Exception("Pass inside the scenario has same name as the scenario itself");
                }

                // Check that no other scenarios are linked to this pass
                var otherScenarioIdsForPassId = scenariosWithPassIds.Where(swp => swp.PassId == passReference.Id && swp.ScenarioId != scenario.Id).Select(swp => swp.ScenarioId).Distinct();
                if (otherScenarioIdsForPassId.Any())
                {
                    var otherScenario = _scenarioRepository.Get(otherScenarioIdsForPassId.First());
                    throw new Exception(string.Format("Pass is already linked to scenario {0}", otherScenario.Name));
                }
            }
        }

        private void ValidateCampaignPriorityRounds(CampaignPriorityRoundsModel campaignPriorityRounds)
        {
            campaignPriorityRounds?.PopulateRoundNumbers();

            var validationResult = CampaignPriorityRoundsModelValidations.ValidateCampaignPriorityRounds(campaignPriorityRounds);

            if (!string.IsNullOrWhiteSpace(validationResult))
            {
                throw new Exception($"Scenario {nameof(CampaignPriorityRoundsModel)} contains error: {validationResult}");
            }
        }

        /// <summary>
        /// Updates Pass repository for passes
        /// </summary>
        /// <param name="scenario"></param>
        /// <param name="command"></param>
        /// <param name="deletedPassIds"></param>
        private void UpdatePassRepository(Scenario scenario, CreateScenarioModel command, List<int> deletedPassIds)
        {
            List<Pass> updatedPasses = new List<Pass>();
            List<Pass> newPasses = new List<Pass>();

            // Get changes for Pass repository
            for (int passIndex = 0; passIndex < scenario.Passes.Count; passIndex++)
            {
                PassReference passReference = scenario.Passes[passIndex];
                PassModel passModel = command.Passes[passIndex];
                Pass pass = passReference.Id == 0 ? null : _passRepository.Get(passReference.Id);
                if (pass == null)   // New pass
                {
                    pass = _mapper.Map<Pass>(passModel);
                    pass.Id = passReference.Id;     // Set in previous call to IdUpdater.SetIds
                    Pass.ValidateForSave(pass);
                    newPasses.Add(pass);
                }
                else    // Update pass
                {
                    Mappings.ApplyToPass(pass, passModel, _mapper);
                    Pass.ValidateForSave(pass);
                    updatedPasses.Add(pass);
                }
            }

            // Add/update
            updatedPasses.ForEach(currentPass => _passRepository.Update(currentPass));
            newPasses.ForEach(currentPass => _passRepository.Add(currentPass));

            // Delete passes
            if (deletedPassIds != null && deletedPassIds.Any())
            {
                _passRepository.Remove(deletedPassIds);
            }
        }

        /// <summary>
        /// Updates Pass repository for passes
        /// </summary>
        /// <param name="scenario"></param>
        /// <param name="command"></param>
        /// <param name="deletedPassIds"></param>
        private void UpdatePassRepository(Scenario scenario, ScenarioModel command, List<int> deletedPassIds)
        {
            var updatedPasses = new List<Pass>();
            var newPasses = new List<Pass>();

            // Get changes for Pass repository
            for (int passIndex = 0; passIndex < scenario.Passes.Count; passIndex++)
            {
                PassReference passReference = scenario.Passes[passIndex];
                PassModel passModel = command.Passes[passIndex];
                Pass pass = passReference.Id == 0 ? null : _passRepository.Get(passReference.Id);
                if (pass == null)   // New pass
                {
                    pass = _mapper.Map<Pass>(passModel);
                    pass.Id = passReference.Id;     // Set in previous call to IdUpdater.SetIds
                    Pass.ValidateForSave(pass);
                    newPasses.Add(pass);
                }
                else    // Update pass
                {
                    Mappings.ApplyToPass(pass, passModel, _mapper);
                    Pass.ValidateForSave(pass);
                    updatedPasses.Add(pass);
                }
            }

            // Add/update
            updatedPasses.ForEach(currentPass => _passRepository.Update(currentPass));
            newPasses.ForEach(currentPass => _passRepository.Add(currentPass));

            // Delete passes
            if (deletedPassIds != null && deletedPassIds.Any())
            {
                _passRepository.Remove(deletedPassIds);
            }
        }

        private string ValidateCampaignPassPriorities(IEnumerable<CreateCampaignPassPriorityModel> campaignPassPriorities, List<PassReference> scenarioPasses, List<CampaignWithProductFlatModel> allCampaigns)
        {
            var errorMsgBuilder = new StringBuilder(string.Empty);

            if (allCampaigns == null || !allCampaigns.Any())
            {
                return "No Campaigns found in the DB. Cannot add Default CampaignPassPriorities.";
            }

            if (scenarioPasses == null || !scenarioPasses.Any())
            {
                return "Default Scenario contains no Pass. Cannot add Default CampaignPassPriorities.";
            }

            if (campaignPassPriorities != null && campaignPassPriorities.Any())
            {
                //check campaignPassPriorities has correct number of PassPriorities
                var noOfPassesInScenario = scenarioPasses.Count;
                if (campaignPassPriorities.Any(c => c.PassPriorities == null || !c.PassPriorities.Any() ||
                                                    c.PassPriorities.Count() != noOfPassesInScenario))
                {
                    return $"Default Scenario contains {noOfPassesInScenario} Pass. CampaignPassPriorities for each Campaign should also contain PassPriorities for {noOfPassesInScenario} pass.";
                }

                //check CampaignExternalId
                if (campaignPassPriorities.Any(cpp => string.IsNullOrWhiteSpace(cpp.CampaignExternalId)))
                {
                    return "CampaignExternalId is required for each Campaign in CampaignPassPriorities";
                }

                var allCampaignsExternalIds = allCampaigns.Select(c => c.ExternalId).ToList();
                var campaignPassPrioritiesWithInvalidCampaignsExternalIds = campaignPassPriorities.Where(cpp => !string.IsNullOrWhiteSpace(cpp.CampaignExternalId) &&
                                                                            !allCampaignsExternalIds.Contains(cpp.CampaignExternalId)).ToList();
                if (campaignPassPrioritiesWithInvalidCampaignsExternalIds != null && campaignPassPrioritiesWithInvalidCampaignsExternalIds.Any())
                {
                    foreach (var cpp in campaignPassPrioritiesWithInvalidCampaignsExternalIds)
                    {
                        errorMsgBuilder.AppendLine($"CampaignExternalId: {cpp.CampaignExternalId} is not valid.");
                    }
                }
            }

            return errorMsgBuilder.ToString();
        }
    }
}
