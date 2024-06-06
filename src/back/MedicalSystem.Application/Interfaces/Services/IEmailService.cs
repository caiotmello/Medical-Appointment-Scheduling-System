using MedicalSystem.Domain.Models;

namespace MedicalSystem.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmail(Email email);
    }
}
