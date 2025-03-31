using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace CurrencyExchangeAPI.Models
{
    public class ExchangeRatesResponse
    {
        [JsonProperty("result")]
        public string Result  {get; set; }
        
        [JsonProperty("base_code")]
        public string BaseCode { get; set; }

        [JsonProperty("conversion_rates")]
        public Dictionary<string, decimal> ConversionRates { get; set; }
    }
}
