using System.Collections.Generic;

namespace InternetBanking.Models
{
    public class User : Account
    {
        public string AcccountNumber { get; set; }
        public BankAccount CheckingAccount { get; set; } = new BankAccount();
        public List<BankAccount> SavingsAccounts { get; set; } = new List<BankAccount>();
    }
}
