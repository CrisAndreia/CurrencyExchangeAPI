namespace CurrencyExchangeAPI.Models
{
    public class Wallet
    {
        public long Id { get; set; }

        public string UserId { get; set; }

        public decimal Balance { get; set; }
        
        public string Currency { get; set; }

    }
}