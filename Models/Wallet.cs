using System;
using System.ComponentModel.DataAnnotations;
using CurrencyExchangeAPI.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CurrencyExchangeAPI.Models
{
    public class Wallet
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public List<WalletBalance> Balances { get; set; } = new List<WalletBalance>();
    }
}