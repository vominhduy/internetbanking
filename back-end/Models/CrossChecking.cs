using InternetBanking.Models.Constants;
using System;

namespace InternetBanking.Models
{
    public class CrossChecking
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public Guid? BankId { get; set; }
    }
}
