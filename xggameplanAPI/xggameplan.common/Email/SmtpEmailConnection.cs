using System;
using System.Net.Mail;

namespace xggameplan.common.Email
{
    /// <summary>
    /// Sends email via SMTP
    /// </summary>
    public class SmtpEmailConnection : IEmailConnection
    {
        private readonly EmailConnectionSettings _emailConnectionSettings;
        private SmtpClient _client = null;

        public SmtpEmailConnection(EmailConnectionSettings emailConnectionSettings)
        {
            _emailConnectionSettings = emailConnectionSettings;
        }

        public void SendEmail(MailMessage message)
        {
            SmtpClient client = GetSmtpClient();
            client.Send(message);
        }

        private SmtpClient GetSmtpClient()
        {
            if (_client == null)   // Not set
            {
                // Set SMTP connection
                _client = new SmtpClient((string)_emailConnectionSettings.Settings["Server"], (int)_emailConnectionSettings.Settings["Port"]);
                _client.DeliveryMethod = SmtpDeliveryMethod.Network;
                if (String.IsNullOrEmpty((string)_emailConnectionSettings.Settings["Username"]))   // Default credentials
                {
                    _client.UseDefaultCredentials = true;
                }
                else          // Specific credentials
                {
                    _client.UseDefaultCredentials = false;
                    _client.Credentials = new System.Net.NetworkCredential((string)_emailConnectionSettings.Settings["Username"], (string)_emailConnectionSettings.Settings["Password"]);
                }
                _client.EnableSsl = (bool)_emailConnectionSettings.Settings["UseSSL"];
            }
            return _client;
        }
    }
}
