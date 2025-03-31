using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyExchangeAPI.Data;
using CurrencyExchangeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeAPI.Repositories
{
    public class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly ApplicationDbContext _context;

        public ExchangeRateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExchangeRate>> GetAllAsync()
        {
            return await _context.ExchangeRates.ToListAsync();
        }

        public async Task<ExchangeRate> GetByCurrencyAsync(string currency)
        {
            return await _context.ExchangeRates.FirstOrDefaultAsync(e => e.BaseCurrency == currency);
        }

        public async Task AddAsync(ExchangeRate exchangeRate)
        {
            _context.ExchangeRates.Add(exchangeRate);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<ExchangeRate> exchangeRates)
        {
            await _context.ExchangeRates.AddRangeAsync(exchangeRates);
            await _context.SaveChangesAsync();
        }

    }
}