namespace Pal.Api.Services
{
    public static class ServiceBootstrapExtensions
    {
        public static IServiceCollection AddPalServices(this IServiceCollection services)
        {
            return services.AddScoped<DefaultPalNumberStrategy>();
        }
    }
}