using System;
using System.Collections.Generic;
using System.Net.Mail;
using ImagineCommunications.GamePlan.Domain.Generic;
using Microsoft.Extensions.Configuration;
using xggameplan.common.Email;
using xggameplan.Email;

namespace xggameplan.SystemTests
{
    /// <summary>
    /// Tests email mechanism
    /// </summary>
    internal class EmailTest : ISystemTest
    {
        private readonly IConfiguration _applicationConfiguration;
        private const string _category = "Email";

        public EmailTest(IConfiguration applicationConfiguration)
        {
            _applicationConfiguration = applicationConfiguration;
        }

        public bool CanExecute(SystemTestCategories systemTestCategories) => true;

        public List<SystemTestResult> Execute(SystemTestCategories systemTestCategory)
        {
            var results = new List<SystemTestResult>();
            MailMessage mailMessage = new MailMessage()
            {
                Subject = "Test email",
                Sender = new MailAddress(Globals.EngineeringEmailAddress),
                From = new MailAddress(Globals.EngineeringEmailAddress),
                Body = "Test email",
                IsBodyHtml = false,
            };
            mailMessage.To.Add(new MailAddress(Globals.EngineeringEmailAddress));

            try
            {
                IEmailConnection emailConnection = EmailUtilities.GetEmailConnection(_applicationConfiguration);
                emailConnection.SendEmail(mailMessage);

                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, "Email test OK.", ""));  // Email passed to server, we don't know for certain that it will arrive
            }
            catch (System.Exception exception)
            {
                results.Add(new SystemTestResult(SystemTestResult.ResultTypes.Warning, _category, String.Format("Error sending test email: {0}. Notifications will not work. Please check the configuration.", exception.Message), ""));
            }
            return results;
        }
    }
}
