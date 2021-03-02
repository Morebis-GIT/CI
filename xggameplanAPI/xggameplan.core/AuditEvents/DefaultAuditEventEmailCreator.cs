using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Generic;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Default email creator for audit events
    /// </summary>
    public class DefaultAuditEventEmailCreator : BaseAuditEventEmailCreator, IAuditEventEmailCreator
    {
        private string _environmentId;
        private List<EmailAuditEventSettings> _emailAuditEventSettingsList;
        private List<AuditEventValueConverter> _valueConverters;

        public DefaultAuditEventEmailCreator(string environmentId, List<EmailAuditEventSettings> emailAuditEventSettingsList,
                                             List<AuditEventValueConverter> valueConverters)
        {
            _environmentId = environmentId;
            _emailAuditEventSettingsList = emailAuditEventSettingsList;
            _valueConverters = valueConverters;
        }

        public string Id
        {
            get { return "Default"; }
        }

        public MailMessage CreateEmail(AuditEvent auditEvent)
        {
            // Determine if we need to generate email
            EmailAuditEventSettings emailAuditEventSettings = _emailAuditEventSettingsList.Find(s => s.EventTypeId == auditEvent.EventTypeID);
            if (!Handles(auditEvent))
            {
                return null;
            }            

            AuditEventValueTypeRepository auditEventValueTypeRepository = new AuditEventValueTypeRepository();
            AuditEventType auditEventType = new AuditEventTypeRepository().GetByID(auditEvent.EventTypeID);

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

            MailMessage mail = new MailMessage();
            if (!String.IsNullOrEmpty(subEvent))   // Sub event indicated
            {
                mail.Subject = string.Format("[{0}] {1}: {2}: {3}", _environmentId, Globals.ProductName, auditEventType.Description, subEvent);
            }
            else
            { 
                mail.Subject = string.Format("[{0}] {1}: {2}", _environmentId, Globals.ProductName, auditEventType.Description);
            }                        
            mail.Sender = new MailAddress(emailAuditEventSettings.NotificationSettings.SenderAddress, emailAuditEventSettings.NotificationSettings.SenderAddress);
            mail.From = new MailAddress(emailAuditEventSettings.NotificationSettings.SenderAddress, emailAuditEventSettings.NotificationSettings.SenderAddress);
            mail.Body = GetBody(auditEvent, subEvent, _valueConverters, auditEventValueTypeRepository);
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

        /// <summary>
        /// Returns email body. Email has header details (event type, event type etc) and table for each audit event value.
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <returns></returns>
        private string GetBody(AuditEvent auditEvent, string subEvent, List<AuditEventValueConverter> valueConverters, AuditEventValueTypeRepository auditEventValueTypeRepository)
        {
            AuditEventType auditEventType = new AuditEventTypeRepository().GetByID(auditEvent.EventTypeID);
            StringBuilder html = new StringBuilder(string.Format("<html><head>{0}</head><body>", GetStyle()));

            html.Append("<table>");
            //html.Append("<tr><th>Item</th><th>Value</th></tr>");
            DataTable valuesDataTable = GetValuesDataTable(auditEvent, auditEventType, subEvent, auditEventValueTypeRepository);
            for (int rowIndex = 0; rowIndex < valuesDataTable.Rows.Count; rowIndex++)
            {
                html.Append(string.Format("<tr><td><B>{0}</B></td><td>{1}</td></tr>", valuesDataTable.Rows[rowIndex]["Item"].ToString(), valuesDataTable.Rows[rowIndex]["Value"].ToString()));
            }
            html.Append("</table>");
            html.Append("</body>");
            html.Append("</html>");
            return html.ToString();
        }

        private DataTable GetValuesDataTable(AuditEvent auditEvent, AuditEventType auditEventType, string subEvent, AuditEventValueTypeRepository auditEventValueTypeRepository)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Item", typeof(String));
            dataTable.Columns.Add("Value", typeof(String));

            // Add standard header
            AddValuesRow(dataTable, "Time", auditEvent.TimeCreated.ToString());
            AddValuesRow(dataTable, "Event ID", auditEvent.ID);
            AddValuesRow(dataTable, "Event", string.IsNullOrEmpty(subEvent) ? auditEventType.Description : string.Format("{0} - {1}", auditEventType.Description, subEvent));

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

    public abstract class BaseAuditEventEmailCreator
    {        
        protected string GetValueDescription(AuditEvent auditEvent, int valueTypeId, AuditEventValueTypeRepository auditEventValueTypeRepository, List<AuditEventValueConverter> valueConverters)
        {
            string eventDescription = "";
            AuditEventValue auditEventValue = auditEvent.GetValueByValueTypeId(valueTypeId);
            if (auditEventValue != null && auditEventValue.Value != null)
            {
                AuditEventValueType auditEventValueType = auditEventValueTypeRepository.GetByID(auditEventValue.TypeID);
                AuditEventValueConverter auditEventValueConverter = valueConverters.Find(current => current.Handles(auditEventValue.TypeID));
                if (auditEventValueConverter != null)   // Sanity check
                {
                    eventDescription = auditEventValueConverter.ValueConverter.Convert(auditEventValue.Value, auditEventValueType.Type, typeof(String)).ToString();
                }
            }
            return eventDescription;
        }    

        protected string GetStyle()
        {
            StringBuilder style = new StringBuilder("<style>" +
                        "table { border: 1px solid; } " +
                        "th { border: 1px solid; } " +
                        "td { border: 1px solid; } " +
                        //"a, u { text-decoration: none; } " +
                        "</style>");
            return style.ToString();
        }

        protected void AddValuesRow(DataTable dataTable, string item, string value)
        {
            DataRow row = dataTable.NewRow();
            row[0] = item;
            row[1] = value;
            dataTable.Rows.Add(row);
        }

        /// <summary>
        /// Creates HTML link
        /// </summary>
        /// <param name="linkText"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        protected string CreateHTMLLink(string linkText, string url)
        {
            return string.Format("<a href='{0}'>{1}</a>", url, linkText);
        }
    }
}
