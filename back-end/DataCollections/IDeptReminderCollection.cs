using InternetBanking.Models;
using InternetBanking.Models.Filters;
using System;
using System.Collections.Generic;

namespace InternetBanking.DataCollections
{
    public interface IDeptReminderCollection
    {
        void Create(DeptReminder deptReminder);
        DeptReminder GetById(Guid id);
        IEnumerable<DeptReminder> GetMany(DeptReminderFilter deptReminderFilter);
        long Replace(DeptReminder deptReminder);
        long Delete(Guid id);
    }
}
