using System;

namespace InternetBanking.Models.Filters
{
    public class TransferFilter
    {
        public Guid Id { get; set; }
        public string DestinationAccountNumber { get; set; }
        public Guid DestinationLinkingBankId { get; set; }
        public string SourceAccountNumber { get; set; }
        public Guid SourceLinkingBankId { get; set; }
        public string Otp { get; set; }
        public bool IsInternal { get; set; }
    }
}
