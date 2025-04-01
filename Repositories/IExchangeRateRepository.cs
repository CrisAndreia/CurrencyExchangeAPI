using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyExchangeAPI.Models;

namespace CurrencyExchangeAPI.Repositories
{
    public interface IExchangeRateRepository
    {
        Task<IEnumerable<ExchangeRate>> GetAllAsync();
        Task<ExchangeRate> GetByCurrencyAsync(string currency);
        Task AddAsync(ExchangeRate exchangeRate);
        Task AddRangeAsync(IEnumerable<ExchangeRate> exchangeRates);
        
    }
}