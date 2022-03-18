namespace Palprimes.Api.Services
{
    public static class ServiceBootstrapExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<NotificationService>();
            services.AddSingleton<StateManagementService>();

            return services;
        }
    }
}