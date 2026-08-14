using Microsoft.Extensions.DependencyInjection;

namespace WarehouseFlow.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Example: register use cases, CQRS handlers, mediators
            // services.AddScoped<IOrderService, OrderService>();

            return services;
        }
    }
}
