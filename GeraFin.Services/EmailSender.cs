using System.Threading.Tasks;
using GeraFin.InterFaces.Factory;
using Microsoft.Extensions.Options;

namespace GeraFin.Services
{
    public class EmailSender : IEmailSender
    {
        //dependency injection
        private SendGridOptions _sendGridOptions { get; }
        private IFunctional _functional { get; }
        private SmtpOptions _smtpOptions { get; }

        public EmailSender(IOptions<SendGridOptions> sendGridOptions, IFunctional functional, IOptions<SmtpOptions> smtpOptions)
        {
            _sendGridOptions = sendGridOptions.Value;
            _functional = functional;
            _smtpOptions = smtpOptions.Value;
        }

        Task IEmailSender.SendEmailAsync(string email, string subject, string message)
        {
            if (_sendGridOptions.IsDefault)
            {
                _functional.SendEmailBySendGridAsync(
                    _sendGridOptions.SendGridKey,
                    _sendGridOptions.FromEmail,
                    _sendGridOptions.FromFullName,
                    subject,
                    message,
                    email).Wait();
            }
            //smtp is become default
            if (_smtpOptions.IsDefault)
            {
                _functional.SendEmailByGmailAsync(
                    _smtpOptions.FromEmail,
                    _smtpOptions.FromFullName,
                    subject,
                    message,
                    email,
                    email,
                    _smtpOptions.SmtpUserName,
                    _smtpOptions.SmtpPassword,
                    _smtpOptions.SmtpHost,
                    _smtpOptions.SmtpPort,
                    _smtpOptions.SmtpSSL).Wait();
            }
            return Task.CompletedTask;
        }

        public Task SendEmailConfirmationAsync(string email, object callbackUrl)
        {
            throw new System.NotImplementedException();
        }
    }
}
