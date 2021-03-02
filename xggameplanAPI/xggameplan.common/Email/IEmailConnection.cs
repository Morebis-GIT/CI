using System.Net.Mail;

namespace xggameplan.common.Email
{
    /// <summary>
    /// Interface for email connection
    /// </summary>
    public interface IEmailConnection
    {
        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="message"></param>
        void SendEmail(MailMessage message);
    }
}
