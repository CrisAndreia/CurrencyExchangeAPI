using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=CurrencyExchange.db");
            }
            //base.OnConfiguring(optionsBuilder);
        }
    }
}