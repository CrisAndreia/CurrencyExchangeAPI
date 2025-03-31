using System;
using System.ComponentModel.DataAnnotations;


namespace CurrencyExchangeAPI.Models
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public decimal Balance { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}
