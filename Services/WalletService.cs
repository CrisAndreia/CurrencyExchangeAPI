using System.Threading.Tasks;
using CurrencyExchangeAPI.Models;
using CurrencyExchangeAPI.Data;
using CurrencyExchangeAPI.Services;
using CurrencyExchangeAPI.Repositories;

namespace CurrencyExchangeAPI.Services
{
    public class WalletService : IWalletService
    {
        //private readonly ApplicationDbContext _context;
        private readonly IWalletRepository _walletRepository;

        /*public WalletService(ApplicationDbContext context)
        {
            _context = context;
        }*/

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }

        public async Task<Wallet> GetWalletByIdAsync(int id){
            return await _walletRepository.GetByIdAsync(id);
        }

        public async Task<Wallet> CreateWalletAsync(WalletDto walletDto)
        {
            var wallet = new Wallet
            {
                Currency = walletDto.Currency,
                Balance = walletDto.Balance,
                CreateAt = walletDto.CreateAt
            };

            /*_context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            return wallet;*/
            return await _walletRepository.CreateAsync(wallet);
        }
        
    }
}
