using System;
using System.Collections.Generic;
using CurrencyExchangeAPI.Dto;

namespace CurrencyExchangeAPI.Dto
{
    public class WalletDto
    {
        public long Id { get; set; }
        public string UserId{ get; set; }

        public decimal Balance { get; set; }

        public string Currency { get; set; }
        //public List<WalletBalanceDto> Balances { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}