namespace xggameplan.RunManagement.Notifications
{
    /// <summary>
    /// Generates a notification via email
    /// </summary>
    //public class EmailNotification : INotification<EmailNotificationSettings>
    //{
    //    private string _environmentId;
    //    private IEmailConnection _emailConnection;

    //    public EmailNotification(string environmentId, IEmailConnection emailConnection)
    //    {
    //        _environmentId = environmentId;
    //        _emailConnection = emailConnection;
    //    }

    //    public void RunCompleted(Run run, bool success, EmailNotificationSettings emailNotificationSettings)
    //    {
    //        if (emailNotificationSettings.Enabled)
    //        {
    //            _emailConnection.SendEmail(GetRunCompletedEmail(run, success, emailNotificationSettings));
    //        }
    //    }

    //    private MailMessage GetRunCompletedEmail(Run run, bool success, EmailNotificationSettings emailNotificationSettings)
    //    {
    //        MailMessage mailMessage = new MailMessage()
    //        {
    //            Subject = string.Format("[{0}]: {1} notification: Run completed", _environmentId, Globals.ProductName),
    //            Body = string.Format("<html><head>{0}</head><body>{1}</body></html>", DefaultStyle, GetRunDetailsForEmail(run)),
    //            IsBodyHtml = true,
    //            From = new MailAddress(emailNotificationSettings.SenderAddress, emailNotificationSettings.SenderAddress),
    //            Sender = new MailAddress(emailNotificationSettings.SenderAddress, emailNotificationSettings.SenderAddress)
    //        };
    //        foreach (var recipientAddress in emailNotificationSettings.RecipientAddresses)
    //        {
    //            mailMessage.To.Add(new MailAddress(recipientAddress));
    //        }
    //        foreach (var ccAddress in emailNotificationSettings.CCAddresses)
    //        {
    //            mailMessage.CC.Add(new MailAddress(ccAddress));
    //        }
    //        return mailMessage;
    //    }

    //    public void RunCompleted(Run run, Scenario scenario, bool success, EmailNotificationSettings emailNotificationSettings)
    //    {
    //        if (emailNotificationSettings.Enabled)
    //        {
    //            _emailConnection.SendEmail(GetRunCompletedEmail(run, scenario, success, emailNotificationSettings));
    //        }
    //    }

    //    private MailMessage GetRunCompletedEmail(Run run, Scenario scenario, bool success, EmailNotificationSettings emailNotificationSettings)
    //    {
    //        MailMessage mailMessage = new MailMessage()
    //        {
    //            Subject = string.Format("[{0}]: {1} notification: Run scenario completed", _environmentId, Globals.ProductName),
    //            Body = string.Format("<html><head>{0}</head><body>{1}</body></html>", DefaultStyle, GetRunDetailsForEmail(run, scenario)),
    //            IsBodyHtml = true,
    //            From = new MailAddress(emailNotificationSettings.SenderAddress, emailNotificationSettings.SenderAddress),
    //            Sender = new MailAddress(emailNotificationSettings.SenderAddress, emailNotificationSettings.SenderAddress)
    //        };
    //        foreach (var recipientAddress in emailNotificationSettings.RecipientAddresses)
    //        {
    //            mailMessage.To.Add(new MailAddress(recipientAddress));
    //        }
    //        foreach (var ccAddress in emailNotificationSettings.CCAddresses)
    //        {
    //            mailMessage.CC.Add(new MailAddress(ccAddress));
    //        }
    //        return mailMessage;
    //    }

    //    public void SmoothCompleted(Run run, bool success, EmailNotificationSettings emailNotificationSettings)
    //    {
    //        if (emailNotificationSettings.Enabled)
    //        {
    //            _emailConnection.SendEmail(GetSmoothCompletedEmail(run, success, emailNotificationSettings));
    //        }
    //    }

