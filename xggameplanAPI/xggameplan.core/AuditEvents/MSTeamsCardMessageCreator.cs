using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using xggameplan.common.MSTeams;
using xggameplan.Services;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Card message creator for MS Teams
    /// </summary>
    public class MSTeamsCardMessageCreator : MSTeamsMessageCreatorBase, IMSTeamsMessageCreator
    {
        private readonly string _frontendUrl;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly MSTeamsREST _msTeamsREST;
        private readonly List<MSTeamsAuditEventSettings> _msTeamsAuditEventSettingsList;
        private readonly List<AuditEventValueConverter> _valueConverters;

        public MSTeamsCardMessageCreator(string frontendUrl, IRepositoryFactory repositoryFactory, MSTeamsREST msTeamsREST,
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
            get { return "Card"; }
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

            List<string> detailsTextList = new List<string>();
            List<string> detailsUrlList = new List<string>();

            StringBuilder title = new StringBuilder(auditEventType.Description);
            if (!String.IsNullOrEmpty(subEvent))
            {
                title.Append(string.Format(" - {0}", subEvent));
            }

            // Set Run/Scenario details
            Run run = null;
            StringBuilder message = new StringBuilder("");
            if (auditEventValueRunId != null || auditEventValueScenarioId != null)
            {
                using (var scope = _repositoryFactory.BeginRepositoryScope())
                {
                    var runRepository = scope.CreateRepository<IRunRepository>();
                    run = (auditEventValueRunId != null ? runRepository.Find((Guid)auditEventValueRunId.Value) : runRepository.FindByScenarioId((Guid)auditEventValueScenarioId.Value));

                    if (auditEventValueScenarioId == null)
                    {
                        // Set URL
                        detailsUrlList.Add(WebLinkFactory.GetRunDetailsURL(_frontendUrl, run.Id));
                        detailsTextList.Add("Run Details");
                    }
                    else
                    {
                        detailsUrlList.Add(WebLinkFactory.GetRunDetailsURL(_frontendUrl, run.Id));
                        detailsTextList.Add("Run Details");

                        // Set URL
                        detailsUrlList.Add(WebLinkFactory.GetOptimiserReportURL(_frontendUrl, run.Id, (Guid)auditEventValueScenarioId.Value));
                        detailsTextList.Add("Scenario Details");
                    }
                }
            }

            // Set AutoBook details
            if (auditEventValueAutoBookId != null)
            {
                detailsUrlList.Add(WebLinkFactory.GetAutoBookURL(_frontendUrl, (Guid)auditEventValueAutoBookId.Value));
                detailsTextList.Add("AutoBook Details");
            }

            DataTable dataTable = GetValuesDataTable(auditEvent, run, auditEventType, subEvent, auditEventValueTypeRepository, _valueConverters);
            foreach (DataRow dataRow in dataTable.Rows)
            {
                message.Append(message.Length == 0 ? "" : "; ");        //message.Append(message.Length == 0 ? "" : "<BR/>");
                message.Append(string.Format("{0}: {1}", dataRow[0].ToString(), dataRow[1].ToString()));
            }

            // Post message
            _msTeamsREST.PostMessageCard(postMessageSettings.Url, "Title", title.ToString(), message.ToString(), detailsTextList, detailsUrlList);
        }

        public bool Handles(AuditEvent auditEvent)
        {
            MSTeamsAuditEventSettings auditEventSettings = _msTeamsAuditEventSettingsList == null ? null : _msTeamsAuditEventSettingsList.Find(s => s.EventTypeId == auditEvent.EventTypeID && s.MessageCreatorId == Id);
            if (auditEventSettings == null || auditEventSettings.PostMessageSettings == null || !auditEventSettings.PostMessageSettings.Enabled)
            {
                return false;
            }
            return true;
        }
    }
}
