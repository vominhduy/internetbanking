using InternetBanking.Daos;
using InternetBanking.DataCollections;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Settings;
using InternetBanking.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InternetBanking.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private IUserCollection _UserCollection;
        private ISetting _Setting;
        private IContext _Context;
        private MongoDBClient _MongoDBClient;
        private ITransferCollection _TransferCollection;
        private ITransactionCollection _TransactionCollection;
        private ILinkingBankCollection _LinkingBankCollection;

        public EmployeeService(ISetting setting, IUserCollection userCollection, IContext context, MongoDBClient mongoDBClient
            , ITransactionCollection transactionCollection, ITransferCollection transferCollection, ILinkingBankCollection linkingBankCollection)
        {
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
            employee.Password = _Context.MakeOTP(8);
            Employee res = null;
            while (true)
            {
                employee.Code = _Context.MakeOTP(10, isAllDigits: true);
                if (!_UserCollection.Get(new UserFilter() { AccountNumber = employee.Code }).Any())
                    break;
            }
            var user = new User();

            user.AccountNumber = employee.Code;
            user.Address = employee.Address;
            user.Email = employee.Email;
            user.Gender = employee.Gender;
            user.Name = employee.Name;
            user.Phone = employee.Phone;
            user.Password = Encrypting.Bcrypt(employee.Password);
            user.Role = 2;

            _UserCollection.Create(user);
            if (user.Id != Guid.Empty)
            {
                employee.Id = user.Id;
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
                    //var passDecrypt = Encrypting.AesDecrypt(account.Password, Encoding.UTF8.GetBytes(_Setting.AesKey), Encoding.UTF8.GetBytes(_Setting.AesIv), Encoding.UTF8);
                    var lstUser = _UserCollection.Get(new UserFilter() { Username = account.Username });
                    if (!lstUser.Any())
                    {
                        res = new User();
                        res.Id = Guid.Empty;
                        res.Name = account.Name;
                        res.Gender = account.Gender;
                        res.Email = account.Email;
                        res.Phone = account.Phone;
                        //res.Password = Encrypting.Bcrypt(passDecrypt);
                        res.Password = Encrypting.Bcrypt(account.Password);
                        res.Address = account.Address;
                        res.Role = 1;

                        // Tao so tai khoan
                        while (true)
                        {
                            res.AccountNumber = _Context.MakeOTP(10, isAllDigits: true);
                            if (!_UserCollection.Get(new UserFilter() { AccountNumber = res.AccountNumber }).Any())
                                break;
                        }
                        res.Username = string.Concat(account.Name.Split(' ').Last(), res.AccountNumber);

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

        public IEnumerable<CrossChecking> CrossCheckingIn(DateTime? from, DateTime? to, Guid? bankId)
        {
            var res = new List<CrossChecking>();

            var transfers = _TransferCollection.GetMany(new TransferFilter() { IsConfirmed = true});
            transfers = transfers.Where(x => x.DestinationLinkingBankId == Guid.Empty);
            if (bankId.HasValue)
            {
                transfers = transfers.Where(x => x.SourceLinkingBankId == bankId.Value);
            }
            foreach (var transfer in transfers)
            {
                var bank = _LinkingBankCollection.Get(new LinkingBankFilter() { Code = _Setting.BankCode }).FirstOrDefault();
                // Get ngân hàng liên kết
                var linkBank = _LinkingBankCollection.GetById(transfer.SourceLinkingBankId);
                if (linkBank != null)
                {
                    // Get thông tin giao dịch
                    var transactions = _TransactionCollection.GetMany(new TransactionFilter() { ReferenceId = transfer.Id, Type = 0 });
                    if (from.HasValue)
                    {
                        transactions = transactions.Where(x => x.ConfirmTime.Value.Date >= from.Value.Date);
                    }
                    if (to.HasValue)
                    {
                        transactions = transactions.Where(x => x.ConfirmTime.Value.Date <= to.Value.Date);
                    }

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

        public IEnumerable<CrossChecking> CrossCheckingOut(DateTime? from, DateTime? to, Guid? bankId)
        {
            var res = new List<CrossChecking>();

            var transfers = _TransferCollection.GetMany(new TransferFilter() { IsConfirmed = true });
            transfers = transfers.Where(x => x.SourceLinkingBankId == Guid.Empty);
            if (bankId.HasValue)
            {
                transfers = transfers.Where(x => x.DestinationLinkingBankId == bankId.Value);
            }
            foreach (var transfer in transfers)
            {
                var bank = _LinkingBankCollection.Get(new LinkingBankFilter() { Code = _Setting.BankCode }).FirstOrDefault();
                // Get ngân hàng liên kết
                var linkBank = _LinkingBankCollection.GetById(transfer.DestinationLinkingBankId);
                if (linkBank != null)
                {
                    // Get thông tin giao dịch
                    var transactions = _TransactionCollection.GetMany(new TransactionFilter() { ReferenceId = transfer.Id, Type = 0 });
                    if (from.HasValue)
                    {
                        transactions = transactions.Where(x => x.ConfirmTime.Value.Date >= from.Value.Date);
                    }
                    if (to.HasValue)
                    {
                        transactions = transactions.Where(x => x.ConfirmTime.Value.Date <= to.Value.Date);
                    }

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
            return _UserCollection.Delete(id) > 0;
        }

        public IEnumerable<Employee> GetEmployees(UserFilter employeeFilter)
        {
            var res = new List<Employee>();

            var users = _UserCollection.Get(new UserFilter()
            {
                AccountNumber = employeeFilter.AccountNumber,
                Id = employeeFilter.Id,
                Email = employeeFilter.Email,
                Name = employeeFilter.Name,
                Username = employeeFilter.Username
            });

            foreach (var user in users)
            {
                var employee = new Employee();
                employee.Username = user.Username;
                employee.Address = user.Address;
                employee.Id = user.Id;
                employee.Gender = user.Gender;
                employee.Phone = user.Phone;
                employee.Code = user.AccountNumber;
                employee.Email = user.Email;
                employee.Name = user.Name;
                res.Add(employee);
            }

            return res;
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
            var res = false;
            // Get chi tiết nhân viên
            using (var sessionTask = _MongoDBClient.StartSessionAsync())
            {
                var session = sessionTask.Result;
                session.StartTransaction();
                try
                {
                    var detail = _UserCollection.GetById(employee.Id);
                    if (detail != null)
                    {
                        detail.Name = employee.Name;
                        detail.Phone = employee.Phone;
                        detail.Address = employee.Address;
                        //detail.Email = employee.Email;
                        detail.Gender = employee.Gender;

                        res = _UserCollection.Replace(detail) > 0;
                    }
                    else
                        _Setting.Message.SetMessage("Không tìm thấy thông tin nhân viên!");

                    if (res)
                        session.CommitTransactionAsync();
                    else
                        session.AbortTransactionAsync();
                }
                catch(Exception ex)
                {
                    session.AbortTransactionAsync();
                    throw ex;
                }
            }
            
            return res;
        }
    }
}
