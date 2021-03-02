using System;
using Microsoft.Extensions.Configuration;
using xggameplan.common.Email;

namespace xggameplan.Email
{
    /// <summary>
    /// Email utilities
    /// </summary>
    public class EmailUtilities
    {
        /// <summary>
        /// Returns the email connection
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IEmailConnection GetEmailConnection(IConfiguration configuration)
        {
            EmailConnectionSettings emailConnectionSettings = GetEmailConnectionSettings(configuration);
            switch (emailConnectionSettings.Settings["Type"])
            {
                case "SMTP": return new SmtpEmailConnection(emailConnectionSettings);
                case "HTTPSendGrid": return new SendGridHttpEmailConnection(emailConnectionSettings);
                default: throw new ArgumentException("Email connection type in the configuration is missing or invalid");
            }            
        }

        /// <summary>
        /// Returns email connection settings from configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static EmailConnectionSettings GetEmailConnectionSettings(IConfiguration configuration)
        {
            EmailConnectionSettings emailConnectionSettings = new EmailConnectionSettings();
            emailConnectionSettings.Settings.Add("Type", configuration["Email:Connection:Type"]);
            switch (configuration["Email:Connection:Type"])
            {
                case "SMTP":
                    emailConnectionSettings.Settings.Add("Server", configuration["Email:Connection:Server"]);
                    emailConnectionSettings.Settings.Add("Port", Convert.ToInt32(configuration["Email:Connection:Port"]));
                    emailConnectionSettings.Settings.Add("Username", configuration["Email:Connection:Username"]);
                    emailConnectionSettings.Settings.Add("Password", configuration["Email:Connection:Password"]);
                    emailConnectionSettings.Settings.Add("UseSSL", Convert.ToBoolean(configuration["Email:Connection:UseSSL"]));
                    break;
                case "HTTPSendGrid":                    
                    emailConnectionSettings.Settings.Add("APIKey", configuration["Email:Connection:APIKey"]);
                    break;
            }
            return emailConnectionSettings;
        }
    }
}
