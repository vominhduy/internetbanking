using InternetBanking.Daos;
using InternetBanking.DataCollections;
using InternetBanking.Models;
using InternetBanking.Models.Filters;
using InternetBanking.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InternetBanking.Services.Implementations
{
    public class DeptReminderService : IDeptReminderService
    {
        private IUserCollection _UserCollection;
        private IDeptReminderCollection _DeptReminderCollection;
        private ILinkingBankCollection _LinkingBankCollection;
        private ISetting _Setting;
        private MongoDBClient _MongoDBClient;

        public DeptReminderService(ISetting setting, IUserCollection userCollection, IDeptReminderCollection deptReminderCollection, ILinkingBankCollection linkingBankCollection, MongoDBClient mongoDBClient)
        {
            _UserCollection = userCollection;
            _Setting = setting;
            _DeptReminderCollection = deptReminderCollection;
            _LinkingBankCollection = linkingBankCollection;
            _MongoDBClient = mongoDBClient;
        }

        public DeptReminder AddDeptReminder(Guid userId, DeptReminder deptReminder)
        {
            DeptReminder res = null;
            var userDetail = _UserCollection.GetById(userId);

            if (userDetail != null)
            {
                // requestor
                deptReminder.RequestorAccountNumber = userDetail.AcccountNumber;

                var recipientUser = _UserCollection.Get(new UserFilter() { AccountNumber = deptReminder.RecipientAccountNumber });

                if (recipientUser != null)
                {
                    _DeptReminderCollection.Create(deptReminder);
                    if (deptReminder.Id != Guid.Empty)
                    {
                        res = deptReminder;
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

        public bool CheckoutDeptReminder(Guid userId, Guid deptReminderId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateDeptReminder(Guid userId, DeptReminder deptReminder)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DeptReminder> GetDeptReminders(Guid userId)
        {
            var res = new List<DeptReminder>();

            var userDetail = _UserCollection.GetById(userId);

            if (userDetail != null)
            {
                var lstRecipient = _DeptReminderCollection.GetMany(new DeptReminderFilter() { RecipientAccountNumber = userDetail.AcccountNumber }).ToList();
                var lstRequestor = _DeptReminderCollection.GetMany(new DeptReminderFilter() { RequestorAccountNumber = userDetail.AcccountNumber }).ToList();

                lstRecipient.AddRange(lstRequestor);

                res = lstRecipient;
            }
            return res;
        }
    }
}
