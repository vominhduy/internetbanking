using InternetBanking.Models;
using InternetBanking.Models.Constants;
using InternetBanking.Models.Filters;
using System;
using System.Collections.Generic;

namespace InternetBanking.Services
{
    public interface IUserService
    {
        public IEnumerable<User> GetUsers(UserFilter userFilter);
        public bool UpdateUser(User user);
        public User AddUser(User user);
        public BankAccount AddSavingsAccount(UserFilter userFilter, BankAccount bankAccount);
        public bool UpdateBankAccount(Guid userId, BankAccountType bankAccountType, BankAccount bankAccount);
        public bool DeleteSavingsAccount(Guid userId, Guid bankAccountId);
        public bool Deposit(Guid userId, BankAccountType bankAccountType, Guid bankAccountId, decimal money);
        public Payee AddPayee(Guid userId, Payee payee);
        public bool UpdatePayee(Guid userId, Payee payee);
        public bool DeletePayee(Guid userId, Guid payeeId);
        public Transaction Transfer(Guid userId, Transfer transfer);
        public bool ConfirmTransfer(Guid userId, Guid transactionId, string otp);
        public IEnumerable<TransactionHistory> HistoryIn(Guid userId);
        public IEnumerable<TransactionHistory> HistoryOut(Guid userId);
        public IEnumerable<TransactionHistory> HistoryDeptIn(Guid userId);
        public IEnumerable<TransactionHistory> HistoryDeptOut(Guid userId);
        public ExternalAccount GetDetailUserByPartner(string accountNumber);
        public Guid PayInByPartner(Transfer transfer);
        public bool PayOutByPartner(Transfer transfer);
        public bool CloseBankAccount(Guid userId, Guid id);
    }
}
