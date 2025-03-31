using System.Threading.Tasks;
using CurrencyExchangeAPI.Models;
using System.Collections.Generic;

/*public interface IWalletRepository
{
    Task<Wallet> CreateAsync(Wallet wallet);
    Task<Wallet> GetByIdAsync(int id);
    Task<IEnumerable<Wallet>> GetAllAsync();
    Task<Wallet> UpdateAsync(Wallet wallet);
    Task DeleteAsync(int id);
}*/

namespace CurrencyExchangeAPI.Repositories
{
    public interface IWalletRepository
    {
        Task<IEnumerable<Wallet>> GetAllAsync();
        Task<Wallet> GetByIdAsync(int id);
        Task<Wallet> CreateAsync(Wallet wallet); // Método que precisa ser implementado
        Task<Wallet> UpdateAsync(Wallet wallet);
        Task DeleteAsync(int id);
    }
}