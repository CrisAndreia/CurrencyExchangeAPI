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
using CurrencyExchangeAPI.Repositories;

namespace CurrencyExchangeAPI.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly ILogger<ExchangeRateService> _logger;
        private readonly IExchangeRateRepository _exchangeRateRepository;
        private readonly HttpClient _httpClient;
        private const string ECB_API_URL = "https://v6.exchangerate-api.com/v6/8aa51a89e70ce8f2bd498467/latest/USD";
        public ExchangeRateService(IExchangeRateRepository exchangeRateRepository, HttpClient httpClient, ILogger<ExchangeRateService> logger)
        {
            _exchangeRateRepository = exchangeRateRepository;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        public async Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync()
        {
            return await _exchangeRateRepository.GetAllAsync();
        }

        public async Task<ExchangeRate> GetExchangeRateByCurrencyAsync(string currency)
        {
            return await _exchangeRateRepository.GetByCurrencyAsync(currency);
        }

        public async Task SaveExchangeRateAsync(ExchangeRate exchangeRate)
        {
            await _exchangeRateRepository.AddAsync(exchangeRate);
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

            foreach (var (currency, rate) in ratesData.ConversionRates)
            {
                exchangeRates.Add(new ExchangeRate
                {
                    BaseCurrency = "USD",
                    TargetCurrency = currency,
                    Rate = rate,
                    DateReceived = DateTime.UtcNow
                });
            }

            if (exchangeRates == null || !exchangeRates.Any())
            {
                throw new Exception("No exchange rates to store.");
            }

            // Adding to the db through the repository
            await _exchangeRateRepository.AddRangeAsync(exchangeRates);

            return exchangeRates;
        }
    }
}