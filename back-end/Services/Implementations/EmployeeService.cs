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
        public EmployeeService(ISetting setting, IEmployeeCollection employeeCollection, IUserCollection userCollection, IContext context, MongoDBClient mongoDBClient)
        {
            _EmployeeCollection = employeeCollection;
            _Setting = setting;
            _UserCollection = userCollection;
            _Context = context;
            _MongoDBClient = mongoDBClient;
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
                            res.AcccountNumber = _Context.MakeOTP(10);
                            if (!_UserCollection.Get(new UserFilter() { AccountNumber = res.AcccountNumber }).Any())
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

        public IEnumerable<HistoryTransaction> CrossCheckingIn(DateTime? From, DateTime? To, Guid? bankId)
        {
            var res = new List<HistoryTransaction>();



            return res;
        }

        public IEnumerable<HistoryTransaction> CrossCheckingOut(DateTime? From, DateTime? To, Guid? bankId)
        {
            var res = new List<HistoryTransaction>();



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
