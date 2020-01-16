namespace InternetBanking.Settings.Implementations
{
    public class Message : IMessage
    {
        private string message;

        public string GetMessage()
        {
            return message;
        }

        public void SetMessage(string value)
        {
            message = value;
        }
    }
}
