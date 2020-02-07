using InternetBanking.Daos;
using InternetBanking.DataCollections;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Settings;
using InternetBanking.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InternetBanking.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private IEmployeeCollection _EmployeeCollection;
        private IUserCollection _UserCollection;
        private ISetting _Setting;
        private IContext _Context;
        private MongoDBClient _MongoDBClient;
        private ITransferCollection _TransferCollection;
        private ITransactionCollection _TransactionCollection;
        private ILinkingBankCollection _LinkingBankCollection;

        public EmployeeService(ISetting setting, IEmployeeCollection employeeCollection, IUserCollection userCollection, IContext context, MongoDBClient mongoDBClient
            , ITransactionCollection transactionCollection, ITransferCollection transferCollection, ILinkingBankCollection linkingBankCollection)
        {
            _EmployeeCollection = employeeCollection;
            _Setting = setting;
            _UserCollection = userCollection;
            _Context = context;
            _MongoDBClient = mongoDBClient;
            _TransferCollection = transferCollection;
            _TransactionCollection = transactionCollection;
            _LinkingBankCollection = linkingBankCollection;
        }

        public Employee Add(Employee employee)
        {
            Employee res = null;
            _EmployeeCollection.Create(employee);
            if (employee.Id != Guid.Empty)
            {
                res = employee;
            }
            return res;
        }

        public User AddUser(Account account)
        {
            User res = null;
            using (var sessionTask = _MongoDBClient.StartSessionAsync())
            {
                var session = sessionTask.Result;
                session.StartTransaction();
                try
                {
                    var lstUser = _UserCollection.Get(new UserFilter() { Username = account.Username });
                    if (!lstUser.Any())
                    {
                        res = new User();
                        res.Id = Guid.Empty;
                        res.Name = account.Name;
                        res.Gender = account.Gender;
                        res.Email = account.Email;
                        res.Phone = account.Phone;
                        res.Password = account.Password;
                        res.Address = account.Address;
                        res.Role = 1;
                        
                        // Tao so tai khoan
                        while (true)
                        {
                            res.AccountNumber = _Context.MakeOTP(10);
                            if (!_UserCollection.Get(new UserFilter() { AccountNumber = res.AccountNumber }).Any())
                                break;
                        }

                        _UserCollection.Create(res);

                        if (res.Id.Equals(Guid.Empty))
                        {
                            res = null;
                        }
                    }
                    else
                    {
                        _Setting.Message.SetMessage("Trùng tên đăng nhập");
                    }
                    session.CommitTransactionAsync().Wait();
                }
                catch
                {
                    session.AbortTransactionAsync().Wait();
                }
            }
            return res;
        }

        public IEnumerable<CrossChecking> CrossCheckingIn(DateTime? From, DateTime? To, Guid? bankId)
        {
            var res = new List<CrossChecking>();

            var transfers = _TransferCollection.GetMany(new TransferFilter() { IsConfirmed = true});
            transfers = transfers.Where(x => x.DestinationLinkingBankId == Guid.Empty);
            foreach (var transfer in transfers)
            {
                var bank = _LinkingBankCollection.Get(new LinkingBankFilter() { Code = _Setting.BankCode }).FirstOrDefault();
                // Get ngân hàng liên kết
                var linkBank = _LinkingBankCollection.GetById(transfer.SourceLinkingBankId);
                if (linkBank != null)
                {
                    // Get thông tin giao dịch
                    var transactions = _TransactionCollection.GetMany(new TransactionFilter() { ReferenceId = transfer.Id });
                    if (transactions.Any())
                    {
                        var transaction = transactions.FirstOrDefault();

                        // Get thông tin tài khoản nguồn
                        // TODO
                        var sourceAccount = new ExternalAccount();
                        if (sourceAccount != null)
                        {
                            // Get thông tin tài khoản đích
                            var destAccount = _UserCollection.GetByAccountNumber(transfer.DestinationAccountNumber);
                            if (destAccount != null)
                            {
                                var history = new CrossChecking();
                                history.SourceAccountName = sourceAccount.Name;
                                history.SourceAccountNumber = sourceAccount.AccountNumber;
                                history.SourceBankName = linkBank.Name;
                                history.DestinationAccountName = destAccount.Name;
                                history.DestinationAccountNumber = destAccount.AccountNumber;
                                history.DestinationBankName = bank.Name;
                                history.Description = transfer.Description;
                                history.Money = transfer.Money;
                                if (!transfer.IsSenderPay)
                                    history.Money -= transfer.Fee;
                                history.ConfirmTime = transaction.ConfirmTime.Value;

                                res.Add(history);
                            }
                        }
                    }
                }
            }

            return res;
        }

        public IEnumerable<CrossChecking> CrossCheckingOut(DateTime? From, DateTime? To, Guid? bankId)
        {
            var res = new List<CrossChecking>();

            var transfers = _TransferCollection.GetMany(new TransferFilter() { IsConfirmed = true });
            transfers = transfers.Where(x => x.SourceLinkingBankId == Guid.Empty);
            foreach (var transfer in transfers)
            {
                var bank = _LinkingBankCollection.Get(new LinkingBankFilter() { Code = _Setting.BankCode }).FirstOrDefault();
                // Get ngân hàng liên kết
                var linkBank = _LinkingBankCollection.GetById(transfer.DestinationLinkingBankId);
                if (linkBank != null)
                {
                    // Get thông tin giao dịch
                    var transactions = _TransactionCollection.GetMany(new TransactionFilter() { ReferenceId = transfer.Id });
                    if (transactions.Any())
                    {
                        var transaction = transactions.FirstOrDefault();

                        // Get thông tin tài khoản nguồn
                        var sourceAccount = _UserCollection.GetByAccountNumber(transfer.SourceAccountNumber);
                        if (sourceAccount != null)
                        {
                            // Get thông tin tài khoản đích
                            // TODO
                            var destAccount = new ExternalAccount();
                            if (destAccount != null)
                            {
                                var history = new CrossChecking();
                                history.SourceAccountName = sourceAccount.Name;
                                history.SourceAccountNumber = sourceAccount.AccountNumber;
                                history.SourceBankName = linkBank.Name;
                                history.DestinationAccountName = destAccount.Name;
                                history.DestinationAccountNumber = destAccount.AccountNumber;
                                history.DestinationBankName = bank.Name;
                                history.Description = transfer.Description;
                                history.Money = transfer.Money;
                                if (transfer.IsSenderPay)
                                    history.Money += transfer.Fee;
                                history.ConfirmTime = transaction.ConfirmTime.Value;

                                res.Add(history);
                            }
                        }
                    }
                }
            }

            return res;
        }

        public bool Delete(Guid id)
        {
            return _EmployeeCollection.Delete(id) > 0;
        }

        public IEnumerable<Employee> GetEmployees(EmployeeFilter employeeFilter)
        {
            return _EmployeeCollection.Get(employeeFilter);
        }

        public bool PayIn(PayInfo payInfo)
        {
            var res = false;

            var details = _UserCollection.Get(new UserFilter() { AccountNumber = payInfo.AccountNumber, Username = payInfo.UserName });

            if (details.Any())
            {
                var detail = details.FirstOrDefault();
                detail.CheckingAccount.AccountBalance += payInfo.Money;

                res = _UserCollection.UpdateCheckingAccount(new UserFilter() { AccountNumber = payInfo.AccountNumber, Id = detail.Id }, detail.CheckingAccount) > 0;
            }
            else
            {
                _Setting.Message.SetMessage("Không tìm thấy thông tin tài khoản!");
            }

            return res;
        }

        public bool Update(Employee employee)
        {
            return _EmployeeCollection.Replace(employee) >= 0;
        }
    }
}
