using System;

namespace InternetBanking.Models.Filters
{
    public class TransactionFilter
    {
        public Guid Id { get; set; }
        public Guid ReferenceId { get; set; }
        public string Otp { get; set; }
        public byte? Type { get; set; }
    }
}