    //    private MailMessage GetSmoothCompletedEmail(Run run, bool success, EmailNotificationSettings emailNotificationSettings)
    //    {
    //        MailMessage mailMessage = new MailMessage()
    //        {
    //            Subject = string.Format("[{0}] {1} notification: Smooth completed", _environmentId, Globals.ProductName),
    //            Body = string.Format("<html><head>{0}</head><body>{1}</body></html>", DefaultStyle, "Smooth completed"),
    //            IsBodyHtml = true,
    //            From = new MailAddress(emailNotificationSettings.SenderAddress, emailNotificationSettings.SenderAddress),
    //            Sender = new MailAddress(emailNotificationSettings.SenderAddress, emailNotificationSettings.SenderAddress)
    //        };
    //        foreach (var recipientAddress in emailNotificationSettings.RecipientAddresses)
    //        {
    //            mailMessage.To.Add(new MailAddress(recipientAddress));
    //        }
    //        foreach (var ccAddress in emailNotificationSettings.CCAddresses)
    //        {
    //            mailMessage.CC.Add(new MailAddress(ccAddress));
    //        }
    //        return mailMessage;
    //    }

    //    private string GetRunDetailsForEmail(Run run)
    //    {
    //        StringBuilder text = new StringBuilder(string.Format("<B>Run ID:</B> {0}<BR/>" +
    //                                                             "<B>Description:</B> {1}<BR/>" +
    //                                                             "<B>From:</B> {2}<BR/>" +
    //                                                             "<B>To:</B> {3}<BR/>" +
    //                                                             "<B>Created:</B> {4}<BR/>" +
    //                                                             "<B>Executed:</B> {5}<BR/><BR/>",
    //                                                                        run.Id, run.Description, run.StartDateTime.ToString(), run.EndDateTime,
    //                                                                        run.CreatedDateTime.ToString(), (run.ExecuteStartedDateTime == null ? "n/a" : run.ExecuteStartedDateTime.ToString())));

    //        text.Append("<B>Scenarios</B><BR/><table>");
    //        text.Append("<tr><th>Scenario</th><th>Passes</th><th>Started</th><th>Completed</th><th>Status</th></tr>");
    //        foreach (Scenario scenario in run.Scenarios)
    //        {
    //            text.Append(string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td></tr>", scenario.Id, scenario.Passes.Count,
    //                                            (scenario.StartedDateTime == null ? "n/a" : scenario.StartedDateTime.Value.ToString()),
    //                                            (scenario.CompletedDateTime == null ? "n/a" : scenario.CompletedDateTime.Value.ToString()),
    //                                            scenario.Status.ToString()));
    //        }
    //        text.Append("</table>");
    //        return text.ToString();
    //    }

    //    private string GetRunDetailsForEmail(Run run, Scenario scenario)
    //    {
    //        StringBuilder text = new StringBuilder(string.Format("<B>Run ID:</B> {0}<BR/>" +
    //                                                             "<B>Description:</B> {1}<BR/>" +
    //                                                             "<B>From:</B> {2}<BR/>" +
    //                                                             "<B>To:</B> {3}<BR/>" +
    //                                                             "<B>Created:</B> {4}<BR/>" +
    //                                                             "<B>Executed:</B> {5}<BR/>" +
    //                                                             "<B>ScenarioID:</B> {6}<BR/>" +
    //                                                             "<B>Status: </B> {7}<BR/>",
    //                                                             run.Id, run.Description, run.StartDateTime.ToString(), run.EndDateTime,
    //                                                             run.CreatedDateTime.ToString(), (run.ExecuteStartedDateTime == null ? "n/a" : run.ExecuteStartedDateTime.ToString()), scenario.Id, scenario.Status));
    //        return text.ToString();
    //    }

    //    private static string DefaultStyle
    //    {
    //        get
    //        {
    //            return "<style>" +
    //                    "table { border: 1px solid; } " +
    //                    "th { border: 1px solid; } " +
    //                    "td { border: 1px solid; } " +
    //                    "</style>";
    //        }
    //    }
    //}
}