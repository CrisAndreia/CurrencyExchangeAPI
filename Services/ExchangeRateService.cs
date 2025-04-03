using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Globalization;
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
        
                // Configure XML namespaces
                /*var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("gesmes", "http://www.gesmes.org/xml/2002-08-01");
                namespaces.Add("", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");*/

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
                // Namespace handling
                /*var ns = XNamespace.Get(ECB_API_URL);
                //var gesmes = XNamespace.Get("http://www.gesmes.org/xml/2002-08-01");

                // Get reference date
                var timeCube = doc.Descendants("Cube")
                                 .FirstOrDefault(x => x.Attribute("time") != null);
                
                if (timeCube == null)
                {
                    throw new InvalidOperationException("Could not find time reference in ECB feed");
                }

                var timeAttribute = timeCube.Attribute("time");
                if (timeAttribute == null || !DateTime.TryParse(timeAttribute.Value, out var date))
                {
                    throw new InvalidOperationException("Invalid or missing time attribute");
                }

                // Get all Cube elements with currency and rate attributes
                var rateElements = doc.Descendants("Cube")
                                    .Where(x => x.Attribute("currency") != null && 
                                              x.Attribute("rate") != null);

                var exchangeRates = new List<ExchangeRate>();
                foreach (var element in rateElements)
                {
                    var currency = element.Attribute("currency").Value;
                    var rateValue = element.Attribute("rate").Value;
                    
                    if (decimal.TryParse(rateValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var rate))
                    {
                        exchangeRates.Add(new ExchangeRate
                        {
                            BaseCurrency = "EUR",
                            TargetCurrency = currency,
                            Rate = rate,
                            DateReceived = date
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Skipping invalid rate value: {RateValue} for {Currency}", 
                                        rateValue, currency);
                    }
                }

                if (!exchangeRates.Any())
                {
                    throw new InvalidOperationException("No valid rates found in ECB feed");
                }

                // Add EUR as base currency (1 EUR = 1 EUR)
                exchangeRates.Add(new ExchangeRate 
                { 
                    BaseCurrency = "EUR", 
                    TargetCurrency = "EUR",
                    Rate = 1m, 
                    DateReceived = date 
                });

                await _exchangeRateRepository.AddRangeAsync(exchangeRates);
                _logger.LogInformation("Successfully stored {Count} exchange rates", exchangeRates.Count);
                
                return exchangeRates;
            }*/
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch and store exchange rates");
                throw;
            }
        }
            /*var response = await _httpClient.GetStringAsync(ECB_API_URL);

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
                    BaseCurrency = "EUR",
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
        }*/
    }
}