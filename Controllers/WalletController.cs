using Microsoft.AspNetCore.Mvc;
using CurrencyExchangeAPI.Services;
using CurrencyExchangeAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using CurrencyExchangeAPI.Dto;
using System;

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
        //Creates a new wallet
        [HttpPost]
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
        }*/
    }
}