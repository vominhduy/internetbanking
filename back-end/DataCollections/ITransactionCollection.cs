using InternetBanking.Models;
using InternetBanking.Models.Filters;
using System;
using System.Collections.Generic;

namespace InternetBanking.DataCollections
{
    public interface ITransactionCollection
    {
        void Create(Transaction transaction);
        Transaction GetById(Guid id);
        IEnumerable<Transaction> GetMany(TransactionFilter transactionFilter);
        long Replace(Transaction transaction);
        long Delete(Guid id);
    }
}
