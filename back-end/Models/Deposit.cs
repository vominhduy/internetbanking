using InternetBanking.Models.Constants;
using System;

namespace InternetBanking.Models
{
    public class Deposit
    {
        public Guid Id { get; set; }
        public BankAccountType Type { get; set; }
        public decimal Money { get; set; }
    }
}
