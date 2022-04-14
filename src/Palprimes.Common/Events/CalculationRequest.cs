namespace Palprimes.Common.Events
{
    public class CalculationRequest
    {
        public string ClientId { get; set; }

        public int Number { get; init; }
    }
}
