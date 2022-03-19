namespace Pal.Api.Services;

public class DefaultPalNumberStrategy
{
    private readonly ILogger<DefaultPalNumberStrategy> logger;

    public DefaultPalNumberStrategy(ILogger<DefaultPalNumberStrategy> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<bool> IsPalindromicAsync(int num, int @base)
    {
        var input = Convert.ToString(num, @base);

        logger.LogDebug("Processing input {@Input} of {@Value} with base {@Base}", input, num, @base);

        var chars = new char[input.Length];
        var len = chars.Length - 1;

        for (int i = 0; i <= len; i++)
        {
            chars[i] = input[len - i];
        }

        return Task.FromResult(string.CompareOrdinal(input, new string(chars)) == 0);
    }
}
