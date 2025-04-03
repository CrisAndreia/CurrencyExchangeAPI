using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyExchangeAPI.Models;

namespace CurrencyExchangeAPI.Services
{
    public interface IECBService
    {
        Task<List<ExchangeRate>> GetLatestRatesAsync();
    }
}