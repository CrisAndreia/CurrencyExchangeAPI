using System.Threading.Tasks;
using CurrencyExchangeAPI.Models;
using CurrencyExchangeAPI.Dto;

namespace CurrencyExchangeAPI.Services
{
    public interface IWalletService
    {
        Task<Wallet> CreateWalletAsync(WalletDto walletCreateDTO);
        Task<Wallet> GetWalletByIdAsync(int id);
    }
}
