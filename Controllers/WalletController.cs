using Microsoft.AspNetCore.Mvc;
using CurrencyExchangeAPI.Services;
using CurrencyExchangeAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyExchangeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWallet([FromBody] WalletDto walletDto)
        {
            var wallet = await _walletService.CreateWalletAsync(walletDto);
            return CreatedAtAction(nameof(GetWallet), new { id = wallet.Id }, wallet);
        }

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
    }
}