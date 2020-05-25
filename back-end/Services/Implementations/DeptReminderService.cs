﻿using InternetBanking.Daos;
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
    public class DeptReminderService : IDeptReminderService
    {
        private IUserCollection _UserCollection;
        private IDeptReminderCollection _DeptReminderCollection;
        private ILinkingBankCollection _LinkingBankCollection;
        private ISetting _Setting;
        private IContext _Context;
        private MongoDBClient _MongoDBClient;
        private ITransferCollection _TransferCollection;
        private ITransactionCollection _TransactionCollection;

        public DeptReminderService(ISetting setting, IUserCollection userCollection, IDeptReminderCollection deptReminderCollection
            , ILinkingBankCollection linkingBankCollection, MongoDBClient mongoDBClient, IContext context, ITransferCollection transferCollection
            , ITransactionCollection transactionCollection)
        {
            _UserCollection = userCollection;
            _Setting = setting;
            _DeptReminderCollection = deptReminderCollection;
            _LinkingBankCollection = linkingBankCollection;
            _MongoDBClient = mongoDBClient;
            _Context = context;
            _TransferCollection = transferCollection;
            _TransactionCollection = transactionCollection;
        }

        public DeptReminder AddDeptReminder(Guid userId, DeptReminder deptReminder)
        {
            DeptReminder res = null;
            var userDetail = _UserCollection.GetById(userId);

            if (userDetail != null)
            {
                // requestor
                deptReminder.RequestorAccountNumber = userDetail.AccountNumber;
                deptReminder.RequestorId = userDetail.Id;


                var recipientUsers = _UserCollection.Get(new UserFilter() { AccountNumber = deptReminder.RecipientAccountNumber });

                if (recipientUsers.Any())
                {
                    var recipientUser = recipientUsers.FirstOrDefault();
                    deptReminder.RecipientId = recipientUser.Id;

                    while (true)
                    {
                        deptReminder.Code = _Context.MakeOTP(15);
                        if (!_DeptReminderCollection.GetMany(new DeptReminderFilter()
                        {
                            Code = deptReminder.Code
                        }).Any())
                            break;
                    }

                    _DeptReminderCollection.Create(deptReminder);
                    if (deptReminder.Id != Guid.Empty)
                    {
                        // Send mail
                        var sb = new StringBuilder();
                        sb.AppendFormat($"Dear {recipientUser.Name},");
                        sb.AppendFormat($"<br /><br /><b>Bạn đang được nhắc nợ từ số tài khoản {userDetail.AccountNumber} - {userDetail.Name}.</b>");
                        sb.AppendFormat($"<br /><br /><b>Vui lòng đăng nhập vào hệ thống để xem chi tiết.</b>");
                        sb.AppendFormat($"<br /><br /><b>Nếu yêu cầu không phải của bạn, vui lòng bỏ qua mail này.</b>");

                        if (_Context.SendMail("Thông báo nhắc nợ", sb.ToString(), recipientUser.Email, recipientUser.Name))
                        {
                            res = deptReminder;
                        }
                    }
                }

                //var lstLinkingBank = _LinkingBankCollection.Get(new LinkingBankFilter() { Code = _Setting.BankCode });
                //if (lstLinkingBank.Any())
                //{
                //    deptReminder.Requestor.LinkingBankId = lstLinkingBank.FirstOrDefault().Id;

                //    // check recipient
                //    var recipientLinkingBank = _LinkingBankCollection.GetById(deptReminder.Recipient.LinkingBankId);
                //    if (recipientLinkingBank != null)
                //    {
                //        if (recipientLinkingBank.Code != _Setting.BankCode) // other bank
                //        {
                //            /*TODO*/
                //            //// get user from linking bank
                //        }
                //        else // same bank
                //        {

                //        }
                //    }
                //}
            }
            return res;
        }

        public bool DeleteDeptReminder(Guid userId, Guid deptReminderId)
        {
            var res = false;
            using (var sessionTask = _MongoDBClient.StartSessionAsync())
            {
                var session = sessionTask.Result;
                session.StartTransaction();

                try
                {
                    var userDetail = _UserCollection.GetById(userId);

                    if (userDetail != null)
                    {
                        var removeSeft = userDetail.SelfDeptReminderIds.Remove(deptReminderId);
                        var removeOther = userDetail.OtherDeptReminderIds.Remove(deptReminderId);

                        if (!removeSeft && !removeOther)
                        {

                        }
                        else
                        {
                            var delete = _DeptReminderCollection.Delete(deptReminderId) >= 0;

                            if (delete)
                            {
                                var updateUser = _UserCollection.Replace(userDetail) >= 0;

                                if (updateUser)
                                {
                                    /*TODO*/
                                    // send mail
                                    res = true;
                                    session.CommitTransactionAsync().Wait();
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    session.AbortTransactionAsync().Wait();
                    throw;
                }

                if (!res)
                {
                    session.AbortTransactionAsync().Wait();
                }
            }
            return res;
        }

        public Transaction CheckoutDeptReminder(Guid userId, Guid deptReminderId)
        {
            Transaction res = null;
            using (var sessionTask = _MongoDBClient.StartSessionAsync())
            {
                var session = sessionTask.Result;
                session.StartTransaction();
                try
                {
                    var userDetail = _UserCollection.GetById(userId);
                    if (userDetail != null)
                    {
                        // Get chi tiết nhắc nợ
                        var dept = _DeptReminderCollection.GetById(deptReminderId);
                        if (dept != null && dept.RecipientId == userId)
                        {
                            // Create OTP
                            string otp = null;
                            while (true)
                            {
                                otp = _Context.MakeOTP(6);
                                if (!_TransactionCollection.GetMany(new TransactionFilter() { Otp = otp, Type = 1 }).Any())
                                    break;
                            }

                            // expire
                            var transaction = new Transaction();
                            transaction.Id = Guid.Empty;
                            transaction.ReferenceId = dept.Id;
                            transaction.Otp = otp;
                            transaction.CreateTime = DateTime.Now;
                            transaction.ExpireTime = transaction.CreateTime.AddMinutes(_Setting.TransferExpiration);
                            transaction.Type = 1;

                            _TransactionCollection.Create(transaction);

                            if (transaction.Id != Guid.Empty)
                            {
                                // Send mail
                                var sb = new StringBuilder();
                                sb.AppendFormat($"Dear {userDetail.Name},");
                                sb.AppendFormat("<br /><br /><b>Bạn đang yêu cầu thanh toán nhắc nợ, mã xác thực của bạn là:</b>");
                                sb.AppendFormat($"<br /><br /><b>{transaction.Otp}</b>");
                                sb.AppendFormat($"<br /><br /><b>Mã xác thực này sẽ hết hạn lúc {transaction.ExpireTime.ToLongTimeString()}.</b>");
                                sb.AppendFormat($"<br /><br /><b>Nếu yêu cầu không phải của bạn, vui lòng bỏ qua mail này.</b>");

                                if (_Context.SendMail("Xác thực thanh toán nhắc nợ", sb.ToString(), userDetail.Email, userDetail.Name))
                                {
                                    res = transaction;
                                }
                            }
                            else
                            {
                                _Setting.Message.SetMessage("Không thể thanh toán nhắc nợ!");
                            }
                        }
                        else
                        {
                            _Setting.Message.SetMessage("Không tìm thấy thông tin nhắc nợ!");
                        }
                    }
                    else
                    {
                        _Setting.Message.SetMessage("Không tìm thấy thông tin khách hàng!");
                    }

                    if (res != null)
                        session.CommitTransactionAsync();
                    else
                        session.AbortTransactionAsync();
                }
                catch
                {
                    session.AbortTransactionAsync();
                }
            }

            return res;
        }

        public bool UpdateDeptReminder(Guid userId, DeptReminder deptReminder)
        {
            throw new NotImplementedException();
        }

        public object GetDeptReminders(Guid userId)
        {
            var res = new List<DeptReminder>();

            var userDetail = _UserCollection.GetById(userId);

            if (userDetail != null)
            {
                var lstRecipient = _DeptReminderCollection.GetMany(new DeptReminderFilter() { RecipientAccountNumber = userDetail.AccountNumber }).ToList();
                var lstRequestor = _DeptReminderCollection.GetMany(new DeptReminderFilter() { RequestorAccountNumber = userDetail.AccountNumber }).ToList();

                return new { SentDeptReminders = lstRequestor, ReceivedDeptReminders = lstRecipient };
            }
            return null;
        }

        public bool ConfirmDeptReminder(Guid userId, Guid transactionId, string otp)
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
                        // Get chi tiết giao dịch
                        var transaction = _TransactionCollection.GetById(transactionId);
                        if (transaction != null)
                        {
                            // Check mã OTP
                            if (transaction.Otp == otp)
                            {
                                // Kiểm tra hết hạn
                                if (transaction.ExpireTime >= DateTime.Now)
                                {
                                    // Get chi tiết nhăc nợ
                                    var dept = _DeptReminderCollection.GetById(transaction.ReferenceId);
                                    if (dept != null)
                                    {
                                        // Trừ số dư
                                        userDetail.CheckingAccount.AccountBalance -= dept.Money;
                                        var fee = _Context.TransactionCost(dept.Money);

                                        // Trừ phí
                                        userDetail.CheckingAccount.AccountBalance -= fee;

                                        if (userDetail.CheckingAccount.AccountBalance >= 0)
                                        {
                                            // Update thông tin người trả nợ
                                            var payOut = _UserCollection.UpdateCheckingAccount(new UserFilter() { Id = userId }, userDetail.CheckingAccount);

                                            if (payOut > 0)
                                            {
                                                // Cộng tiền người nhận
                                                var detailRecepient = _UserCollection.GetById(dept.RequestorId);
                                                if (detailRecepient != null)
                                                {
                                                    detailRecepient.CheckingAccount.AccountBalance += dept.Money;

                                                    payOut = _UserCollection.UpdateCheckingAccount(new UserFilter() { Id = detailRecepient.Id }, detailRecepient.CheckingAccount);

                                                    if (payOut > 0)
                                                    {
                                                        // Update trạng thái nhắc nợ
                                                        dept.IsPaid = true;
                                                        dept.PaidTime = DateTime.Now;
                                                        if (_DeptReminderCollection.Replace(dept) > 0)
                                                        {
                                                            // Update trang thai giao dich
                                                            transaction.ConfirmTime = dept.PaidTime;
                                                            if (_TransactionCollection.Replace(transaction) > 0)
                                                            {
                                                                // Send mail
                                                                var sb = new StringBuilder();
                                                                sb.AppendFormat($"Dear {detailRecepient.Name},");
                                                                sb.AppendFormat("<br /><br /><b>Nhắc nợ của bạn đã được thanh toán.:</b>");
                                                                sb.AppendFormat($"<br /><br /><b>Mã: {dept.Code}</b>");
                                                                sb.AppendFormat($"<br /><br /><b>Người thanh toán: {userDetail.AccountNumber} - {userDetail.Name }</b>");
                                                                sb.AppendFormat($"<br /><br /><b>Vui lòng đăng nhập vào hệ thống để xem chi tiết.</b>");
                                                                sb.AppendFormat($"<br /><br /><b>Nếu yêu cầu không phải của bạn, vui lòng bỏ qua mail này.</b>");

                                                                if (_Context.SendMail("Thanh toán nhắc nợ", sb.ToString(), detailRecepient.Email, detailRecepient.Name))
                                                                {
                                                                    res = true;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                _Setting.Message.SetMessage("Không thể cập nhật số liệu!");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            _Setting.Message.SetMessage("Không thể cập nhật số liệu!");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        _Setting.Message.SetMessage("Không thể cập nhật số liệu!");
                                                    }
                                                }
                                                else
                                                {
                                                    _Setting.Message.SetMessage("Không tìm thấy thông tin người nhận!");
                                                }
                                            }
                                            else
                                            {
                                                _Setting.Message.SetMessage("Không thể cập nhật số liệu!");
                                            }
                                        }
                                        else
                                        {
                                            _Setting.Message.SetMessage("Số dư không đủ!");
                                        }
                                    }
                                    else
                                    {
                                        _Setting.Message.SetMessage("Không tìm thấy thông tin nhắc nợ!");
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
                catch (Exception ex)
                {
                    session.AbortTransactionAsync();
                    throw ex;
                }
            }

            return res;
        }

        public bool CancelDeptReminder(Guid userId, Guid id, string notes)
        {
            var res = false;

            // Get chi tiết nhắc nợ
            var details = _DeptReminderCollection.GetMany(new DeptReminderFilter() { Id = id, IsCanceled = false });

            if (details.Any())
            {
                var detail = details.FirstOrDefault();
                detail.CanceledNotes = notes;
                detail.IsCanceled = true;

                // Update trạng thái
                using (var sessionTask = _MongoDBClient.StartSessionAsync())
                {
                    var session = sessionTask.Result;
                    session.StartTransaction();
                    try
                    {
                        if (_DeptReminderCollection.Replace(detail) > 0)
                        {
                            // Get chi tiết người nhắc nợ
                            var request = _UserCollection.GetById(detail.RequestorId);

                            if(request != null)
                            {
                                // Get chi tiết người được nhắc nợ
                                var recept = _UserCollection.GetById(detail.RecipientId);

                                if (recept != null)
                                {
                                    if (detail.RequestorId == userId)
                                    {
                                        // Self
                                        // Send mail
                                        var sb = new StringBuilder();
                                        sb.AppendFormat($"Dear {recept.Name},");
                                        sb.AppendFormat($"<br /><br /><b>Nhắc nợ đã bị hủy, mã: {detail.Code}.</b>");
                                        sb.AppendFormat($"<br /><br /><b>Người hủy: {request.AccountNumber} - {request.Name}.</b>");
                                        sb.AppendFormat($"<br /><br /><b>Vui lòng đăng nhập vào hệ thống để xem chi tiết.</b>");
                                        sb.AppendFormat($"<br /><br /><b>Nếu yêu cầu không phải của bạn, vui lòng bỏ qua mail này.</b>");

                                        if (_Context.SendMail("Hủy nhắc nợ", sb.ToString(), recept.Email, recept.Name))
                                        {
                                            res = true;
                                        }
                                    }
                                    else if (detail.RecipientId == userId)
                                    {
                                        // Other
                                        // Send mail
                                        var sb = new StringBuilder();
                                        sb.AppendFormat($"Dear {request.Name},");
                                        sb.AppendFormat($"<br /><br /><b>Nhắc nợ đã bị hủy, mã: {detail.Code}.</b>");
                                        sb.AppendFormat($"<br /><br /><b>Người hủy: {recept.AccountNumber} - {recept.Name}.</b>");
                                        sb.AppendFormat($"<br /><br /><b>Vui lòng đăng nhập vào hệ thống để xem chi tiết.</b>");
                                        sb.AppendFormat($"<br /><br /><b>Nếu yêu cầu không phải của bạn, vui lòng bỏ qua mail này.</b>");

                                        if (_Context.SendMail("Hủy nhắc nợ", sb.ToString(), request.Email, request.Name))
                                        {
                                            res = true;
                                        }
                                    }
                                    else
                                        _Setting.Message.SetMessage("Không tìm thấy thông tin nhắc nợ!");
                                }
                            }
                        }
                        else
                            _Setting.Message.SetMessage("Không tìm thấy thông tin nhắc nợ!");
                    }
                    catch(Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            else
                _Setting.Message.SetMessage("Không tìm thấy thông tin nhắc nợ!");

            return res;
        }
    }
}
