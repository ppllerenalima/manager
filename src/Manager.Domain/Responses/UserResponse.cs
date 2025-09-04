namespace Manager.Domain.Responses
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string Nombre { get; set; }
        public string NombreCompleto { get; set; }
        public bool IsInactive { get; set; }
    }
}