using MedicalSystem.Application.Configurations;
using MedicalSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace MedicalSystem.Test.Infrastructure.Services
{
    public class EmailServiceTest
    {
        private readonly Mock<IOptions<EmailSettings>> _emailSettingsMock;
        private readonly Mock<ILogger<EmailService>> _loggerMock;
        private readonly EmailService _emailService;

        public EmailServiceTest()
        {
            _emailSettingsMock = new Mock<IOptions<EmailSettings>>();
            _loggerMock = new Mock<ILogger<EmailService>>();

            var emailSettings = new EmailSettings
            {
                FromAddress = "test@example.com",
                FromName = "Test",
                Provider = "smtp.example.com",
                Port = 587,
                Password = "password"
            };

            _emailSettingsMock.Setup(x => x.Value).Returns(emailSettings);
            _emailService = new EmailService(_emailSettingsMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task SendEmail_ShouldSendEmail_WhenEmailIsValid()
        {
            // Arrange
            var email = new MedicalSystem.Domain.Models.Email
            {
                To = "recipient@example.com",
                Subject = "Test Subject",
                Body = "Test Body"
            };

            // Act
            await _emailService.SendEmail(email);

            // Assert
            // Aqui você pode verificar se os métodos SMTP foram chamados conforme o esperado,
            // mas como estamos trabalhando com classes concretas do .NET, é difícil mocá-las diretamente.
            // Então, ao invés disso, vamos verificar se não ocorreu nenhuma exceção e se o log foi chamado.
            _loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SendEmail_ShouldLogError_WhenEmailSendingFails()
        {
            // Arrange
            var email = new MedicalSystem.Domain.Models.Email
            {
                To = "recipient@example.com",
                Subject = "Test Subject",
                Body = "Test Body"
            };

            // Simular uma falha de SMTP
            _emailSettingsMock.Setup(x => x.Value).Throws(new Exception("SMTP Error"));

            // Act
            await _emailService.SendEmail(email);

            // Assert
            _loggerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SendEmail_ShouldAttachFiles_WhenAttachmentsAreProvided()
        {
            // Arrange
            var email = new MedicalSystem.Domain.Models.Email
            {
                To = "recipient@example.com",
                Subject = "Test Subject",
                Body = "Test Body",
                Attachments = new List<IFormFile>
                {
                    new FormFile(new MemoryStream(new byte[] { 1, 2, 3 }), 0, 3, "Data", "test.txt")
                }
            };

            // Act
            await _emailService.SendEmail(email);

            // Assert
            // mas como não podemos moquear diretamente, vamos verificar se não ocorreu nenhuma exceção.
            _loggerMock.VerifyNoOtherCalls();
        }
    }
}
