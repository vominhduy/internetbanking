using InternetBanking.Models.Constants;
using System;

namespace InternetBanking.Models
{
    public class CrossChecking
    {
        public string SourceAccountNumber { get; set; }
        public string SourceAccountName { get; set; }
        public string SourceBankName { get; set; }
        public string DestinationAccountNumber { get; set; }
        public string DestinationAccountName { get; set; }
        public string DestinationBankName { get; set; }
        public DateTime ConfirmTime { get; set; }
        public decimal Money { get; set; }
        public string Description { get; set; }
    }

    public class RCrossChecking
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public Guid? BankId { get; set; }
    }
}
