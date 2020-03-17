using System;
using System.Collections.Generic;
using System.Linq;
using InternetBanking.Daos;
using InternetBanking.DataCollections;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Settings;
using InternetBanking.Utils;

namespace InternetBanking.Services.Implementations
{
    public class LinkingBankService : ILinkingBankService
    {
        private ILinkingBankCollection _LinkingBankCollection;
        private ISetting _Setting;
        private IContext _Context;
        private ITransactionCollection _TransactionCollection;
        private MongoDBClient _MongoDBClient;

        public LinkingBankService(ISetting setting, IContext context, ILinkingBankCollection linkingBankCollection,
            ITransactionCollection transactionCollection, MongoDBClient mongoDBClient)
        {
            _Setting = setting;
            _Context = context;
            _LinkingBankCollection = linkingBankCollection;
            _TransactionCollection = transactionCollection;
            _MongoDBClient = mongoDBClient;
        }

        public LinkingBank CreateLinkingBank(LinkingBank bank)
        {
            bank.Id = Guid.Empty;
            _LinkingBankCollection.Create(bank);
            if (bank.Id.Equals(Guid.Empty))
            {
                return null;
            }
            else
            {
                return bank;
            }
        }

        public IEnumerable<LinkingBank> GetLinkingBank()
        {
            return _LinkingBankCollection.Get(new LinkingBankFilter());
        }

        public LinkingBank GetLinkingBankById(LinkingBankFilter filter)
        {
            return _LinkingBankCollection.Get(new LinkingBankFilter()).ToList().FirstOrDefault();
        }
    }
}
