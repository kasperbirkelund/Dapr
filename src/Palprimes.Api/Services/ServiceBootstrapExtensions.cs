namespace Palprimes.Api.Services
{
    public static class ServiceBootstrapExtensions
    {
        public static IServiceCollection AddPalprimesServices(this IServiceCollection services)
        {
            return services.AddSingleton<StateManagementService>();
        }
    }
}