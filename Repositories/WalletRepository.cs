using System.Threading.Tasks;
using CurrencyExchangeAPI.Data;
using CurrencyExchangeAPI.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace CurrencyExchangeAPI.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _context;

        public WalletRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet> AddAsync(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<Wallet> GetByIdAsync(int id)
        {
            return await _context.Wallets.Include(w => w.Balance).FirstOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Wallet> CreateAsync(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);  // Adds the new wallet to the context
            await _context.SaveChangesAsync();        // Save the changes on the DB
            return wallet;  // Returns the new wallet
        }

        public async Task<IEnumerable<Wallet>> GetAllAsync()
        {
            return await _context.Wallets.ToListAsync();
        }

        public async Task<Wallet> UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task DeleteAsync(int id)
        {
            var wallet = await GetByIdAsync(id);
            if (wallet != null)
            {
                _context.Wallets.Remove(wallet);
                await _context.SaveChangesAsync();
            }
        }
    }
}
