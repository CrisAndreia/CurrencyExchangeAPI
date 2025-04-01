using Microsoft.EntityFrameworkCore;
using CurrencyExchangeAPI.Models;

namespace CurrencyExchangeAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
    }
}