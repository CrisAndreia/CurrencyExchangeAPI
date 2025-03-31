using Microsoft.AspNetCore.Mvc;
using CurrencyExchangeAPI.Services;
using CurrencyExchangeAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CurrencyExchangeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IExchangeRateService _exchangeRateService;

        public ExchangeRateController(IExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        [HttpGet("fetch")]
        public async Task<ActionResult<List<ExchangeRate>>> FetchRates()
        {
            //var rates = await _exchangeRateService.FetchAndStoreExchangeRatesAsync();
            var rates = await _exchangeRateService.FetchAndStoreExchangeRatesAsync();
            return Ok(rates);
        }
    }
}
