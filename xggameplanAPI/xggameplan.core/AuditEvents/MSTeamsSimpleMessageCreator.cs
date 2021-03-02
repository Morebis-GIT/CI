using System;
using System.Collections.Generic;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using xggameplan.common.MSTeams;
using xggameplan.Services;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Simple message creator for MS Teams
    /// </summary>
    public class MSTeamsSimpleMessageCreator : MSTeamsMessageCreatorBase, IMSTeamsMessageCreator
    {
        private string _frontendUrl;
        private IRepositoryFactory _repositoryFactory;
        private MSTeamsREST _msTeamsREST;
        private List<MSTeamsAuditEventSettings> _msTeamsAuditEventSettingsList;
        private List<AuditEventValueConverter> _valueConverters;

        public MSTeamsSimpleMessageCreator(string frontendUrl, IRepositoryFactory repositoryFactory, MSTeamsREST msTeamsREST,
            List<MSTeamsAuditEventSettings> msTeamsAuditEventSettingsList,
                        List<AuditEventValueConverter> valueConverters)
        {
            _frontendUrl = frontendUrl;
            _repositoryFactory = repositoryFactory;
            _msTeamsREST = msTeamsREST;
            _msTeamsAuditEventSettingsList = msTeamsAuditEventSettingsList;
            _valueConverters = valueConverters;
        }

        public string Id
        {
            get { return "Simple"; }
        }

        /// <summary>
        /// Posts message to MS Teams channel
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <param name="auditEventType"></param>
        /// <param name="postMessageSettings"></param>
        public void PostMessage(AuditEvent auditEvent, AuditEventType auditEventType, MSTeamsPostMessageSettings postMessageSettings)
        {
            AuditEventValueTypeRepository auditEventValueTypeRepository = new AuditEventValueTypeRepository();

            //Get Message value
            AuditEventValue auditEventValueMessage = auditEvent.GetValueByValueTypeId(AuditEventValueTypes.Message);
            AuditEventValue auditEventValueRunId = auditEvent.GetValueByValueTypeId(AuditEventValueTypes.GamePlanRunID);
            AuditEventValue auditEventValueScenarioId = auditEvent.GetValueByValueTypeId(AuditEventValueTypes.GamePlanScenarioID);
            AuditEventValue auditEventValueAutoBookId = auditEvent.GetValueByValueTypeId(AuditEventValueTypes.GamePlanAutoBookID);            
            AuditEventValue auditEventValuePipelineErrorMessage = auditEvent.GetValueByValueTypeId(AuditEventValueTypes.GamePlanPipelineEventErrorMessage);            

            // Determine if sub event is set
            string subEvent = "";
            foreach (int valueTypeId in new int[] { AuditEventValueTypes.GamePlanPipelineEventID, AuditEventValueTypes.GamePlanAutoBookEventID })
            {
                subEvent = GetValueDescription(auditEvent, valueTypeId, auditEventValueTypeRepository, _valueConverters);
                if (!String.IsNullOrEmpty(subEvent))
                {
                    break;
                }
            }            

            StringBuilder title = new StringBuilder(auditEventType.Description);
            if (!String.IsNullOrEmpty(subEvent))
            {
                title.Append(string.Format(" - {0}", subEvent));
            }

            // Set Run/Scenario details
            Run run = null;
            StringBuilder message = new StringBuilder(title.ToString());
            if (auditEventValueRunId != null || auditEventValueScenarioId != null)
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    var runRepository = scope.CreateRepository<IRunRepository>();
                    run = (auditEventValueRunId != null ? runRepository.Find((Guid)auditEventValueRunId.Value) : runRepository.FindByScenarioId((Guid)auditEventValueScenarioId.Value));

                    message.Append(message.Length == 0 ? "" : "; ");
                    message.Append(string.Format("Description: {0}; Run ID: {1}", run.Description, MSTeamsMessageFormatter.FormatHyperlink(run.Id.ToString(), WebLinkFactory.GetRunDetailsURL(_frontendUrl, run.Id))));

                    if (auditEventValueScenarioId != null)
                    {
                        message.Append(message.Length == 0 ? "" : "; ");
                        message.Append(string.Format("Scenario ID: {0}", MSTeamsMessageFormatter.FormatHyperlink(auditEventValueScenarioId.Value.ToString(), WebLinkFactory.GetOptimiserReportURL(_frontendUrl, run.Id, (Guid)auditEventValueScenarioId.Value))));
                    }
                }
            }            

            // Set AutoBook details
            if (auditEventValueAutoBookId != null)
            {
                message.Append(message.Length == 0 ? "" : "; ");
                message.Append(string.Format("AutoBook ID: {0}", MSTeamsMessageFormatter.FormatHyperlink(auditEventValueAutoBookId.Value.ToString(), WebLinkFactory.GetAutoBookURL(_frontendUrl, (Guid)auditEventValueAutoBookId.Value))));
            }

            // Set pipeline error
            if (auditEventValuePipelineErrorMessage != null)
            {
                message.Append(message.Length == 0 ? "" : "; ");
                message.Append(string.Format("Error: {0}", auditEventValuePipelineErrorMessage.Value.ToString()));
            }

            // Set message
            if (auditEventValueMessage != null)
            {
                message.Append(message.Length == 0 ? "" : "; ");
                message.Append(string.Format("Message: {0}", auditEventValueMessage.Value.ToString()));
            }

            // Post message            
            _msTeamsREST.PostSimpleMessage(postMessageSettings.Url, "", message.ToString());
        }
  
        public bool Handles(AuditEvent auditEvent)
        {            
            MSTeamsAuditEventSettings auditEventSettings = _msTeamsAuditEventSettingsList == null ? null : _msTeamsAuditEventSettingsList.Find(s => s.EventTypeId == auditEvent.EventTypeID && s.MessageCreatorId == this.Id);
            if (auditEventSettings == null || auditEventSettings.PostMessageSettings == null || !auditEventSettings.PostMessageSettings.Enabled)
            {
                return false;
            }
            return true;
        }        
    }
}
