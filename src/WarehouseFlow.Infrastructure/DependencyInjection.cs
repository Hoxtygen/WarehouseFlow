using Microsoft.Extensions.DependencyInjection;

namespace WarehouseFlow.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Example: register repositories, EF DbContext, external services
            // services.AddScoped<IProductRepository, ProductRepository>();
            // services.AddDbContext<AppDbContext>(options => ...);

            return services;
        }
    }
}
