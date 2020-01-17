using InternetBanking.Models;
using InternetBanking.Models.Filters;
using System;
using System.Collections.Generic;

namespace InternetBanking.DataCollections
{
    public interface ILinkingBankCollection
    {
        void Create(LinkingBank linkingBankInfo);
        IEnumerable<LinkingBank> Get(LinkingBankFilter linkingBankFilter);
        LinkingBank GetById(Guid id);
        void CreateTransaction(LinkingBank linkingBankInfo);
        long Replace(LinkingBank linkingBankInfo);
        long Update(LinkingBank linkingBankInfo);
        long Delete(Guid id);
    }
}
