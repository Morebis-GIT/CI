using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using xggameplan.Errors;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;

namespace xggameplan.Controllers
{
    [RoutePrefix("RuleTypes")]
    public class RuleTypesController : ApiController
    {
        private readonly IRuleTypeRepository _ruleTypeRepository;
        private readonly IRuleRepository _ruleRepository;
        private readonly IAutopilotRuleRepository _autopilotRuleRepository;
        private readonly IFlexibilityLevelRepository _flexibilityLevelRepository;
        private readonly IMapper _mapper;

        public RuleTypesController(IRuleTypeRepository ruleTypeRepository, IRuleRepository ruleRepository,
            IAutopilotRuleRepository autopilotRuleRepository, IFlexibilityLevelRepository flexibilityLevelRepository,
            IMapper mapper)
        {
            _ruleTypeRepository = ruleTypeRepository;
            _ruleRepository = ruleRepository;
            _autopilotRuleRepository = autopilotRuleRepository;
            _flexibilityLevelRepository = flexibilityLevelRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns RuleType by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("RuleTypes")]
        [ResponseType(typeof(RuleTypeModel))]
        public IHttpActionResult Get(int id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid rule type parameter");
            }

            var item = _ruleTypeRepository.Get(id);
            if (item is null)
            {
                return NotFound();
            }

            return Ok(Mappings.MapToRuleTypeModel(item, _ruleRepository, _mapper));
        }

        /// <summary>
        /// Returns all Rule types
        /// </summary>
        [Route("")]
        [AuthorizeRequest("RuleTypes")]
        public IEnumerable<RuleTypeModel> GetAll()
        {
            var ruleTypes = _ruleTypeRepository.GetAll().OrderBy(p => p.Id).ToList();
            return Mappings.MapToRuleTypeModel(ruleTypes, _ruleRepository, _mapper);
        }

        /// <summary>
        /// Updates a RuleType
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("RuleTypes")]
        [ResponseType(typeof(RuleTypeModel))]
        public IHttpActionResult Put([FromUri] int id, [FromBody] RuleTypeModel command)
        {
            if (command == null)
            {
                return BadRequest("Entity can not be null");
            }

            if (command.Id != id)
            {
                return BadRequest("Model id does not match");
            }

            if (!ModelState.IsValid || command.Rules == null || !command.Rules.Any())
            {
                return this.Error().InvalidParameters("Invalid rule type parameters");
            }

            var targetRuleType = _ruleTypeRepository.Get(id);
            if (targetRuleType is null)
            {
                return NotFound();
            }

            // Update rule type
            targetRuleType.Name = command.Name;

            _ruleTypeRepository.Update(targetRuleType);
            _ruleTypeRepository.SaveChanges();

            var rules = _ruleRepository.FindByRuleTypeId(targetRuleType.Id);

            var deletedRuleIds = rules.Where(r => command.Rules.All(cr => r.Id != cr.Id))
                .Select(r => new {r.Id, r.RuleId})
                .ToList();

            var maxRuleId = command.Rules.Max(r => r.RuleId) + 1;

            var newRuleIds = new List<int>();

            // Upsert rules per rule type
            foreach (var ruleItem in command.Rules)
            {

                var rule = _ruleRepository.Get(ruleItem.Id);
                if (rule != null)
                {
                    rule.Description = ruleItem.Description;
                    rule.InternalType = ruleItem.InternalType;
                    rule.Type = ruleItem.Type;
                    _ruleRepository.Update(rule);
                }
                else
                {
                    rule = _mapper.Map<Rule>(ruleItem);
                    rule.Id = 0;
                    rule.RuleTypeId = id;
                    rule.RuleId = maxRuleId++;
                    _ruleRepository.Add(rule);

                    if (targetRuleType.AllowedForAutopilot && !targetRuleType.IsCustom)
                    {
                        newRuleIds.Add(rule.RuleId);
                    }
                }
            }
            _ruleRepository.SaveChanges();

            // Delete rules only for custom rule types
            if (deletedRuleIds.Any())
            {
                deletedRuleIds.ForEach(r => _ruleRepository.Delete(r.Id));
            }

            _ruleRepository.SaveChanges();

            if (targetRuleType.AllowedForAutopilot && !targetRuleType.IsCustom)
            {
                UpdateAutopilotRulesRepository(targetRuleType.Id, newRuleIds, deletedRuleIds.Select(r => r.RuleId).ToList());
            }

            var ruleType = _ruleTypeRepository.Get(id);

            return Ok(Mappings.MapToRuleTypeModel(ruleType, _ruleRepository, _mapper));
        }

        /// <summary>
        /// Deletes a single RuleType
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("{id}")]
        [AuthorizeRequest("RuleTypes")]
        public IHttpActionResult Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                return this.Error().InvalidParameters("Invalid rule type parameter");
            }

            var ruleType = _ruleTypeRepository.Get(id);
            if (ruleType is null)
            {
                return NotFound();
            }

            var ruleTypeModel = Mappings.MapToRuleTypeModel(ruleType, _ruleRepository, _mapper);

            var deletedRuleIds = ruleTypeModel.Rules
               .Select(r => new { r.Id, r.RuleId })
               .ToList();

            _ruleTypeRepository.Delete(id);
            _ruleTypeRepository.SaveChanges();

            if (deletedRuleIds.Any())
            {
                deletedRuleIds.ForEach(r => _ruleRepository.Delete(r.Id));
                _ruleRepository.SaveChanges();
            }

            if (ruleType.AllowedForAutopilot)
            {
                UpdateAutopilotRulesRepository(ruleTypeModel.Id, new List<int>(), deletedRuleIds.Select(r => r.RuleId).ToList());
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Updates autopilot rules according to the rules repository changes
        /// </summary>
        /// <param name="ruleTypeId"></param>
        /// <param name="newRules"></param>
        /// <param name="deletedRules"></param>
        private void UpdateAutopilotRulesRepository(int ruleTypeId, IReadOnlyCollection<int> newRules, IReadOnlyCollection<int> deletedRules)
        {
            var flexibilityLevels = _flexibilityLevelRepository.GetAll().ToList();

            // delete autopilot rules
            if (deletedRules != null && deletedRules.Any())
            {
                var autopilotRulesToDelete = _autopilotRuleRepository.GetAll()
                    .Where(rt => rt.RuleTypeId == ruleTypeId && deletedRules.Contains(rt.RuleId)).ToList();

                if (autopilotRulesToDelete.Any())
                {
                    _autopilotRuleRepository.Delete(autopilotRulesToDelete.Select(r => r.Id));
                }
            }

            // add new autopilot rules
            if (newRules != null && newRules.Any())
            {
                foreach (var ruleId in newRules)
                {
                    foreach (var flexibilityLevel in flexibilityLevels)
                    {
                        var autopilotRule = AutopilotRule.Create(flexibilityLevel.Id, ruleId, ruleTypeId);
                        _autopilotRuleRepository.Add(autopilotRule);
                    }
                }
            }

            _autopilotRuleRepository.SaveChanges();
        }
    }
}
