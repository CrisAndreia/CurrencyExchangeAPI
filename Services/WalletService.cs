using System;
using System.Threading.Tasks;
using CurrencyExchangeAPI.Models;
using CurrencyExchangeAPI.Repositories;
using CurrencyExchangeAPI.Dto;

namespace CurrencyExchangeAPI.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<Wallet> GetWalletByIdAsync(int id){
            return await _walletRepository.GetByIdAsync(id);
        }

        // 
        public async Task<Wallet> CreateWalletAsync(WalletDto walletDto)
        {
            var wallet = new Wallet
            {
                UserId = walletDto.UserId,
                Balance = walletDto.Balance
            };

            return await _walletRepository.CreateAsync(wallet);
        }
    }
}
