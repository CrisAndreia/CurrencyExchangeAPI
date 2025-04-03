/*using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CurrencyExchangeAPI.Models
{
    public class WalletBalance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Wallet")]
        public long WalletId { get; set; }
        
        //Related to Wallet
        public Wallet Wallet { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public decimal Balance { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}*/
