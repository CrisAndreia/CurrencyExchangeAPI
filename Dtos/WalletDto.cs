using System;
using System.Collections.Generic;
using CurrencyExchangeAPI.Dto;

namespace CurrencyExchangeAPI.Dto
{
    public class WalletDto
    {
        public int Id { get; set; }
        public string UserId{ get; set; }
        public List<WalletBalanceDto> Balances { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}