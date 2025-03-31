using System;
public class WalletDto
{
    public int Id { get; set; }
    public string Currency { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
}