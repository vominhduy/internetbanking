using InternetBanking.Daos;
using InternetBanking.DataCollections;
using InternetBanking.Models;
using InternetBanking.Models.Constants;
using InternetBanking.Models.Filters;
using InternetBanking.Settings;
using InternetBanking.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InternetBanking.Services.Implementations
{
    public class UserService : IUserService
    {
        private IUserCollection _UserCollection;
        private ILinkingBankCollection _LinkingBankCollection;
        private ISetting _Setting;
        private IContext _Context;
        private ITransferCollection _TransferCollection;
        private ITransactionCollection _TransactionCollection;
        private IDeptReminderCollection _DeptReminderCollection;
        private MongoDBClient _MongoDBClient;
        public UserService(ISetting setting, IUserCollection userCollection, ILinkingBankCollection linkingBankCollection, IContext context
            , ITransferCollection transferCollection, MongoDBClient mongoDBClient, IDeptReminderCollection deptReminderCollection
            , ITransactionCollection transactionCollection)
        {
            _UserCollection = userCollection;
            _Setting = setting;
            _LinkingBankCollection = linkingBankCollection;
            _Context = context;
            _TransferCollection = transferCollection;
            _MongoDBClient = mongoDBClient;
            _DeptReminderCollection = deptReminderCollection;
            _TransactionCollection = transactionCollection;
        }

        public Payee AddPayee(Guid userId, Payee payee)
        {
            Payee res = null;
            var detail = _UserCollection.GetById(userId);

            if (detail != null)
            {
                var linkingBank = _LinkingBankCollection.GetById(payee.LinkingBankId);

                if (linkingBank != null)
                {
                    var countModified = _UserCollection.AddPayee(userId, payee);
                    if (countModified > 0)
                    {
                        res = payee;
                    }
                }
            }

            return res;
        }

        public BankAccount AddSavingsAccount(UserFilter userFilter, BankAccount bankAccount)
        {
            BankAccount res = null;
            var details = _UserCollection.Get(userFilter);

            if (details.Any())
            {
                var detail = details.FirstOrDefault();

                if (detail.SavingsAccounts.FirstOrDefault(x => x.Name == bankAccount.Name) != null)
                {
                    _Setting.Message.SetMessage("Duplicate name of bank account!");
                }
                else
                {
                    bankAccount.Id = Guid.NewGuid();
                    userFilter.Name = null;
                    userFilter.Id = detail.Id;
                    var countModified = _UserCollection.AddSavingsAccount(userFilter, bankAccount);
                    if (countModified > 0)
                        res = bankAccount;
                }
            }

            return bankAccount;
        }

        public User AddUser(User user)
        {
            _UserCollection.Create(user);
            return user;
        }



        public bool ConfirmTransfer(Guid userId, Guid transactionId, string otp)
        {
            var res = false;
            using (var sessionTask = _MongoDBClient.StartSessionAsync())
            {
                var session = sessionTask.Result;
                session.StartTransaction();
                try
                {
                    // Get detail user
                    var userDetail = _UserCollection.GetById(userId);

                    if (userDetail != null)
                    {
                        // Get Get chi tiết giao dịch
                        var transaction = _TransactionCollection.GetById(transactionId);
                        if (transaction != null)
                        {
                            // Check OTP
                            if (transaction.Otp == otp)
                            {
                                // Check hết hạn
                                if (transaction.ExpireTime >= DateTime.Now)
                                {
                                    // Get chi tiết chuyển tiền
                                    var transfer = _TransferCollection.GetById(transaction.ReferenceId);
                                    // Check user hiện tại có tạo yêu cầu chuyển tiền
                                    if (transfer != null
                                        && transfer.SourceLinkingBankId == Guid.Empty
                                        && transfer.SourceAccountNumber == userDetail.AccountNumber)
                                    {
                                        // Tru so du
                                        userDetail.CheckingAccount.AccountBalance -= transfer.Money;
                                        transfer.Fee = _Context.TransactionCost(transfer.Money);

                                        // Tru phi
                                        if (transfer.IsSenderPay)
                                        {
                                            userDetail.CheckingAccount.AccountBalance -= transfer.Fee;
                                        }

                                        if (userDetail.CheckingAccount.AccountBalance >= 0)
                                        {
                                            // Luu thong tin nguoi gui
                                            var payOut = _UserCollection.UpdateCheckingAccount(new UserFilter() { Id = userId }, userDetail.CheckingAccount);

                                            if (payOut > 0)
                                            {
                                                // Cong tien nguoi nhan
                                                var success = false;
                                                if (transfer.DestinationLinkingBankId == Guid.Empty)
                                                {
                                                    // noi bo
                                                    var detailRecepients = _UserCollection.Get(new UserFilter() { AccountNumber = transfer.DestinationAccountNumber });
                                                    if (detailRecepients.Any())
                                                    {
                                                        var detailRecepient = detailRecepients.FirstOrDefault();
                                                        detailRecepient.CheckingAccount.AccountBalance += transfer.Money;
                                                        // Tru phi
                                                        if (!transfer.IsSenderPay)
                                                        {
                                                            detailRecepient.CheckingAccount.AccountBalance -= transfer.Fee;
                                                        }

                                                        payOut = _UserCollection.UpdateCheckingAccount(new UserFilter() { Id = detailRecepient.Id }, detailRecepient.CheckingAccount);

                                                        if (payOut > 0)
                                                        {
                                                            success = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        _Setting.Message.SetMessage("Không tìm thấy thông tin người nhận!");
                                                    }
                                                }
                                                else
                                                {
                                                    // lien ngan hang
                                                    // TODO
                                                }

                                                if (success)
                                                {
                                                    // Update trạng thái chuyển tiền
                                                    transfer.IsConfirmed = true;
                                                    var updateTransfer = _TransferCollection.Replace(transfer);
                                                    if (updateTransfer > 0)
                                                    {
                                                        // Update trạng thái giao dịch
                                                        transaction.ConfirmTime = DateTime.Now;
                                                        var updateTransaction = _TransactionCollection.Replace(transaction);
                                                        if (updateTransaction > 0)
                                                        {
                                                            // Send mail
                                                            // TODO
                                                            res = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _Setting.Message.SetMessage("Số dư không đủ!");
                                        }
                                    }
                                    else
                                    {
                                        _Setting.Message.SetMessage("Không tìm thấy thông tin chuyển tiền!");
                                    }
                                }
                                else
                                {
                                    _Setting.Message.SetMessage("Phiên giao dịch hết hạn!");
                                }
                            }
                            else
                            {
                                _Setting.Message.SetMessage("Sai mã OTP!");
                            }
                        }
                        else
                        {
                            _Setting.Message.SetMessage("Không tìm thấy thông tin giao dịch!");
                        }
                    }
                    else
                    {
                        _Setting.Message.SetMessage("Không tìm thấy thông tin người gửi yêu cầu!");
                    }

                    if (res)
                    {
                        session.CommitTransactionAsync();
                    }
                    else
                    {
                        session.AbortTransactionAsync();
                    }
                }
                catch (Exception)
                {
                    session.AbortTransactionAsync().Wait();
                    throw;
                }
            }

            return res;
        }

        public bool DeletePayee(Guid userId, Guid payeeId)
        {
            var res = false;
            var filter = new UserFilter();
            filter.Id = userId;
            var details = _UserCollection.Get(filter);

            if (details.Any())
            {
                var detail = details.FirstOrDefault();
                var payee = detail.Payees.FirstOrDefault(x => x.Id == payeeId);

                if (payee != null)
                {
                    detail.Payees.Remove(payee);
                    res = _UserCollection.Replace(detail) >= 0;
                }
            }

            return res;
        }

        public bool DeleteSavingsAccount(Guid userId, Guid bankAccountId)
        {
            var res = false;
            var filter = new UserFilter();
            filter.Id = userId;
            var details = _UserCollection.Get(filter);

            if (details.Any())
            {
                var detail = details.FirstOrDefault();
                var bankAccount = detail.SavingsAccounts.FirstOrDefault(x => x.Id == bankAccountId);

                if (bankAccount != null)
                {
                    detail.SavingsAccounts.Remove(bankAccount);
                    res = _UserCollection.Replace(detail) >= 0;
                }
            }

            return res;
        }

        public bool Deposit(Guid userId, BankAccountType bankAccountType, Guid bankAccountId, decimal money)
        {
            var res = false;
            var filter = new UserFilter();
            filter.Id = userId;
            var details = _UserCollection.Get(filter);

            if (details.Any())
            {
                var detail = details.FirstOrDefault();
                if (bankAccountType == BankAccountType.CheckingAccount)
                {
                    detail.CheckingAccount.AccountBalance += money;
                    res = _UserCollection.UpdateCheckingAccount(new UserFilter() { Id = userId }, detail.CheckingAccount) >= 0;
                }
                else
                {
                    var bankDetail = detail.SavingsAccounts.FirstOrDefault(x => x.Id == bankAccountId);

                    if (bankDetail != null)
                    {
                        bankDetail.AccountBalance += money;
                        res = _UserCollection.UpdateSavingsAccount(new UserFilter() { Id = userId }, bankDetail) >= 0;
                    }
                }
            }

            return res;
        }

        public ExternalAccount GetDetailUserByPartner(string accountNumber)
        {
            ExternalAccount res = null;
            var detail = _UserCollection.GetByAccountNumber(accountNumber);
            if (detail != null)
            {
                if (detail.Role == 1)
                {
                    res = new ExternalAccount();
                    res.AccountNumber = detail.AccountNumber;
                    res.Address = detail.Address;
                    res.Email = detail.Email;
                    if (detail.Gender == 0)
                        res.Gender = "Nam";
                    else if (detail.Gender == 1)
                        res.Gender = "Nữ";
                    else
                        res.Gender = "Khác";
                    res.Name = detail.Name;
                    res.Phone = detail.Phone;
                }
            }

            return res;
        }

        public IEnumerable<User> GetUsers(UserFilter employeeFilter)
        {
            return _UserCollection.Get(employeeFilter);
        }

        public IEnumerable<TransactionHistory> HistoryDeptIn(Guid userId)
        {
            var res = new List<TransactionHistory>();

            var userDetail = _UserCollection.GetById(userId);
            if (userDetail != null)
            {
                var userDepts = _DeptReminderCollection.GetMany(new DeptReminderFilter() { RequestorAccountNumber = userDetail.AccountNumber, IsCanceled = false, IsPaid = true });

                if (userDepts.Any())
                {
                    foreach (var userDept in userDepts)
                    {
                        var hisTransaction = new TransactionHistory();

                        // Get chi tiết người trả nợ
                        var dest = _UserCollection.GetByAccountNumber(userDept.RecipientAccountNumber);
                        if (dest != null)
                        {
                            hisTransaction.AccountName = dest.Name;
                            hisTransaction.AccountNumber = dest.AccountNumber;
                            hisTransaction.Description = userDept.Description;
                            hisTransaction.Money = userDept.Money;

                            var linkingBanks = _LinkingBankCollection.Get(new LinkingBankFilter() { Code = _Setting.BankCode } );
                            if (linkingBanks != null)
                            {
                                hisTransaction.BankName = linkingBanks.FirstOrDefault().Name;
                            }
                            else
                                continue;
                        }
                        else
                            continue;

                        res.Add(hisTransaction);
                    }
                }
            }
            return res;
        }
        public IEnumerable<TransactionHistory> HistoryDeptOut(Guid userId)
        {
            var res = new List<TransactionHistory>();

            var userDetail = _UserCollection.GetById(userId);
            if (userDetail != null)
            {
                var userDepts = _DeptReminderCollection.GetMany(new DeptReminderFilter() { RecipientAccountNumber = userDetail.AccountNumber, IsCanceled = false, IsPaid = true });

                if (userDepts.Any())
                {
                    foreach (var userDept in userDepts)
                    {
                        var hisTransaction = new TransactionHistory();

                        // Get chi tiết người được trả nợ
                        var source = _UserCollection.GetByAccountNumber(userDept.RequestorAccountNumber);
                        if (source != null)
                        {
                            hisTransaction.AccountName = source.Name;
                            hisTransaction.AccountNumber = source.AccountNumber;
                            hisTransaction.Description = userDept.Description;
                            hisTransaction.Money = userDept.Money;

                            var linkingBanks = _LinkingBankCollection.Get(new LinkingBankFilter() { Code = _Setting.BankCode });
                            if (linkingBanks != null)
                            {
                                hisTransaction.BankName = linkingBanks.FirstOrDefault().Name;
                            }
                            else
                                continue;
                        }
                        else
                            continue;

                        res.Add(hisTransaction);
                    }
                }
            }
            return res;
        }

        public IEnumerable<TransactionHistory> HistoryIn(Guid userId)
        {
            var res = new List<TransactionHistory>();

            var userDetail = _UserCollection.GetById(userId);
            if (userDetail != null)
            {
                var userTransfers = _TransferCollection.GetMany(new TransferFilter() { DestinationAccountNumber = userDetail.AccountNumber });
                userTransfers = userTransfers.Where(x => x.DestinationLinkingBankId == Guid.Empty);
                if (userTransfers.Any())
                {
                    foreach (var transfer in userTransfers)
                    {
                        var hisTransaction = new TransactionHistory();
                        // Nội bộ
                        if (transfer.SourceLinkingBankId == Guid.Empty)
                        {
                            // Get chi tiết người gửi
                            var source = _UserCollection.GetByAccountNumber(transfer.SourceAccountNumber);
                            if (source != null)
                            {
                                hisTransaction.AccountName = source.Name;
                                hisTransaction.AccountNumber = source.AccountNumber;
                                hisTransaction.Description = transfer.Description;

                                if (transfer.IsSenderPay)
                                {
                                    hisTransaction.Money = transfer.Money;
                                }
                                else
                                {
                                    hisTransaction.Money = transfer.Money - transfer.Fee;
                                }

                                var linkingBank = _LinkingBankCollection.GetById(transfer.SourceLinkingBankId);
                                if (linkingBank != null)
                                {
                                    hisTransaction.BankName = linkingBank.Name;
                                }
                                else
                                    continue;
                            }
                            else
                                continue;
                        }
                        // Linking bank
                        else
                        {
                            // TODO
                        }

                        res.Add(hisTransaction);
                    }
                }
            }

            return res;
        }

        public IEnumerable<TransactionHistory> HistoryOut(Guid userId)
        {
            var res = new List<TransactionHistory>();

            var userDetail = _UserCollection.GetById(userId);
            if (userDetail != null)
            {
                var userTransfers = _TransferCollection.GetMany(new TransferFilter() { SourceAccountNumber = userDetail.AccountNumber });
                userTransfers = userTransfers.Where(x => x.SourceLinkingBankId == Guid.Empty);
                if (userTransfers.Any())
                {
                    foreach (var transfer in userTransfers)
                    {
                        var hisTransaction = new TransactionHistory();
                        // Nội bộ
                        if (transfer.DestinationLinkingBankId == Guid.Empty)
                        {
                            // Get chi tiết người nhận
                            var dest = _UserCollection.GetByAccountNumber(transfer.DestinationAccountNumber);
                            if (dest != null)
                            {
                                hisTransaction.AccountName = dest.Name;
                                hisTransaction.AccountNumber = dest.AccountNumber;
                                hisTransaction.Description = transfer.Description;

                                if (!transfer.IsSenderPay)
                                {
                                    hisTransaction.Money = transfer.Money;
                                }
                                else
                                {
                                    hisTransaction.Money = transfer.Money + transfer.Fee;
                                }

                                var linkingBank = _LinkingBankCollection.GetById(transfer.DestinationLinkingBankId);
                                if (linkingBank != null)
                                {
                                    hisTransaction.BankName = linkingBank.Name;
                                }
                                else
                                    continue;
                            }
                            else
                                continue;
                        }
                        // Linking bank
                        else
                        {
                            // TODO
                        }

                        res.Add(hisTransaction);
                    }
                }
            }
            return res;
        }

        public bool PayInByPartner(Transfer transfer)
        {
            var res = false;
            var linkBank = _LinkingBankCollection.GetById(transfer.SourceLinkingBankId);
            if (linkBank != null)
            {
                // get chi tiết người nhận
                using (var sessionTask = _MongoDBClient.StartSessionAsync())
                {
                    var session = sessionTask.Result;
                    session.StartTransaction();
                    try
                    {
                        var userDetail = _UserCollection.GetByAccountNumber(transfer.DestinationAccountNumber);
                        if (userDetail != null)
                        {
                            transfer.Id = Guid.Empty;
                            transfer.Fee = _Context.TransactionCost(transfer.Money);
                            transfer.IsConfirmed = true;
                            transfer.DestinationLinkingBankId = Guid.Empty;
                            transfer.SignedData = transfer.SignedData;
                            // Update số dư
                            userDetail.CheckingAccount.AccountBalance += transfer.Money;
                            if (!transfer.IsSenderPay)
                            {
                                userDetail.CheckingAccount.AccountBalance -= transfer.Fee;
                            }

                            // Create chuyển tiền
                            _TransferCollection.Create(transfer);

                            if (transfer.Id != Guid.Empty)
                            {
                                // Update người nhận
                                var updateUser = _UserCollection.UpdateCheckingAccount(new UserFilter() { Id = userDetail.Id }, userDetail.CheckingAccount);

                                if (updateUser > 0)
                                {
                                    // Create giao dịch
                                    var transaction = new Transaction();
                                    transaction.Id = Guid.Empty;
                                    transaction.CreateTime = DateTime.Now;
                                    transaction.ExpireTime = transaction.CreateTime.AddMinutes(_Setting.TransferExpiration);
                                    transaction.ConfirmTime = transaction.CreateTime;
                                    transaction.ReferenceId = transfer.Id;
                                    transaction.Type = 0;

                                    _TransactionCollection.Create(transaction);

                                    if (transaction.Id != Guid.Empty)
                                    {
                                        res = true;
                                    }
                                    else
                                        _Setting.Message.SetMessage("Không thể lưu thông tin giao dịch!");
                                }
                                else
                                    _Setting.Message.SetMessage("Không thể lưu thông tin người nhận!");
                            }
                            else
                                _Setting.Message.SetMessage("Không thể lưu thông tin chuyển tiền!");
                        }
                        else
                            _Setting.Message.SetMessage("Không tìm thấy thông tin người nhận!");

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
            }
            return res;
        }

        public bool PayOutByPartner(Transfer transfer)
        {
            var res = false;
            var linkBank = _LinkingBankCollection.GetById(transfer.SourceLinkingBankId);
            if (linkBank != null)
            {
                // get chi tiết người nhận
                using (var sessionTask = _MongoDBClient.StartSessionAsync())
                {
                    var session = sessionTask.Result;
                    session.StartTransaction();
                    try
                    {
                        var userDetail = _UserCollection.GetByAccountNumber(transfer.SourceAccountNumber);
                        if (userDetail != null)
                        {
                            transfer.Id = Guid.Empty;
                            transfer.Fee = _Context.TransactionCost(transfer.Money);
                            transfer.IsConfirmed = true;
                            transfer.SourceLinkingBankId = Guid.Empty;
                            // Update số dư
                            userDetail.CheckingAccount.AccountBalance -= transfer.Money;
                            if (transfer.IsSenderPay)
                            {
                                userDetail.CheckingAccount.AccountBalance -= transfer.Fee;
                            }

                            // Create chuyển tiền
                            _TransferCollection.Create(transfer);

                            if (transfer.Id != Guid.Empty)
                            {
                                // Update gửi
                                var updateUser = _UserCollection.UpdateCheckingAccount(new UserFilter() { Id = userDetail.Id }, userDetail.CheckingAccount);

                                if (updateUser > 0)
                                {
                                    // Create giao dịch
                                    var transaction = new Transaction();
                                    transaction.Id = Guid.Empty;
                                    transaction.CreateTime = DateTime.Now;
                                    transaction.ExpireTime = transaction.CreateTime.AddMinutes(_Setting.TransferExpiration);
                                    transaction.ConfirmTime = transaction.CreateTime;
                                    transaction.ReferenceId = transfer.Id;
                                    transaction.Type = 0;

                                    _TransactionCollection.Create(transaction);

                                    if (transaction.Id != Guid.Empty)
                                    {
                                        res = true;
                                    }
                                    else
                                        _Setting.Message.SetMessage("Không thể lưu thông tin giao dịch!");
                                }
                                else
                                    _Setting.Message.SetMessage("Không thể lưu thông tin người người chuyển tiền!");
                            }
                            else
                                _Setting.Message.SetMessage("Không thể lưu thông tin chuyển tiền!");
                        }
                        else
                            _Setting.Message.SetMessage("Không tìm thấy thông tin người chuyển tiền!");

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
            }
            return res;
        }

        public Transaction Transfer(Guid userId, Transfer transfer)
        {
            Transaction res = null;

            var userDetail = _UserCollection.GetById(userId);
            transfer.SourceAccountNumber = userDetail.AccountNumber;
            transfer.SourceLinkingBankId = Guid.Empty;

            if (userDetail != null)
            {
                User recepient = null;

                if (transfer.DestinationLinkingBankId == Guid.Empty)
                {
                    recepient = _UserCollection.GetByAccountNumber(transfer.DestinationAccountNumber);
                }
                else
                {
                    // Get recepient info from linking bank
                    // TODO
                }

                if (recepient != null)
                {
                    string otp = null;
                    using (var sessionTask = _MongoDBClient.StartSessionAsync())
                    {
                        var session = sessionTask.Result;
                        session.StartTransaction();
                        try
                        {
                            // Create OTP
                            while (true)
                            {
                                otp = _Context.MakeOTP(6);
                                if (!_TransactionCollection.GetMany(new TransactionFilter() { Otp = otp, Type = 0 }).Any())
                                    break;
                            }

                            _TransferCollection.Create(transfer);

                            if (!transfer.Id.Equals(Guid.Empty))
                            {
                                // Lưu thông tin giao dịch
                                var transaction = new Transaction();
                                transaction.Id = Guid.Empty;
                                transaction.ReferenceId = transfer.Id;
                                transaction.Otp = otp;
                                transaction.CreateTime = DateTime.Now;
                                transaction.ExpireTime = transaction.CreateTime.AddMinutes(_Setting.TransferExpiration);
                                transaction.Type = 0;

                                _TransactionCollection.Create(transaction);

                                if (transaction.Id != Guid.Empty)
                                {
                                    // Send mail
                                    var sb = new StringBuilder();
                                    sb.AppendFormat($"Dear {userDetail.Name},");
                                    sb.AppendFormat("<br /><br /><b>Bạn đang yêu cầu chuyển tiền từ hệ thống của chúng tôi, mã xác thực của bạn là:</b>");
                                    sb.AppendFormat($"<br /><br /><b>{transaction.Otp}</b>");
                                    sb.AppendFormat($"<br /><br /><b>Mã xác thực này sẽ hết hạn lúc {transaction.ExpireTime.ToLongTimeString()}.</b>");
                                    sb.AppendFormat($"<br /><br /><b>Nếu yêu cầu không phải của bạn, vui lòng bỏ qua mail này.</b>");

                                    if (_Context.SendMail("Xác thực yêu cầu chuyển tiền", sb.ToString(), userDetail.Email, userDetail.Name))
                                    {
                                        res = transaction;
                                    }
                                }
                            }
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
                    _Setting.Message.SetMessage("Không tìm thấy tài khoản người nhận!");
                }
            }
            else
            {
                _Setting.Message.SetMessage("Không tìm thấy tài khoản người gửi!");
            }

            return res;
        }

        public bool UpdateBankAccount(Guid userId, BankAccountType bankAccountType, BankAccount bankAccount)
        {
            var res = false;
            var filter = new UserFilter() { Id = userId };
            var details = _UserCollection.Get(filter);

            if (details.Any())
            {
                if (bankAccountType == BankAccountType.CheckingAccount)
                {
                    var detail = details.FirstOrDefault().CheckingAccount;
                    if (detail.Id == bankAccount.Id)
                    {
                        detail.Description = bankAccount.Description;
                        detail.Name = bankAccount.Name;

                        res = _UserCollection.UpdateCheckingAccount(filter, detail) >= 0;
                    }
                }
                else
                {
                    var detail = details.FirstOrDefault().SavingsAccounts.FirstOrDefault(x => x.Id == bankAccount.Id);

                    if (detail != null)
                    {
                        detail.Description = bankAccount.Description;
                        detail.Name = bankAccount.Name;

                        res = _UserCollection.UpdateSavingsAccount(filter, detail) >= 0;
                    }
                }
            }
            return res;
        }

        public bool UpdatePayee(Guid userId, Payee payee)
        {
            var res = false;
            var filter = new UserFilter() { Id = userId };
            var details = _UserCollection.Get(filter);

            if (details.Any())
            {
                var detail = details.FirstOrDefault().Payees.FirstOrDefault(x => x.Id == payee.Id);

                if (detail != null)
                {
                    detail.MnemonicName = payee.MnemonicName;

                    res = _UserCollection.UpdatePayee(userId, detail) >= 0;
                }
            }
            return res;
        }

        public bool UpdateUser(User user)
        {
            var res = false;
            var details = _UserCollection.Get(new UserFilter() { Id = user.Id });

            if (details.Any())
            {
                var detail = details.FirstOrDefault();

                detail.Name = user.Name;
                detail.Gender = user.Gender;
                detail.Address = user.Address;

                res = _UserCollection.Replace(detail) >= 0;
            }
            return res;
        }
    }
}
