using System.Threading.Tasks;
using CurrencyExchangeAPI.Models;
using CurrencyExchangeAPI.Dto;

namespace CurrencyExchangeAPI.Services
{
    public interface IWalletService
    {
        Task<Wallet> CreateWalletAsync(WalletDto walletCreateDTO);
        Task<Wallet> GetWalletByIdAsync(int id);
        //Task<WalletBalance> AddBalanceToWalletAsync(int walletId, WalletBalanceDto balanceDto);
        //Task<WalletBalance> GetBalanceByCurrencyAsync(int walletId, string currency);
        //Task<WalletBalance> UpdateWalletBalanceAsync(int walletId, WalletBalanceDto balanceDto);
        //Task<WalletBalance> DepositAsync(int walletId, WalletBalanceDto depositDto);
        //Task<WalletBalance> WithdrawAsync(int walletId, WalletBalanceDto withdrawDto);

    }
}
