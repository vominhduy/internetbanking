using InternetBanking.Daos;
using InternetBanking.DataCollections;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Settings;
using InternetBanking.Utils;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private IEncrypt _Encrypt;

        public EmployeeService(ISetting setting, IUserCollection userCollection, IContext context, MongoDBClient mongoDBClient
            , ITransactionCollection transactionCollection, ITransferCollection transferCollection, ILinkingBankCollection linkingBankCollection, IEncrypt Encrypt)
        {
            _Setting = setting;
            _UserCollection = userCollection;
            _Context = context;
            _MongoDBClient = mongoDBClient;
            _TransferCollection = transferCollection;
            _TransactionCollection = transactionCollection;
            _LinkingBankCollection = linkingBankCollection;
            _Encrypt = Encrypt;
        }

        public Employee Add(Employee employee)
        {
            string randomPass = _Context.MakeOTP(8);
            employee.Password = randomPass;
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
            user.Username = string.Concat(employee.Name.Split(' ').Last(), employee.Code);
            user.Role = 2;
            

            _UserCollection.Create(user);
            if (user.Id != Guid.Empty)
            {
                employee.Id = user.Id;
                employee.Password = randomPass;
                employee.Role = 2;
                employee.Username = user.Username;
                res = employee;
            }
            return res;
        }

        public User AddUser(Account account)
        {
            User res = null;

            var duplicates = _UserCollection.Get(new UserFilter() { Email = account.Email });
            if (duplicates != null && duplicates.Any())
                return res;

            using (var sessionTask = _MongoDBClient.StartSessionAsync())
            {
                var session = sessionTask.Result;
                session.StartTransaction();
                try
                {
                    //var passDecrypt = Encrypting.AesDecrypt(account.Password, Encoding.UTF8.GetBytes(_Setting.AesKey), Encoding.UTF8.GetBytes(_Setting.AesIv), Encoding.UTF8);
                    //var lstUser = _UserCollection.Get(new UserFilter() { Username = account.Username });
                    //if (!lstUser.Any())
                    //{
                    // phát sinh password ngẫu nhiên
                    string randomPass = _Context.MakeOTP(10);
                    // test only
                    account.Password = "123456";

                    res = new User();
                    res.Id = Guid.Empty;
                    res.Name = account.Name;
                    res.Gender = account.Gender;
                    res.Email = account.Email;
                    res.Phone = account.Phone;
                    //res.Password = Encrypting.Bcrypt(passDecrypt);
                    res.Password = Encrypting.Bcrypt(randomPass);
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

                    // Tạo thông tin tài khoản thanh toán
                    res.CheckingAccount = new BankAccount()
                    {
                        AccountBalance = 0,
                        Description = "Tài khoản thanh toán",
                        Name = res.AccountNumber
                    };

                    _UserCollection.Create(res);

                    // tạo thành công trả về password
                    res.Password = randomPass;
                    if (res.Id.Equals(Guid.Empty))
                    {
                        res = null;
                    }
                    //}
                    //else
                    //{
                    //    _Setting.Message.SetMessage("Trùng tên đăng nhập");
                    //}
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

            var transfers = _TransferCollection.GetMany(new TransferFilter() { IsConfirmed = true });
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
                        IExternalBanking externalBanking = null;
                        if (transfer.SourceLinkingBankId == Guid.Parse("8df09f0a-fd6d-42b9-804c-575183dadaf3"))
                        {
                            externalBanking = new ExternalBanking_BKTBank(_Encrypt, _Setting);
                            externalBanking.SetPartnerCode();
                        }
                        else if (transfer.SourceLinkingBankId == Guid.Parse("a707ac8f-829f-5c41-8e35-30c58ee67a62"))
                        {
                            externalBanking = new ExternalBanking_VuBank(_Encrypt, _Setting);
                            externalBanking.SetPartnerCode();
                        }

                        var source = externalBanking.GetInfoUser(transfer.SourceAccountNumber);
                        if (source != null)
                        {
                            sourceAccount.Name = source.full_name;
                            sourceAccount.AccountNumber = source.account_number;
                        }
                        else
                        {
                            sourceAccount = null;
                        }
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
                            IExternalBanking externalBanking = null;
                            if (transfer.DestinationLinkingBankId == Guid.Parse("8df09f0a-fd6d-42b9-804c-575183dadaf3"))
                            {
                                externalBanking = new ExternalBanking_BKTBank(_Encrypt, _Setting);
                                externalBanking.SetPartnerCode();
                            }
                            else if (transfer.DestinationLinkingBankId == Guid.Parse("a707ac8f-829f-5c41-8e35-30c58ee67a62"))
                            {
                                externalBanking = new ExternalBanking_VuBank(_Encrypt, _Setting);
                                externalBanking.SetPartnerCode();
                            }

                            var source = externalBanking.GetInfoUser(transfer.DestinationAccountNumber);
                            if (source != null)
                            {
                                destAccount.AccountNumber = source.account_number;
                                destAccount.Name = source.full_name;
                            }
                            else
                            {
                                destAccount = null;
                            }
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
                employee.Role = 2;
                res.Add(employee);
            }

            return res;
        }

        public bool PayIn(Guid userId, PayInfo payInfo)
        {
            var res = false;

            var details = _UserCollection.Get(new UserFilter() { AccountNumber = payInfo.AccountNumber, Username = payInfo.Username });
            var detailsEmployee = _UserCollection.GetById(userId);

            if (details.Any() && detailsEmployee != null)
            {
                var detail = details.FirstOrDefault();
                detail.CheckingAccount.AccountBalance += payInfo.Money;

                // luu giao dich
                var transfer = new Transfer();
                transfer.Description = "Nạp tiền";
                transfer.DestinationAccountNumber = detail.AccountNumber;
                transfer.DestinationLinkingBankId = Guid.Empty;
                transfer.Fee = 0;
                transfer.IsConfirmed = true;
                transfer.IsSenderPay = true;
                transfer.Money = payInfo.Money;
                transfer.SourceAccountNumber = detailsEmployee.AccountNumber;
                transfer.SourceLinkingBankId = Guid.Empty;
                transfer.IsPayIn = true;

                using (var sessionTask = _MongoDBClient.StartSessionAsync())
                {
                    var session = sessionTask.Result;
                    session.StartTransaction();
                    try
                    {
                        _TransferCollection.Create(transfer);

                        if (transfer.Id == Guid.Empty)
                            throw new Exception();

                        var resUp = _UserCollection.UpdateCheckingAccount(new UserFilter() { AccountNumber = payInfo.AccountNumber, Id = detail.Id }, detail.CheckingAccount) > 0;

                        if (!resUp)
                            throw new Exception();

                        var transaction = new Transaction();
                        transaction.Id = Guid.Empty;
                        transaction.ReferenceId = transfer.Id;
                        transaction.Otp = "";
                        transaction.CreateTime = DateTime.Now;
                        transaction.ExpireTime = transaction.CreateTime.AddMinutes(_Setting.TransferExpiration);
                        transaction.Type = 0;
                        transaction.ConfirmTime = transaction.CreateTime;

                        _TransactionCollection.Create(transaction);

                        if (transaction.Id == Guid.Empty)
                            throw new Exception();
                        res = true;
                        session.CommitTransactionAsync();
                    }
                    catch (Exception)
                    {
                        session.AbortTransactionAsync();
                    }
                }
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
                catch (Exception ex)
                {
                    session.AbortTransactionAsync();
                    throw ex;
                }
            }

            return res;
        }
    }
}
