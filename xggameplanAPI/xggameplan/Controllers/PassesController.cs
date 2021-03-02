using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Passes.Queries;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.core.Interfaces;
using xggameplan.core.Validators;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;
using xggameplan.Validations.Passes;

namespace xggameplan.Controllers
{
    [RoutePrefix("Passes")]
    public class PassesController : ApiController
    {
        private readonly IScenarioRepository _scenarioRepository;
        private readonly IPassRepository _passRepository;
        private readonly IIdentityGeneratorResolver _identityGeneratorResolver;
        private readonly IMapper _mapper;
        private readonly IDataManipulator _dataManipulator;
        private readonly ISalesAreaRepository _salesAreaRepository;
        private readonly IPassInspectorService _passInspectorService;

        public PassesController(
            IScenarioRepository scenarioRepository,
            IPassRepository passRepository,
            IMapper mapper,
            IDataManipulator dataManipulator,
            ISalesAreaRepository salesAreaRepository,
            IIdentityGeneratorResolver identityGeneratorResolver,
            IPassInspectorService passInspectorService)
        {
            _scenarioRepository = scenarioRepository;
            _passRepository = passRepository;
            _identityGeneratorResolver = identityGeneratorResolver;
            _mapper = mapper;
            _dataManipulator = dataManipulator;
            _salesAreaRepository = salesAreaRepository;
            _passInspectorService = passInspectorService;
        }

        /// <summary>
        /// Returns passes by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("Passes")]
        [ResponseType(typeof(PassModel))]
        public IHttpActionResult Get(int id)
        {
            var item = _passRepository.Get(id);
            if (item is null)
            {
                return NotFound();
            }
            return Ok(Mappings.MapToPassModel(item, _passRepository, _mapper));
        }

        /// <summary>
        /// Returns all passes, optionally allows filter on IsLibraried
        /// </summary>
        /// <param name="isLibraried">Filter on IsLibraried (null=Any)</param>
        [Route("")]
        [AuthorizeRequest("Passes")]
        public IEnumerable<PassModel> GetAll([FromUri] bool? isLibraried = null)
        {
            var passes = _passRepository.GetAll().OrderBy(p => p.Id).ToList();

            var passModels = Mappings.MapToPassModels(passes, _passRepository, _mapper);

            return (isLibraried == null) ? passModels : passModels.Where(p => p.IsLibraried == isLibraried.Value);
        }

        [HttpGet]
        [Route("search-library-items")]
        [AuthorizeRequest("Passes")]
        [ResponseType(typeof(SearchResultModel<PassDigestListItem>))]
        public IHttpActionResult SearchLibraryItems([FromUri] SearchQueryModel query)
        {
            var searchQuery = new SearchQueryDto(query);

            var passesWithMinimalData = _passRepository.MinimalDataSearch(
                searchQuery,
                true
                );

            if (passesWithMinimalData.TotalCount > 0)
            {
                return Ok(passesWithMinimalData);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Deletes pass
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("Passes")]
        public IHttpActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid pass parameters");
            }

            // Check that pass exists
            Pass pass = _passRepository.Get(id);
            if (pass == null)
            {
                return NotFound();
            }

            // Get scenarios that reference pass
            var scenarios = _scenarioRepository.GetByPassId(id);
            if (scenarios.Any())
            {
                return this.Error().BadRequest("Cannot delete a pass because a scenario has a dependency on it");
            }

            _passRepository.Delete(id);
            _passRepository.SaveChanges();
            return this.NoContent();
        }

