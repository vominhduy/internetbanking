using InternetBanking.Models.Constants;
using System;

namespace InternetBanking.Models
{
    public class Payee
    {
        public Guid Id { get; set; }
        public Guid LinkingBankId { get; set; }
        public string Name { get; set; }
        public string AcccountNumber { get; set; }
        public string MnemonicName { get; set; }
    }
}
