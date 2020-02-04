using System;
using System.Collections.Generic;

namespace InternetBanking.Models
{
    public class User : Account
    {
        public string AcccountNumber { get; set; }
        public BankAccount CheckingAccount { get; set; } = new BankAccount();
        public List<BankAccount> SavingsAccounts { get; set; } = new List<BankAccount>();
        public List<Payee> Payees { get; set; } = new List<Payee>();
        public List<Guid> SelfDeptReminderIds { get; set; } = new List<Guid>();
        public List<Guid> OtherDeptReminderIds { get; set; } = new List<Guid>();
        public string Email { get; set; }
    }
}
