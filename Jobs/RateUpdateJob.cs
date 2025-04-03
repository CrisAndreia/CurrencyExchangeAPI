using System;
using System.Threading.Tasks;
//using ECBGateway;
//using Microsoft.EntityFrameworkCore;
using Quartz;
using CurrencyExchangeAPI.Services;
//using CurrencyExchangeAPI.Data;
using System.Linq;
//using System.Globalization;
using Microsoft.Extensions.Logging;

public class RateUpdateJob : IJob
{
    private readonly IExchangeRateService _exchangeRateService;
    private readonly ILogger<RateUpdateJob> _logger;

    public RateUpdateJob(
        IExchangeRateService exchangeRateService,
        ILogger<RateUpdateJob> logger)
    {
        _exchangeRateService = exchangeRateService;
        _logger = logger;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        try
            {
                _logger.LogInformation("Starting exchange rate update job");
                
                // Delegate all work to the service
                var updatedRates = await _exchangeRateService.FetchAndStoreExchangeRatesAsync();
                
                _logger.LogInformation(
                    "Successfully updated {Count} exchange rates. Newest rate date: {Date}",
                    updatedRates.Count,
                    updatedRates.FirstOrDefault()?.DateReceived.ToString("yyyy-MM-dd"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to execute rate update job");
                
                // Store error in job context if needed
                context.Put("error", ex.Message);
                
                // Prevent job from refiring immediately
                throw new JobExecutionException(ex, false);
            }
    }
}