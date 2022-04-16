namespace Prime.Api.Services
{
    public static class ServiceBootstrapExtensions
    {
        public static IServiceCollection AddPrimeServices(this IServiceCollection services)
        {
            return services.AddSingleton<DefaultPrimeNumberStrategy>();
        }
    }
}