namespace Manager.Domain.Requests.User
{
    public class SignUpRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }       // Nombre
        public string LastName { get; set; }        // Apellido paterno
        public string MiddleName { get; set; }      // Apellido materno
    }
}