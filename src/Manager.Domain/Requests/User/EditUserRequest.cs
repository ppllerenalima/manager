namespace Manager.Domain.Requests.User
{
    public class EditUserRequest
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public Guid PersonaId { get; set; }

        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string Nombre { get; set; }
        public bool IsInactive { get; set; } = false;
    }
}
