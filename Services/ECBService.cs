using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using CurrencyExchangeAPI.Models;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace CurrencyExchangeAPI.Services
{
    public class ECBService : IECBService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ECBService> _logger;
        private const string ECB_URL = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";

        public ECBService(HttpClient httpClient, ILogger<ECBService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<ExchangeRate>> GetLatestRatesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching latest rates from ECB");
                var response = await _httpClient.GetAsync(ECB_URL);
                response.EnsureSuccessStatusCode();

                var xml = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Received ECB response: {Xml}", xml);
                
                var doc = XDocument.Parse(xml);

                var rateElements = doc.Descendants("Cube")
                            .Where(x => x.Attribute("currency") != null && 
                                      x.Attribute("rate") != null);

                var rates = new List<ExchangeRate>();
                foreach (var element in rateElements)
                {
                    var currency = element.Attribute("currency").Value;
                    var rateValue = element.Attribute("rate").Value;
                    
                    if (decimal.TryParse(rateValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate))
                    {
                        rates.Add(new ExchangeRate
                        {
                            BaseCurrency = currency,
                            Rate = rate,
                            DateReceived = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Skipping invalid rate value: {RateValue} for {Currency}", 
                                        rateValue, currency);
                    }
                }

                // Add EUR as base currency (1 EUR = 1 EUR)
                rates.Add(new ExchangeRate 
                { 
                    BaseCurrency = "EUR", 
                    Rate = 1m, 
                    DateReceived = DateTime.UtcNow 
                });
                
                _logger.LogInformation("Successfully parsed {Count} currency rates", rates.Count);
                return rates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ECB rates");
                throw;
            }
        }
    }
}