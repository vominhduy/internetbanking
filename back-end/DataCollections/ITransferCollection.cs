using InternetBanking.Models;
using InternetBanking.Models.Filters;
using System;
using System.Collections.Generic;

namespace InternetBanking.DataCollections
{
    public interface ITransferCollection
    {
        void Create(Transfer transfer);
        Transfer GetById(Guid id);
        IEnumerable<Transfer> GetMany(TransferFilter transferFilter);
        long Replace(Transfer transfer);
        long Delete(Guid id);
    }
}
