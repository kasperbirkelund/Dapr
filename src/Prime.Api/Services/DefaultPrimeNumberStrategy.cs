namespace Prime.Api.Services;

using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class DefaultPrimeNumberStrategy
{
    private readonly ILogger<DefaultPrimeNumberStrategy> logger;

    public DefaultPrimeNumberStrategy(ILogger<DefaultPrimeNumberStrategy> logger)
    {
        this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
    }

    public Task<bool> IsPrimeAsync(int num)
    {
        logger.LogDebug("Default processing value {@Value}", num);

        if (num <= 1) return Task.FromResult(false);

        int i;
        int m = num / 2;

        for (i = 2; i <= m; i++)
        {
            if (num % i == 0)
            {
                return Task.FromResult(false);
            }
        }

        return Task.FromResult(true);
    }
}

