using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternetBanking.Models.ViewModels
{
    public class TransferMoneyRequest
    {
        public string from_account_number { get; set; }
        public string to_account_number { get; set; }
        public decimal amount { get; set; }
        public string message { get; set; }
        public string signature { get; set; }
    }
}
