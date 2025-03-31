using System.Threading.Tasks;
using CurrencyExchangeAPI.Models;
using System.Collections.Generic;

namespace CurrencyExchangeAPI.Repositories
{
    public interface IWalletRepository
    {
        Task<IEnumerable<Wallet>> GetAllAsync();
        Task<Wallet> GetByIdAsync(int id);
        Task<Wallet> CreateAsync(Wallet wallet); 
        Task<Wallet> UpdateAsync(Wallet wallet);
        Task DeleteAsync(int id);
    }
}