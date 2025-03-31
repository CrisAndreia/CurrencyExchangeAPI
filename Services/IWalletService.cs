using System.Threading.Tasks;
using CurrencyExchangeAPI.Models;

namespace CurrencyExchangeAPI.Services
{
    public interface IWalletService
    {
        Task<Wallet> CreateWalletAsync(WalletDto walletCreateDTO);
        Task<Wallet> GetWalletByIdAsync(int id);
    }
}
