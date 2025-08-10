namespace Manager.Domain.Responses
{
    public class UserResponse
    {
        public string FirstName { get; set; }       // Nombre
        public string LastName { get; set; }        // Apellido paterno
        public string MiddleName { get; set; }      // Apellido materno
        public string Email { get; set; }
    }
}