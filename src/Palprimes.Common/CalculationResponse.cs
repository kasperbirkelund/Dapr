using System;

namespace Palprimes.Common
{
    public class CalculationResponse
    {
        public string ClientId { get; set; }      
        public int Number { get; set; }
        public bool Result { get; set; }
        public CalculationResultType Type { get; set; }
    }
}