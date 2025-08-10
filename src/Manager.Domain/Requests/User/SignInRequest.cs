namespace Manager.Domain.Requests.User
{
    public class SignInRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}