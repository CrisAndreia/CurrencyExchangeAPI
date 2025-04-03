using System;
using Microsoft.AspNetCore.Mvc;
using CurrencyExchangeAPI.Services;
using CurrencyExchangeAPI.Models;
using System.Threading.Tasks;
using CurrencyExchangeAPI.Data;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly ApplicationDbContext _context;
        private readonly IECBService _ecbService;
        private readonly ILogger<WalletController> _logger;

        public WalletController(IWalletService walletService, ApplicationDbContext context, IECBService ecbService, ILogger<WalletController> logger)
        {
            _walletService = walletService;
            _context = context;
            _ecbService = ecbService;
            _logger = logger;
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
            try
            {
                // Get the wallet
                var wallet = await _context.Wallets.FindAsync(walletId);
                if (wallet == null)
                {
                    return NotFound($"Wallet {walletId} not found");
                }

                // If no currency specified or same currency requested, return original balance
                if (string.IsNullOrEmpty(currency) || 
                    currency.ToUpper() == wallet.Currency.ToUpper())
                {
                    return Ok(new WalletResponse
                    {
                        Id = wallet.Id,
                        Balance = wallet.Balance,
                        Currency = wallet.Currency
                    });
                }
                
                if (currency.ToUpper() == "EUR" && currency.ToUpper() != wallet.Currency.ToUpper())
                {
                    var rate = await _context.ExchangeRates
                    .Where(r => r.TargetCurrency.ToUpper() == wallet.Currency.ToUpper())
                    .Select(r => r.Rate)
                    .FirstOrDefaultAsync();

                    if (rate == 0) // Assuming 0 is not a valid exchange rate
                    {
                        return BadRequest($"Conversion rate for {currency} not found");
                    }

                    return Ok(new WalletResponse
                    {
                        Id = wallet.Id,
                        Balance = Math.Round(wallet.Balance * rate, 2),
                        Currency = currency
                    });
                }

                else
                {
                    // Handle conversion cases
                    var rate = await _context.ExchangeRates
                        .Where(r => r.TargetCurrency.ToUpper() == currency.ToUpper())
                        .Select(r => r.Rate)
                        .FirstOrDefaultAsync();

                    if (rate == 0) // Assuming 0 is not a valid exchange rate
                    {
                        return BadRequest($"Conversion rate for {currency} not found");
                    }

                    return Ok(new WalletResponse
                    {
                        Id = wallet.Id,
                        Balance = Math.Round(wallet.Balance * rate, 2),
                        Currency = currency
                    });
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing wallet {walletId}");
                return StatusCode(500, "Internal server error");
            }
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