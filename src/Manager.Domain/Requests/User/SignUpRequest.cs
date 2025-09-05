namespace Manager.Domain.Requests.User
{
    public class SignUpRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string Nombre { get; set; }
        public bool IsInactive { get; set; } = false;

        public string Role { get; set; }

        public SignUpRequest()
        {
            Password = UserName;
        }
    }
}