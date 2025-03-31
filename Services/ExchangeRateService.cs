using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CurrencyExchangeAPI.Data;
using CurrencyExchangeAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace CurrencyExchangeAPI.Services
{
    public class ExchangeRateService 
    {
        private readonly ILogger<ExchangeRateService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private const string ECB_API_URL = "https://v6.exchangerate-api.com/v6/8aa51a89e70ce8f2bd498467/latest/USD";

        public ExchangeRateService(ApplicationDbContext context, HttpClient httpClient, ILogger<ExchangeRateService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        public async Task<List<ExchangeRate>> FetchAndStoreExchangeRatesAsync()
        {
            var response = await _httpClient.GetStringAsync(ECB_API_URL);

            var ratesData = JsonConvert.DeserializeObject<ExchangeRatesResponse>(response);

             if (ratesData?.Result != "success")
            {
                throw new InvalidOperationException($"API response error: {ratesData?.Result}");
            }

            if (ratesData?.ConversionRates == null || ratesData.ConversionRates.Count == 0)
            {
                throw new Exception("No exchange rates available in the response.");
            }
            
            var exchangeRates = new List<ExchangeRate>();
            
            foreach(var (currency, rate) in ratesData.ConversionRates)
            {
                exchangeRates.Add(new ExchangeRate
                {
                    BaseCurrency = "USD",
                    TargetCurrency = currency,
                    Rate = rate,
                    DateReceived = DateTime.UtcNow
                });
            }

            if(exchangeRates == null || !exchangeRates.Any())
            {
                throw new Exception("No exchange rates to store.");
            }


            await _context.ExchangeRates.AddRangeAsync(exchangeRates);
            await _context.SaveChangesAsync();

            return exchangeRates;
        }
    }
}