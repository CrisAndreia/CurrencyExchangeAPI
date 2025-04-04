using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CurrencyExchangeAPI.Data;
using Microsoft.OpenApi.Models;
using CurrencyExchangeAPI.Services;
using CurrencyExchangeAPI.Repositories;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Adding DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories and services
builder.Services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IECBService, ECBService>();
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("RateUpdateJob");
    q.AddJob<RateUpdateJob>(opts => opts.WithIdentity(jobKey));
    
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("RateUpdateJob-trigger")
        .WithSimpleSchedule(x => x
            .WithIntervalInMinutes(1)
            .RepeatForever()));
});

builder.Services.AddQuartzHostedService();

// HttpClient
//builder.Services.AddHttpClient<IExchangeRateService, ExchangeRateService>();
builder.Services.AddHttpClient<IECBService, ECBService>();

// Adding necessary services
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Currency Exchange API",
        Version = "v1",
        Description = "API to manage wallets and exchanging rates"
    });
});

var app = builder.Build();

// Pipeline request configuration
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency Exchange API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseRouting();

// Start application
app.Run();
