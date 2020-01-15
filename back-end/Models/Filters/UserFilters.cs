using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternetBanking.Models.Filters
{
    public class UserFilter
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
    }
}
