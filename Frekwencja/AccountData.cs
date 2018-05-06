namespace Frekwencja
{
    public class AccountData
    {
        public string Username { get; }
        public string Password { get; }
        public string AccessToken { get; set; }
        public AccountViewData ViewData { get; set; }

        public AccountData(string username, string password, string accessToken)
        {
            Username = username;
            Password = password;
            AccessToken = accessToken;
        }
    }
}
