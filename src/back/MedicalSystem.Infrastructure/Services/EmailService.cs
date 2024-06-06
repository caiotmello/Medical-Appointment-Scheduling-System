using Azure.Core;
using MedicalSystem.Application.Configurations;
using MedicalSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace MedicalSystem.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public EmailSettings _emailSettings { get; }
        public ILogger<EmailService> _logger { get; }

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public Task SendEmail(Domain.Models.Email email)
        {
            try
            {               
                SmtpClient smtp = new SmtpClient();
                MailMessage message = new MailMessage();


                message.From = new MailAddress(_emailSettings.FromAddress, _emailSettings.FromName);
                message.To.Add(new MailAddress(email.To));
                message.Subject = email.Subject;
                message.Body = $"<html><body>{email.Body}</body></html>";
                message.IsBodyHtml = true;

                if (email.Attachments != null)
                {
                    foreach (var file in email.Attachments)
                    {
                        if (file.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                var fileBytes = ms.ToArray();
                                Attachment att = new Attachment(new MemoryStream(fileBytes), file.FileName);
                                message.Attachments.Add(att);
                            }
                        }
                    }
                }

                var smtpClient = new SmtpClient(_emailSettings.Provider)
                {
                    Port = _emailSettings.Port,
                    Credentials = new NetworkCredential(_emailSettings.FromAddress, _emailSettings.Password),
                    EnableSsl = true,
                };
                
                smtpClient.SendCompleted += OnSmtpClientSendCompleted;
                smtpClient.SendAsync(message,null);

                
            }
            catch(Exception ex)
            {
                _logger.LogError("Email sending failed.");
            }

            return Task.CompletedTask;
        }

        private void OnSmtpClientSendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e?.Error != null)
            {
                _logger.LogError("[Sending email]", e.Error.ToString());
            }
        }
    }
}
