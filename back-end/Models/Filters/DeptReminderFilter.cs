using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternetBanking.Models.Filters
{
    public class DeptReminderFilter
    {
        public Guid Id { get; set; }
        public string AccountNumberRequestor { get; set; }
        public string AccountNumberRecipient { get; set; }
    }
}
