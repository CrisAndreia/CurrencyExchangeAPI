using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyExchangeAPI.Models;

namespace CurrencyExchangeAPI.Services
{
    public interface IExchangeRateService
    {
        Task<IEnumerable<ExchangeRate>> GetExchangeRatesAsync();
        Task<ExchangeRate> GetExchangeRateByCurrencyAsync(string currency);
        Task SaveExchangeRateAsync(ExchangeRate exchangeRate);
        Task<List<ExchangeRate>> FetchAndStoreExchangeRatesAsync();
    }
}