using MedicalSystem.Application.Interfaces.Services;
using MedicalSystem.Domain.Models;
using MedicalSystem.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MedicalSystem.Infrastructure.TimerSchedulers
{
    public class EmailNotification : CronJobExtensions
    {
        private readonly ILogger<EmailNotification> _logger;
        public EmailNotification(IScheduleConfig<EmailNotification> config, IServiceProvider serviceProvider, ILogger<EmailNotification> logger)
        : base(config.CronExpression, config.TimeZoneInfo, serviceProvider)
        {
            _logger = logger;
        }

        public override async Task DoWork(IServiceScope scope, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Send notification to patients with tomorrow appointments!!");

            try
            {
                // Por ser uma instância singleton mantêm instâncias de suas dependências durante todo o ciclo de vida da aplicação.
                // Se uma das dependências precisa ser escopo (como DbContext),
                // pode ocorrer um problema. Para resolver isso, é pego o scopo enviado no método DoWork para recuperar os serviços:
                var appointmentService = scope.ServiceProvider.GetRequiredService<IAppointmentService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                DateTime tomorrow = DateTime.Now.Date.AddDays(1);
                var appoitments = await appointmentService.GetAppointmentByDateAsync(tomorrow);

                foreach (var appointment in appoitments)
                {
                    var email = new Email
                    {
                        To = appointment.Patient.Email,
                        Subject = "Lembrete consulta médica!!!",
                        Body = $"Este email está sendo enviado para lembrar que o senhor possui uma consulta" +
                        $" médica com o doutor {appointment.Doctor.FirstName} {appointment.Doctor.LastName} no dia {appointment.Date}"
                    };

                    await emailService.SendEmail(email);

                    _logger.LogInformation($"Email sent to {email.To}");

                }

                return;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send email!!", ex);
            }
            
        }
    }
}
