using System.Collections.Generic;

namespace CurrencyExchangeAPI.Dto
{
    public class WalletBalanceDto
    {
        public string Currency { get; set; }
        public decimal Balance { get; set; }
    }
}