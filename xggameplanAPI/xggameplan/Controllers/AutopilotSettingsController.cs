using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using xggameplan.Filters;
using xggameplan.Model;
using xggameplan.Services;
using xggameplan.Validations;

namespace xggameplan.Controllers
{
    [RoutePrefix("AutopilotSettings")]
    public class AutopilotSettingsController : ApiController
    {
        private readonly IAutopilotSettingsRepository _autopilotSettingsRepository;
        private readonly IAutopilotRuleRepository _autopilotRuleRepository;
        private readonly IRuleRepository _ruleRepository;
        private readonly IRuleTypeRepository _ruleTypeRepository;
        private readonly IModelDataValidator<UpdateAutopilotSettingsModel> _autopilotSettingsValidator;
        private readonly IMapper _mapper;

        public AutopilotSettingsController(IAutopilotSettingsRepository autopilotSettingsRepository,
            IAutopilotRuleRepository autopilotRuleRepository, IRuleRepository ruleRepository,
            IRuleTypeRepository ruleTypeRepository,
            IModelDataValidator<UpdateAutopilotSettingsModel> autopilotSettingsValidator,
            IMapper mapper)
        {
            _autopilotSettingsRepository = autopilotSettingsRepository;
            _autopilotRuleRepository = autopilotRuleRepository;
            _ruleRepository = ruleRepository;
            _ruleTypeRepository = ruleTypeRepository;
            _autopilotSettingsValidator = autopilotSettingsValidator;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets autopilot settings
        /// </summary>
        /// <returns></returns>
        [Route("default")]
        [AuthorizeRequest("AutopilotSettings")]
        [ResponseType(typeof(AutopilotSettingsModel))]
        public IHttpActionResult GetDefault()
        {
            var autopilotSettings = _autopilotSettingsRepository.GetDefault();

            if (autopilotSettings == null)
            {
                return NotFound();
            }

            return Ok(Mappings.MapToAutopilotSettingsModel(autopilotSettings, _autopilotRuleRepository, _ruleRepository,
                _ruleTypeRepository, _mapper));
        }

        /// <summary>
        /// Updates the autopilot settings by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [Route("default/{id}")]
        [AuthorizeRequest("AutopilotSettings")]
        [ResponseType(typeof(AutopilotSettingsModel))]
        public IHttpActionResult PutDefault([FromUri] int id, [FromBody] UpdateAutopilotSettingsModel command)
        {
            if (command != null && id != command.Id)
            {
                ModelState.AddModelError(nameof(UpdateAutopilotSettingsModel.Id), "Model id does not match");
                return BadRequest(ModelState);
            }

            var autopilotSettings = _autopilotSettingsRepository.Get(id);
            if (autopilotSettings == null)
            {
                return NotFound();
            }

            if (!_autopilotSettingsValidator.IsValid(command))
            {
                return _autopilotSettingsValidator.BadRequest();
            }

            autopilotSettings.DefaultFlexibilityLevelId = command.DefaultFlexibilityLevelId;
            autopilotSettings.ScenariosToGenerate = command.ScenariosToGenerate;
            _autopilotSettingsRepository.Update(autopilotSettings);

            UpdateAutopilotRulesRepository(command.AutopilotRules);

            // Do not remove this, need to persist changes now so that we can return AutopilotSettingsModel
            _autopilotSettingsRepository.SaveChanges();
            _autopilotRuleRepository.SaveChanges();

            return Ok(Mappings.MapToAutopilotSettingsModel(autopilotSettings, _autopilotRuleRepository, _ruleRepository,
                _ruleTypeRepository, _mapper));
        }

        /// <summary>
        /// Updates existing autopilot rules
        /// </summary>
        /// <param name="autopilotRuleModels"></param>
        private void UpdateAutopilotRulesRepository(IReadOnlyCollection<UpdateAutopilotRuleModel> autopilotRuleModels)
        {
            if (autopilotRuleModels == null || !autopilotRuleModels.Any())
            {
                return;
            }

            var autopilotRules = _autopilotRuleRepository
                .GetAll()
                .GroupBy(r => r.UniqueRuleKey)
                .ToDictionary(r => r.Key, r => r.ToList());

            foreach (var ruleModel in autopilotRuleModels)
            {
                var rulesToUpdate = !autopilotRules.ContainsKey(ruleModel.UniqueRuleKey)
                        ? null
                        : autopilotRules[ruleModel.UniqueRuleKey];

                if (rulesToUpdate == null || !rulesToUpdate.Any())
                {
                    continue;
                }

                // update for each flexibility level
                foreach (var rule in rulesToUpdate)
                {
                    rule.Enabled = ruleModel.Enabled;
                    _autopilotRuleRepository.Update(rule);
                }
            }
        }
    }
}
