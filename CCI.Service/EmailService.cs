using CCI.Common.Extensions;
using CCI.Model.Options;
using CCI.Service.Contractors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Events;
using System.Net;
using System.Net.Mail;

namespace CCI.Service
{
    public class EmailService : IEmailService
    {
        private readonly SmtpOption _smtp;
        private readonly ILogger<EmailService> _logger;
        private const string ClassName = nameof(EmailService);
        public EmailService(IOptions<SmtpOption> options,
            ILogger<EmailService> logger)
        {
            _logger = logger;
            this._smtp = options.Value;
        }

        public void SendEmail(string toEmail, string subject, string body, bool isHtml = false)
        {
            using var client = new SmtpClient(_smtp.Server, _smtp.Port);
            client.EnableSsl = _smtp.EnableSsl; // Use TLS
            client.Credentials = new NetworkCredential(_smtp.User, _smtp.Password);

            var fromAddress = new MailAddress(_smtp.FromEmail, _smtp.DisplayName);
            var toAddress = new MailAddress(toEmail);

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            try
            {
                client.Send(message);
                _logger.LogInformation("Email sent successfully".GeneratedLog(ClassName, LogEventLevel.Information));
            }
            catch (SmtpException ex)
            {
                _logger.LogError($"Failed to send email: {ex.Message}".GeneratedLog(ClassName, LogEventLevel.Error));
            }

        }
    }
}