        /// <summary>
        /// Creates a pass using passed details
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("")]  // TODO: Remove this, added to stop clash with internal route
        [AuthorizeRequest("Passes")]
        [ResponseType(typeof(PassModel))]
        public IHttpActionResult Post([FromBody] CreatePassModel command)
        {
            // Check empty name
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return this.Error().InvalidParameters("Pass name must be set");
            }

            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid pass parameters");
            }

            // Check that pass doesn't exist
            Pass pass = _passRepository.Get(command.Id);
            if (pass != null)
            {
                return this.Error().InvalidParameters("Pass already exists");
            }

            // Check that pass with such name doesn't exist
            pass = _passRepository.FindByName(command.Name, command.IsLibraried);
            if (pass != null)
            {
                return this.Error().InvalidParameters("Pass with such name already exists");
            }

            if (_passInspectorService.InspectPassSalesAreaPriorities(
                command.PassSalesAreaPriorities,
                out string errorMessage))
            {
                return this.Error().InvalidParameters(errorMessage);
            }

            List<string> breakExclusionsErrorMessages = new List<string>();
            if (!BreakExclusionsValidations.DateTimeRangeIsValid(command.BreakExclusions, _salesAreaRepository.GetAll(), out breakExclusionsErrorMessages))
            {
                return this.Error().InvalidParameters(breakExclusionsErrorMessages);
            }

            pass = _mapper.Map<Pass>(command);
            IdUpdater.SetIds(pass, _identityGeneratorResolver);

            // Validate
            ValidateForSave(pass);

            // Save
            _passRepository.Add(pass);
            _passRepository.SaveChanges();   // Do not remove this, need to persist changes now so that we can return PassModel

            return Ok(Mappings.MapToPassModel(pass, _passRepository, _mapper));
        }

        /// <summary>
        /// Updates a pass
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("Passes")]
        [ResponseType(typeof(PassModel))]
        public IHttpActionResult Put([FromUri] int id, [FromBody] PassModel command)
        {
            // Check empty name
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return this.Error().InvalidParameters("No name was entered");
            }

            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid pass parameters");
            }

            var pass = _passRepository.Get(id);
            if (pass == null)
            {
                return NotFound();
            }

            var pass2 = _passRepository.FindByName(command.Name, command.IsLibraried);
            if (pass2 != null && pass.Id != pass2.Id)
            {
                return this.Error().InvalidParameters("Pass with such name already exists");
            }

            if (_passInspectorService.InspectPassSalesAreaPriorities(
                command.PassSalesAreaPriorities,
                out string errorMessage))
            {
                return this.Error().InvalidParameters(errorMessage);
            }

            List<string> breakExclusionsErrorMessages = new List<string>();
            if (!BreakExclusionsValidations.DateTimeRangeIsValid(command.BreakExclusions, _salesAreaRepository.GetAll(), out breakExclusionsErrorMessages))
            {
                return this.Error().InvalidParameters(breakExclusionsErrorMessages);
            }

            Mappings.ApplyToPass(pass, command, _mapper);
            IdUpdater.SetIds(pass, _identityGeneratorResolver);

            // Validate
            ValidateForSave(pass);

            // Save
            _passRepository.Update(pass);
            _passRepository.SaveChanges();   // Do not remove this, need to persist changes now so that we can return PassModel

            return Ok(Mappings.MapToPassModel(pass, _passRepository, _mapper));
        }

        /// <summary>
        /// Creates test data
        /// </summary>
        /// <returns></returns>
        /// <param name="numberOfPasses">Number of passes</param>
        /// <param name="cloneFromPassId">Pass to clone from (0=First PassID)</param>
        [Route("devtests/createtestdata")]
        [AuthorizeRequest("Passes")]
        //[ResponseType(typeof(PassModel))]
        public IHttpActionResult PostCreateTestData([FromUri] int numberOfPasses, int cloneFromPassId = 0)
        {
            var passes = _passRepository.GetAll().OrderBy(p => p.Id).ToList();
            if (!passes.Any())
            {
                return this.Error().UnknownError("No passes to clone from");
            }
            var templatePass = cloneFromPassId == 0 ? passes.First() : passes.FirstOrDefault(p => p.Id == cloneFromPassId);
            if (templatePass == null)
            {
                return this.Error().InvalidParameters("Pass to clone from does not exist");
            }
            if (numberOfPasses < 1)
            {
                return this.Error().InvalidParameters("Number of passes to create is invalid");
            }
            else if (numberOfPasses > 100)
            {
                return this.Error().InvalidParameters("Number of passes to create is too large");
            }

            // Create new passes from
            var newPasses = new List<Pass>();
            for (int passIndex = 0; passIndex < numberOfPasses; passIndex++)
            {
                var pass = (Pass)templatePass.Clone();
                pass.Id = 0;
                IdUpdater.SetIds(pass, _identityGeneratorResolver);
                pass.Name = $"Pass {pass.Id}";
                newPasses.Add(pass);
            }

            _passRepository.Add(newPasses);
            _passRepository.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Validates for saving pass
        /// </summary>
        /// <param name="pass"></param>
        private void ValidateForSave(Pass pass)
        {
            // Basic validation
            Pass.ValidateForSave(pass, _salesAreaRepository.GetAll());
            // Parent naming Validation
            Pass.ValidateScenarioNamingUniqueness(pass, _scenarioRepository.GetByPassId(pass.Id));
        }

        /// <summary>
        /// Returns passes matching the search criteria, supports paging
        /// </summary>
        [Route("search")]
        [AuthorizeRequest("Passes")]
        [ResponseType(typeof(PagedQueryResult<PassModel>))]
        public IHttpActionResult GetSearch([FromUri(Name = "")] PassSearchQueryModel queryModel)
        {
            if (queryModel == null || !ModelState.IsValid)
            {
                return this.Error().InvalidParameters();
            }

            // Search
            var freeTextMatchRules = new StringMatchRules(StringMatchHowManyWordsToMatch.AllWords, StringMatchWordOrders.AnyOrder, StringMatchWordComparisons.ContainsWord, false, new Char[] { ' ', ',' }, null);
            var passes = _passRepository.Search(queryModel, freeTextMatchRules);

            // Map to model
            var passModels = Mappings.MapToPassModels(passes.Items.ToList(), _passRepository, _mapper);
            var results = new PagedQueryResult<PassModel>(passes.TotalCount, passModels);
            return Ok(results);
        }
    }
}
