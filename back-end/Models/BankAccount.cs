using System;

namespace InternetBanking.Models
{
    public class BankAccount
    {
        public BankAccount()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal AccountBalance { get; set; } = 0;
        public string Name { get; set; }
        public bool? IsClosed { get; set; }
    }
}
