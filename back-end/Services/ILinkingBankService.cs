using InternetBanking.Models;
using InternetBanking.Models.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InternetBanking.Services
{
    public interface ILinkingBankService
    {
        public LinkingBank CreateLinkingBank(LinkingBank bank);
        public IEnumerable<LinkingBank> GetLinkingBank();
        public LinkingBank GetLinkingBankById(LinkingBankFilter filter);
    }
}