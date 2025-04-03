using Microsoft.AspNetCore.Mvc;
using CurrencyExchangeAPI.Services;
using CurrencyExchangeAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyExchangeAPI.Dto;
using CurrencyExchangeAPI.Data;
using System;
using System.Linq;

namespace CurrencyExchangeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ApplicationDbContext _context;
        private readonly IECBService _ecbService;

        public WalletController(IWalletService walletService, ApplicationDbContext context, IECBService ecbService)
        {
            _walletService = walletService;
            _context = context;
            _ecbService = ecbService;
        }

        [HttpPost]
        public async Task<ActionResult<Wallet>> CreateWallet([FromBody] CreateWalletRequest request)
        {
            var wallet = new Wallet
            {
                UserId = request.UserId,
                Balance = request.InitialBalance,
                Currency = request.Currency
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWallet), new { walletId = wallet.Id }, wallet);
        }

        [HttpGet("{walletId}")]
        public async Task<ActionResult<WalletResponse>> GetWallet(long walletId, [FromQuery] string currency = null)
        {
            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(currency) || currency == wallet.Currency)
            {
                return new WalletResponse
                {
                    Id = wallet.Id,
                    Balance = wallet.Balance,
                    Currency = wallet.Currency
                };
            }

            // Convert to requested currency
            var rates = await _ecbService.GetLatestRatesAsync();
            var fromRate = rates.FirstOrDefault(r => r.BaseCurrency == wallet.Currency);
            var toRate = rates.FirstOrDefault(r => r.BaseCurrency == currency);

            if (fromRate == null || toRate == null)
            {
                return BadRequest("Unsupported currency conversion");
            }

            var convertedAmount = wallet.Balance * (toRate.Rate / fromRate.Rate);

            return new WalletResponse
            {
                Id = wallet.Id,
                Balance = Math.Round(convertedAmount, 2),
                Currency = currency
            };
        }

        [HttpPost("{walletId}/adjustbalance")]
        public async Task<ActionResult<Wallet>> AdjustBalance(
            long walletId,
            [FromQuery] decimal amount,
            [FromQuery] string currency,
            [FromQuery] string strategy)
        {
            if (amount <= 0)
            {
                return BadRequest("Amount must be positive");
            }

            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet == null)
            {
                return NotFound();
            }

            decimal amountInWalletCurrency = amount;
            
            // Convert amount to wallet currency if needed
            if (!string.IsNullOrEmpty(currency) && currency != wallet.Currency)
            {
                var rates = await _ecbService.GetLatestRatesAsync();
                var fromRate = rates.FirstOrDefault(r => r.BaseCurrency == currency);
                var toRate = rates.FirstOrDefault(r => r.BaseCurrency == wallet.Currency);

                if (fromRate == null || toRate == null)
                {
                    return BadRequest("Unsupported currency conversion");
                }

                amountInWalletCurrency = amount * (toRate.Rate / fromRate.Rate);
            }

            switch (strategy.ToLower())
            {
                case "addfundsstrategy":
                    wallet.Balance += amountInWalletCurrency;
                    break;
                    
                case "subtractfundsstrategy":
                    if (wallet.Balance < amountInWalletCurrency)
                    {
                        return BadRequest("Insufficient funds");
                    }
                    wallet.Balance -= amountInWalletCurrency;
                    break;
                    
                case "forcesubtractfundsstrategy":
                    wallet.Balance -= amountInWalletCurrency;
                    break;
                    
                default:
                    return BadRequest("Invalid strategy");
            }

            await _context.SaveChangesAsync();
            return wallet;
        }
    }

    public class CreateWalletRequest
    {
        public string UserId { get; set; }
        public decimal InitialBalance { get; set; }
        public string Currency { get; set; }
    }

    public class WalletResponse
    {
        public long Id { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
    }
}
        //Creates a new wallet
       /* [HttpPost]
        public async Task<IActionResult> CreateWallet([FromBody] WalletDto walletDto)
        {
            var wallet = await _walletService.CreateWalletAsync(walletDto);
            return CreatedAtAction(nameof(GetWallet), new { id = wallet.Id }, wallet);
        }

        //Get Wallet by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWallet(int id)
        {
            var wallet = await _walletService.GetWalletByIdAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }

            return Ok(wallet);
        }

        //Adds balance to a specific currency in the wallet
        [HttpPost("{walletId}/balance")]
        public async Task<IActionResult> AddBalance(int walletId, [FromBody] WalletBalanceDto balanceDto)
        {
            try{
                var updatedBalance = await _walletService.AddBalanceToWalletAsync(walletId, balanceDto);
                return Ok(updatedBalance);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{walletId}/deposit")]
        public async Task<IActionResult> Deposit(int walletId, [FromBody] WalletBalanceDto depositDto)
        {
            try
            {
                var balance = await _walletService.DepositAsync(walletId, depositDto);
                return Ok(balance);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{walletId}/withdraw")]
        public async Task<IActionResult> Withdraw(int walletId, [FromBody] WalletBalanceDto withdrawDto)
        {
            try
            {
                var balance = await _walletService.WithdrawAsync(walletId, withdrawDto);
                return Ok(balance);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        //updates the balance to a specific wallet
        /*[HttpPut("{walletId}/balance")]
        public async Task<IActionResult> UpdateBalance(int walletId, [FromBody] WalletBalanceDto balanceDto)
        {
            try
            {
                var updatedBalance = await _walletService.UpdateWalletBalanceAsync(walletId, balanceDto);
                return Ok(updatedBalance);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }*/

        /*
        [HttpGet("{walletId}/balance/{currency}")]
        public async Task<IActionResult> GetBalance(int walletId, string currency)
        {
            var balance = await _walletService.GetBalanceByCurrencyAsync(walletId, currency);
            return balance != null ? Ok(balance) : NotFound("Saldo para esta moeda não encontrado.");
        }
    }
}*/