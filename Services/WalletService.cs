using System;
using System.Threading.Tasks;
using CurrencyExchangeAPI.Models;
using CurrencyExchangeAPI.Data;
using CurrencyExchangeAPI.Services;
using CurrencyExchangeAPI.Repositories;
using CurrencyExchangeAPI.Dto;
using System.Collections.Generic;
using System.Linq;

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
                Balance = walletDto.Balance//new List<WalletBalance>()
            };

            /*foreach (var balanceDto in walletDto.Balance)
            {
                wallet.Balances.Add(new WalletBalance
                {
                    Currency = balanceDto.Currency,
                    Balance = balanceDto.Balance
                });
            }*/
            return await _walletRepository.CreateAsync(wallet);
        }
        
        // Adding balance to an existing wallet
        /*public async Task<WalletBalance> AddBalanceToWalletAsync(int walletId, WalletBalanceDto balanceDto)
        {
            var wallet = await _walletRepository.GetByIdAsync(walletId);
            if (wallet == null)
                throw new KeyNotFoundException("Wallet not found");

            var balance = wallet.Balances.Find(b => b.Currency == balanceDto.Currency);
            if(balance != null)
                balance.Balance += balanceDto.Balance;

            else
            {
                var newBalance = new WalletBalance
                {
                    Currency = balanceDto.Currency,
                    Balance = balanceDto.Balance,
                    WalletId = walletId
                };

                wallet.Balances.Add(newBalance);
            }

            await _walletRepository.UpdateAsync(wallet);

            return balance;
        }

        // Updates balance in an existing wallet
        public async Task<WalletBalance> UpdateWalletBalanceAsync(int walletId, WalletBalanceDto balanceDto)
        {
            var wallet = await _walletRepository.GetByIdAsync(walletId);
            if (wallet == null)
                throw new KeyNotFoundException("Wallet not found");

            var balance = wallet.Balances.Find(b => b.Currency == balanceDto.Currency);
            if (balance == null)
                throw new KeyNotFoundException("Balance for the specified currency not found");

            balance.Balance = balanceDto.Balance;
            await _walletRepository.UpdateAsync(wallet);

            return balance;
        }

        // To deposit into a wallet
        public async Task<WalletBalance> DepositAsync(int walletId, WalletBalanceDto depositDto)
        {
            var wallet = await _walletRepository.GetByIdAsync(walletId);
            if (wallet == null)
                throw new KeyNotFoundException("Wallet not found");

            var balance = wallet.Balances.FirstOrDefault(b => b.Currency == depositDto.Currency);

            if (balance == null)
            {
                // if there isn't a balance with this currency in the wallet yet, it creates a new balance for the new currency
                balance = new WalletBalance
                {
                    WalletId = walletId,
                    Currency = depositDto.Currency,
                    Balance = depositDto.Balance
                };
                wallet.Balances.Add(balance);
            }
            else
            {
                // if there is a balance with this curency, it adds to the balance
                balance.Balance += depositDto.Balance;
            }

            return await UpdateWalletBalanceAsync(walletId, new WalletBalanceDto
            {
                Currency = balance.Currency,
                Balance = balance.Balance
            });
        }

        public async Task<WalletBalance> WithdrawAsync(int walletId, WalletBalanceDto withdrawDto)
        {
            var wallet = await _walletRepository.GetByIdAsync(walletId);
            if (wallet == null)
                throw new KeyNotFoundException("Wallet not found");

            var balance = wallet.Balances.FirstOrDefault(b => b.Currency == withdrawDto.Currency);
            if (balance == null)
                throw new KeyNotFoundException("Balance for the specified currency not found");

            if (balance.Balance < withdrawDto.Balance)
                throw new InvalidOperationException("Insufficient funds");

            balance.Balance -= withdrawDto.Balance;

            return await UpdateWalletBalanceAsync(walletId, new WalletBalanceDto
            {
                Currency = balance.Currency,
                Balance = balance.Balance
            });
        }*/

    }
}
