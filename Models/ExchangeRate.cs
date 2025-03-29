using System;
using System.ComponentModel.DataAnnotations;

public class ExchangeRate
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string BaseCurrency { get; set; }

    [Required]
    public string TargetCurrency { get; set; }

    [Required]
    public decimal Rate { get; set; }

    public DateTime DateReceived { get; set; } = DateTime.UtcNow;
}