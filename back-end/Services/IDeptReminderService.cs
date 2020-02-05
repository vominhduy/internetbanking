using InternetBanking.Models;
using InternetBanking.Models.Constants;
using InternetBanking.Models.Filters;
using System;
using System.Collections.Generic;

namespace InternetBanking.Services
{
    public interface IDeptReminderService
    {
        #region Dept reminder
        public IEnumerable<DeptReminder> GetDeptReminders(Guid userId);
        public DeptReminder AddDeptReminder(Guid userId, DeptReminder deptReminder);
        public bool UpdateDeptReminder(Guid userId, DeptReminder deptReminder);
        public bool DeleteDeptReminder(Guid userId, Guid deptReminderId);
        public bool CancelDeptReminder(Guid userId, Guid id, string notes);
        public Transaction CheckoutDeptReminder(Guid userId, Guid deptReminderId);
        public bool ConfirmDeptReminder(Guid userId, Guid transactionId, string otp);
        #endregion
    }
}
