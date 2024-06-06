using MedicalSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace MedicalSystem.Infrastructure.Persistence
{
    public class IdentityDataContextSeed
    {
        private readonly IUserService _userService;

        public static async Task SeedAsync(IdentityDataContext dbContext, ILogger<IdentityDataContextSeed> logger, IUserService userService)
        {
            //if (!orderContext.Orders.Any())
            //{
            //    orderContext.Orders.AddRange(GetPreconfiguredOrders());
            //    await orderContext.SaveChangesAsync();
            //    logger.LogInformation("Seed database associated with context {DbContextName}", typeof(OrderContext).Name);
            //}

            await userService.CreateRolesAsync();
        }

    }
}
