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
            _logger.LogInformation($"vai chamar a API: {ECB_API_URL}");
            //var response = await _httpClient.GetAsync(ECB_API_URL);
            var response = await _httpClient.GetStringAsync(ECB_API_URL);
            _logger.LogInformation($"RESPONSE: {response}");

            var ratesData = JsonConvert.DeserializeObject<ExchangeRatesResponse>(response);
             _logger.LogInformation($"RATES DATA: {ratesData}");

             if (ratesData?.Result != "success")
            {
                throw new InvalidOperationException($"API response error: {ratesData?.Result}");
            }

            _logger.LogInformation($"CONVERSION RATES: {ratesData.ConversionRates}");
            if (ratesData?.ConversionRates == null || ratesData.ConversionRates.Count == 0)
            {
                throw new Exception("No exchange rates available in the response.");
            }
            //_logger.LogInformation($"Content-Type: {response.Content.Headers.ContentType}");

            //string responseString = await response.Content.ReadAsStringAsync();
            //_logger.LogInformation($"Raw api response: {responseString}");
            
            /*if(!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch exchange rates from ECB.");
            }*/

            //Parse the response as JSON
            /*var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"API Response: {response.Content}");

            if(string.IsNullOrEmpty(responseContent))
            {
                throw new Exception("No data received from the API.");
            }*/

            //var ratesData = JsonConvert.DeserializeObject<ExchangeRatesResponse>(responseContent);//await response.Content.ReadFromJsonAsync<EcbRatesResponse>();

            /*if (ratesData?.ConversionRates == null || !ratesData.ConversionRates.Any())
            {
                throw new Exception("No exchange rates available in the response.");
            }
            if(ratesData == null || !ratesData.Rates.Any())
            {
                throw new Exception("No exchange rates received from ECB.");
            }*/

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

            /*if(!exchangeRates.Any())
            {
                throw new Exception("Exchange rates list is empty.");
            }*/

            await _context.ExchangeRates.AddRangeAsync(exchangeRates);
            await _context.SaveChangesAsync();

            return exchangeRates;
        }

        /*public class ExchangeRateApiResponse
        {
            public string Result { get; set; }
            public Dictionary<string, decimal> ConversionRates { get; set; }
            
        }*/
    }
}