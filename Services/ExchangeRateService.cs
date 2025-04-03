using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CurrencyExchangeAPI.Data;
using CurrencyExchangeAPI.Models;
using Microsoft.Extensions.Logging;
using CurrencyExchangeAPI.Repositories;
using System.Xml.Linq;

namespace CurrencyExchangeAPI.Services
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly ILogger<ExchangeRateService> _logger;
        private readonly IExchangeRateRepository _exchangeRateRepository;
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private const string ECB_API_URL = "https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";
        public ExchangeRateService(IExchangeRateRepository exchangeRateRepository, HttpClient httpClient, ILogger<ExchangeRateService> logger, ApplicationDbContext context)
        {
            _exchangeRateRepository = exchangeRateRepository;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
            _context = context;
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
            try
            {
                _logger.LogInformation("Fetching exchange rates from ECB");
                var response = await _httpClient.GetStringAsync(ECB_API_URL);
                var doc = XDocument.Parse(response);
                _logger.LogInformation($"DOC: {doc}");

                var serializer = new XmlSerializer(typeof(Envelope));
                Envelope envelope;

                using (var stringReader = new StringReader(response))
                {
                    using (var xmlReader = XmlReader.Create(stringReader))
                    {
                        envelope = (Envelope)serializer.Deserialize(xmlReader);
                    }
                }

                // Extract the date from the response
                if (envelope?.RootCube?.TimeCubes == null || !envelope.RootCube.TimeCubes.Any())
                {
                    throw new InvalidOperationException("No time cubes found in ECB response");
                }

                var timeCube = envelope.RootCube.TimeCubes.First();
                if (!DateTime.TryParse(timeCube.Time, out var date))
                {
                    throw new InvalidOperationException("Invalid date format in ECB response");
                }

                // Convert to ExchangeRate objects
                var exchangeRates = new List<ExchangeRate>();
                
                if (timeCube.CurrencyRates != null)
                {
                    foreach (var currencyRate in timeCube.CurrencyRates)
                    {
                        exchangeRates.Add(new ExchangeRate
                        {
                            BaseCurrency = "EUR",
                            TargetCurrency = currencyRate.Currency,
                            Rate = currencyRate.Rate,
                            DateReceived = date
                        });
                    }
                }

                // Add EUR as base currency (1 EUR = 1 EUR)
                exchangeRates.Add(new ExchangeRate 
                { 
                    BaseCurrency = "EUR", 
                    TargetCurrency = "EUR",
                    Rate = 1m, 
                    DateReceived = date 
                });

                // Save to database
                await _exchangeRateRepository.AddRangeAsync(exchangeRates);
                _logger.LogInformation("Successfully stored {Count} exchange rates", exchangeRates.Count);
                
                return exchangeRates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch and store exchange rates");
                throw;
            }
        }
    }
}