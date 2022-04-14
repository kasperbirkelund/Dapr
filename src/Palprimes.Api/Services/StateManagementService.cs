using Dapr.Client;
using Palprimes.Api.Model;
using Palprimes.Common;
using Palprimes.Common.Events;

namespace Palprimes.Api.Services
{
    public class StateManagementService
    {
        private const string StoreName = "statestore";
        private readonly ILogger<StateManagementService> _logger;
        private readonly StateOptions _stateOptions = new StateOptions { Concurrency = ConcurrencyMode.FirstWrite, Consistency = ConsistencyMode.Strong };

        public StateManagementService(ILogger<StateManagementService> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PalprimesResult> SetStateAsync(CalculationResponse response)
        {
            using var client = new DaprClientBuilder().Build();

            var stateKey = response.Number.ToString();
            var (state, etag) = await client.GetStateAndETagAsync<PalprimesResult>(StoreName, stateKey);
            if (state == null) state = new PalprimesResult { Number = response.Number };

            SetState(response, state);

            //We know there are only two competing resources so no need for recursion.
            if (!await client.TrySaveStateAsync(StoreName, stateKey, state, etag, _stateOptions))
            {
                _logger.LogInformation("State concurrency violation.");
                (state, etag) = await client.GetStateAndETagAsync<PalprimesResult>(StoreName, stateKey);
                SetState(response, state);
                await client.TrySaveStateAsync(StoreName, stateKey, state, etag, _stateOptions);
            }

            return state;
        }

        public async Task ResetStateAsync(PalprimesResult state)
        {
            using var client = new DaprClientBuilder().Build();
            await client.DeleteStateAsync(StoreName, state.Number.ToString(), _stateOptions);
        }

        private static void SetState(CalculationResponse response, PalprimesResult state)
        {
            if (response.Type == CalculationResultType.Pal)
            {
                state.IsPal = response.Result;
            }
            else
            {
                state.IsPrime = response.Result;
            }
        }
    }
}