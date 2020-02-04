using System;

namespace InternetBanking.Models.Filters
{
    public class TransferFilter
    {
        public Guid Id { get; set; }
        public Guid InternalUserId { get; set; }
        public string AccountNumber { get; set; }
        public Guid LinkingBankId { get; set; }
        public string Otp { get; set; }
        public bool IsInternal { get; set; }
    }
}
