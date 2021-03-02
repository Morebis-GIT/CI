using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Mail;
using System.Text;

namespace xggameplan.AuditEvents
{
    /// <summary>
    /// Creates email for audit event using basic formatting
    /// </summary>
    public class BasicEmailCreator : IAuditEventEmailCreator
    {
        private readonly string _htmlStyle;
        private readonly IAuditEventTypeRepository _auditEventTypeRepository;
        private readonly IAuditEventValueTypeRepository _auditEventValueTypeRepository;
        private readonly List<EmailAuditEventSettings> _emailAuditEventSettingsList;
        private readonly List<AuditEventValueConverter> _valueConverters;

        public BasicEmailCreator(IAuditEventTypeRepository auditEventTypeRepository, IAuditEventValueTypeRepository auditEventValueTypeRepository,
                                             List<EmailAuditEventSettings> emailAuditEventSettingsList,
                                             List<AuditEventValueConverter> valueConverters, string htmlStyle)
        {
            _auditEventTypeRepository = auditEventTypeRepository;
            _auditEventValueTypeRepository = auditEventValueTypeRepository;
            _emailAuditEventSettingsList = emailAuditEventSettingsList;
            _valueConverters = valueConverters;
            _htmlStyle = htmlStyle;
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

            AuditEventType auditEventType = _auditEventTypeRepository.GetByID(auditEvent.EventTypeID);

            MailMessage mail = new MailMessage();
            mail.Subject = string.Format("{0}", auditEventType.Description);
            mail.Sender = new MailAddress(emailAuditEventSettings.NotificationSettings.SenderAddress, emailAuditEventSettings.NotificationSettings.SenderAddress);
            mail.From = new MailAddress(emailAuditEventSettings.NotificationSettings.SenderAddress, emailAuditEventSettings.NotificationSettings.SenderAddress);
            mail.Body = GetBody(auditEvent, auditEventType);
            mail.IsBodyHtml = mail.Body.ToLower().Contains("<html>") && (mail.Body.ToLower().Contains("<head>") || mail.Body.ToLower().Contains("<head/>"));

            // Set list of recipients
            for (int index = 0; index < emailAuditEventSettings.NotificationSettings.RecipientAddresses.Count; index++)
            {
                mail.To.Add(new MailAddress(emailAuditEventSettings.NotificationSettings.RecipientAddresses[index], emailAuditEventSettings.NotificationSettings.RecipientAddresses[index]));
            }
            return mail;
        }

        /// <summary>
        /// Returns email body. Email has header details (event type, event type etc) and table for each audit event value.
        /// </summary>
        /// <param name="auditEvent"></param>
        /// <returns></returns>
        private string GetBody(AuditEvent auditEvent, AuditEventType auditEventType)
        {
            StringBuilder html = new StringBuilder(string.Format("<html><head>{0}</head><body>", _htmlStyle));

            _ = html.Append("<table>");
            DataTable valuesDataTable = GetValuesDataTable(auditEvent, auditEventType);
            for (int rowIndex = 0; rowIndex < valuesDataTable.Rows.Count; rowIndex++)
            {
                _ = html.Append(string.Format("<tr><td><B>{0}</B></td><td>{1}</td></tr>", valuesDataTable.Rows[rowIndex]["Item"].ToString(), valuesDataTable.Rows[rowIndex]["Value"].ToString()));
            }
            _ = html.Append("</table>");
            _ = html.Append("</body>");
            _ = html.Append("</html>");
            return html.ToString();
        }

        private DataTable GetValuesDataTable(AuditEvent auditEvent, AuditEventType auditEventType)
        {
            DataTable dataTable = new DataTable();
            _ = dataTable.Columns.Add("Item", typeof(String));
            _ = dataTable.Columns.Add("Value", typeof(String));

            // Add standard header
            AddValuesRow(dataTable, "Time", auditEvent.TimeCreated.ToString());
            AddValuesRow(dataTable, "Event ID", auditEvent.ID);
            AddValuesRow(dataTable, "Event", auditEventType.Description);

            // Add other values
            foreach (AuditEventValue auditEventValue in auditEvent.Values)
            {
                AuditEventValueType auditEventValueType = _auditEventValueTypeRepository.GetByID(auditEventValue.TypeID);
                //if (!auditEventValueType.Internal)
                //{
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
            return dataTable;
        }

        public bool Handles(AuditEvent auditEvent)
        {
            EmailAuditEventSettings emailAuditEventSettings = _emailAuditEventSettingsList == null ? null : _emailAuditEventSettingsList.Find(s => s.EventTypeId == auditEvent.EventTypeID && s.EmailCreatorId == Id);
            if (emailAuditEventSettings == null || emailAuditEventSettings.NotificationSettings == null || !emailAuditEventSettings.NotificationSettings.Enabled)
            {
                return false;
            }
            return true;
        }

        private void AddValuesRow(DataTable dataTable, string item, string value)
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
        private string CreateHTMLLink(string linkText, string url)
        {
            return string.Format("<a href='{0}'>{1}</a>", url, linkText);
        }
    }
}
