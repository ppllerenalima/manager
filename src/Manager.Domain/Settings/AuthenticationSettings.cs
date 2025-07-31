namespace Manager.Domain.Settings
{
    public class AuthenticationSettings
    {
        //public string Secret { get; set; }
        public int ExpirationDays { get; set; }

        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public bool ValidateIssuer { get; set; } = false;
        public bool ValidateAudience { get; set; } = false;
    }
}