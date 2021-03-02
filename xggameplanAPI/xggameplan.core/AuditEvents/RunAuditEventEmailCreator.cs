using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Email creator for audit events related to a run.
    /// </summary>
    public class RunAuditEventEmailCreator : BaseAuditEventEmailCreator, IAuditEventEmailCreator
    {
        private string _environmentId;
        private string _frontendUrl = "";
        private IRepositoryFactory _repositoryFactory;
        private List<EmailAuditEventSettings> _emailAuditEventSettingsList;
        private List<AuditEventValueConverter> _valueConverters;

        public RunAuditEventEmailCreator(string environmentId, string frontendUrl, IRepositoryFactory repositoryFactory,
                                             List<EmailAuditEventSettings> emailAuditEventSettingsList,
                                             List<AuditEventValueConverter> valueConverters)
        {
            _environmentId = environmentId;
            _frontendUrl = frontendUrl;
            _repositoryFactory = repositoryFactory;
            _emailAuditEventSettingsList = emailAuditEventSettingsList;
            _valueConverters = valueConverters;
        }

        public string Id
        {
            get { return "Run"; }
        }

        public MailMessage CreateEmail(AuditEvent auditEvent)
        {
            // Determine if we need to generate email
            if (!Handles(auditEvent))
            {
                return null;
            }

            using (var scope = _repositoryFactory.BeginRepositoryScope())
            {
                EmailAuditEventSettings emailAuditEventSettings = _emailAuditEventSettingsList.Find(s => s.EventTypeId == auditEvent.EventTypeID);

                AuditEventValueTypeRepository auditEventValueTypeRepository = new AuditEventValueTypeRepository();

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

                // Get run details, check if specific scenario specified
                var runRepository = scope.CreateRepository<IRunRepository>();
                AuditEventValue runIdAuditEventValue = auditEvent.GetValueByValueTypeId(AuditEventValueTypes.GamePlanRunID);
                AuditEventValue scenarioIdAuditEventValue = auditEvent.GetValueByValueTypeId(AuditEventValueTypes.GamePlanScenarioID);
                Run run = runRepository.Find((Guid)runIdAuditEventValue.Value);

                AuditEventType auditEventType = new AuditEventTypeRepository().GetByID(auditEvent.EventTypeID);
                MailMessage mail = new MailMessage();
                if (!String.IsNullOrEmpty(subEvent))   // No pipeline event
                {
                    mail.Subject = string.Format("[{0}] {1}: {2}: {3}", _environmentId, Globals.ProductName, auditEventType.Description, subEvent);
                }
                else
                {
                    mail.Subject = string.Format("[{0}] {1}: {2}", _environmentId, Globals.ProductName, auditEventType.Description);
                }

                // Indicate if run failed in subject, bit of a hack as we have to check the message string
                if (auditEvent.EventTypeID == AuditEventTypes.GamePlanRunCompleted)
                {
                    string message = GetValueDescription(auditEvent, AuditEventValueTypes.Message, auditEventValueTypeRepository, _valueConverters);
                    if (!String.IsNullOrEmpty(message) && message.ToLower().Contains("errors"))
                    {
                        mail.Subject = mail.Subject + string.Format(" ({0})", message);
                    }
                }

                // Load scenarios
                var scenarioRepository = scope.CreateRepository<IScenarioRepository>();

                var scenarioIds = run.Scenarios.Select(e => e.Id).Distinct().ToArray();
                List<Scenario> scenarios = scenarioRepository.FindByIds(scenarioIds).ToList();

                mail.Sender = new MailAddress(emailAuditEventSettings.NotificationSettings.SenderAddress, emailAuditEventSettings.NotificationSettings.SenderAddress);
                mail.From = new MailAddress(emailAuditEventSettings.NotificationSettings.SenderAddress, emailAuditEventSettings.NotificationSettings.SenderAddress);
                mail.Body = scenarioIdAuditEventValue == null ? GetBody(auditEvent, auditEventType, run, scenarios, subEvent, auditEventValueTypeRepository) : GetBody(auditEvent, auditEventType, run, (Guid)scenarioIdAuditEventValue.Value, scenarios.First(s => s.Id == (Guid)scenarioIdAuditEventValue.Value), subEvent, auditEventValueTypeRepository);
                mail.IsBodyHtml = mail.Body.ToLower().Contains("<html>") && (mail.Body.ToLower().Contains("<head>") || mail.Body.ToLower().Contains("<head/>"));

                // Check if specific email address indicated in audit event

                List<AuditEventValue> recipientAuditEventValues = new List<AuditEventValue>();
                AuditEventValue recipientAuditEventValue = auditEvent.GetValueByValueTypeId(AuditEventValueTypes.RecipientEmailAddress);
                if (recipientAuditEventValue != null)
                {
                    recipientAuditEventValues.Add(recipientAuditEventValue);
                }
                if (recipientAuditEventValues.Count == 0)
                {
                    for (int index = 0; index < emailAuditEventSettings.NotificationSettings.RecipientAddresses.Count; index++)
                    {
                        mail.To.Add(new MailAddress(emailAuditEventSettings.NotificationSettings.RecipientAddresses[index], emailAuditEventSettings.NotificationSettings.RecipientAddresses[index]));
                    }
                }
                else
                {
                    foreach (AuditEventValue auditEventValue in recipientAuditEventValues)
                    {
                        mail.To.Add(new MailAddress(auditEventValue.Value.ToString()));
                    }
                }
                if (emailAuditEventSettings.NotificationSettings.CCAddresses != null)
                {
                    emailAuditEventSettings.NotificationSettings.CCAddresses.ForEach(address => mail.CC.Add(new MailAddress(address, address)));
                }

                /*
                foreach (AuditEventValue auditEventValue in auditEvent.Values.Where(item => item.TypeID == AuditEventValueTypes.EmailAttachment))
                {
                    mail.Attachments.Add(new Attachment(auditEventValue.Value.ToString()));
                }
                */
                return mail;
            }
        }

        private DataTable GetValuesDataTable(AuditEvent auditEvent, Run run, AuditEventType auditEventType, string subEvent, AuditEventValueTypeRepository auditEventValueTypeRepository)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Item", typeof(String));
            dataTable.Columns.Add("Value", typeof(String));

            string runUrl = string.Format(@"{0}/#/manage-runs", _frontendUrl);

            // Add standard header
            AddValuesRow(dataTable, "Time", auditEvent.TimeCreated.ToString());
            AddValuesRow(dataTable, "Event ID", auditEvent.ID);
            AddValuesRow(dataTable, "Event", string.IsNullOrEmpty(subEvent) ? auditEventType.Description : string.Format("{0} - {1}", auditEventType.Description, subEvent));

            // Add values from Run ID
            AddValuesRow(dataTable, "Run ID", CreateHTMLLink(run.Id.ToString(), runUrl));
            AddValuesRow(dataTable, "Description", run.Description);
            AddValuesRow(dataTable, "Date Range", string.Format("{0} - {1}", run.StartDate.ToString(), run.EndDate.ToString()));
            AddValuesRow(dataTable, "Created", run.CreatedDateTime.ToString());
            AddValuesRow(dataTable, "Executed", run.ExecuteStartedDateTime == null ? "n/a" : run.ExecuteStartedDateTime.Value.ToString());
            if (run.CompletedScenarios.Count == run.Scenarios.Count && run.LastScenarioCompletedDateTime != null)
            {
                AddValuesRow(dataTable, "Completed", run.LastScenarioCompletedDateTime.Value.ToString());
            }

            // Add other values
            HashSet<int> valueTypesDone = new HashSet<int>() { AuditEventValueTypes.GamePlanRunID, AuditEventValueTypes.GamePlanPipelineEventID, AuditEventValueTypes.GamePlanAutoBookEventID };
            foreach (AuditEventValue auditEventValue in auditEvent.Values)
            {
                if (!valueTypesDone.Contains(auditEventValue.TypeID))   // Ignore values done above
                {
                    AuditEventValueType auditEventValueType = auditEventValueTypeRepository.GetByID(auditEventValue.TypeID);
                    if (!auditEventValueType.Internal)
                    {
                        AuditEventValueConverter auditEventValueConverter = _valueConverters.Find(current => current.Handles(auditEventValue.TypeID));
                        if (auditEventValueConverter != null)
                        {
                            AddValuesRow(dataTable, auditEventValueType.Description, auditEventValueConverter.ValueConverter.Convert(auditEventValue.Value, auditEventValueType.Type, typeof(String)).ToString());
                        }
                        else
                        {
                            AddValuesRow(dataTable, auditEventValueType.Description, auditEventValue.Value.ToString());
                        }
                    }
                }
            }
            return dataTable;
        }

        private string GetBody(AuditEvent auditEvent, AuditEventType auditEventType, Run run, List<Scenario> scenarios, string subEvent, AuditEventValueTypeRepository auditEventValueTypeRepository)
        {
            StringBuilder text = new StringBuilder("<table>");
            //text.Append("<tr><th>Item</th><th>Value</th></tr>");
            DataTable valuesDataTable = GetValuesDataTable(auditEvent, run, auditEventType, subEvent, auditEventValueTypeRepository);
            for (int rowIndex = 0; rowIndex < valuesDataTable.Rows.Count; rowIndex++)
            {
                text.Append(string.Format("<tr><td><B>{0}</B></td><td>{1}</td></tr>", valuesDataTable.Rows[rowIndex]["Item"].ToString(), valuesDataTable.Rows[rowIndex]["Value"].ToString()));
            }
            text.Append("</table><BR/>");

            text.Append("<B>Scenarios</B><BR/><table>");
            text.Append("<tr><th>Scenario ID</th><th>Passes</th><th>Started</th><th>Completed</th><th>Status</th></tr>");
            foreach (var runScenario in run.Scenarios)
            {
                var scenario = scenarios.First(s => s.Id == runScenario.Id);
                string scenarioUrl = string.Format(@"{0}/#/optimizer-report/{1}/{2}", _frontendUrl, run.Id, runScenario.Id);
                text.Append(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", CreateHTMLLink(runScenario.Id.ToString(), scenarioUrl), scenario.Passes.Count,
                                                (runScenario.StartedDateTime == null ? "n/a" : runScenario.StartedDateTime.Value.ToString()),
                                                (runScenario.CompletedDateTime == null ? "n/a" : runScenario.CompletedDateTime.Value.ToString()),
                                                runScenario.Status.ToString()));
            }
            text.Append("</table>");
            return string.Format("<html><head>{0}</head><body>{1}</body></html>", GetStyle(), text.ToString());
        }

        /// <summary>
        /// Returns email body. Email has header details (event type, event type etc) and table for each audit event value.
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <returns></returns>
        private string GetBody(AuditEvent auditEvent, AuditEventType auditEventType, Run run, Guid scenarioId, Scenario scenario, string pipelineEvent, AuditEventValueTypeRepository auditEventValueTypeRepository)
        {
            StringBuilder text = new StringBuilder("<table>");
            //text.Append("<tr><th>Item</th><th>Value</th></tr>");
            DataTable valuesDataTable = GetValuesDataTable(auditEvent, run, auditEventType, pipelineEvent, auditEventValueTypeRepository);
            for (int rowIndex = 0; rowIndex < valuesDataTable.Rows.Count; rowIndex++)
            {
                text.Append(string.Format("<tr><td><B>{0}</B></td><td>{1}</td></tr>", valuesDataTable.Rows[rowIndex]["Item"].ToString(), valuesDataTable.Rows[rowIndex]["Value"].ToString()));
            }
            text.Append("</table><BR/>");

            text.Append("<B>Scenario</B><BR/><table>");
            text.Append("<tr><th>Scenario ID</th><th>Passes</th><th>Started</th><th>Completed</th><th>Status</th></tr>");
            foreach (var runScenario in run.Scenarios.Where(s => s.Id == scenarioId))
            {
                string scenarioUrl = string.Format(@"{0}/#/optimizer-report/{1}/{2}", _frontendUrl, run.Id, runScenario.Id);
                text.Append(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", CreateHTMLLink(runScenario.Id.ToString(), scenarioUrl), scenario.Passes.Count,
                                                (runScenario.StartedDateTime == null ? "n/a" : runScenario.StartedDateTime.Value.ToString()),
                                                (runScenario.CompletedDateTime == null ? "n/a" : runScenario.CompletedDateTime.Value.ToString()),
                                                runScenario.Status.ToString()));
            }
            text.Append("</table>");
            return string.Format("<html><head>{0}</head><body>{1}</body></html>", GetStyle(), text.ToString());
        }

        public bool Handles(AuditEvent auditEvent)
        {
            EmailAuditEventSettings emailAuditEventSettings = _emailAuditEventSettingsList == null ? null : _emailAuditEventSettingsList.Find(s => s.EventTypeId == auditEvent.EventTypeID && s.EmailCreatorId == this.Id);
            if (emailAuditEventSettings == null || emailAuditEventSettings.NotificationSettings == null || !emailAuditEventSettings.NotificationSettings.Enabled)
            {
                return false;
            }
            return true;
        }
    }
}
