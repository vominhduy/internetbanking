namespace InternetBanking.Models.ViewModels
{
    public class ExternalBankRes<T>
    {
        public string messageCode { get; set; }
        public string message { get; set; }
        public T data { get; set; }
    }

    public class ExternalInfoUserResponse
    {
        public string account_number { get; set; }
        public string email { get; set; }
        public string username { get; set; }
        public string full_name { get; set; }
    }

    public class ExternalTransferMoneyResponse
    {
        public bool is_success { get; set; }
    }
}
