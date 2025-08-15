namespace Manager.Domain.Requests.Persona
{
    public class EditPersonaRequest
    {
        public Guid Id { get; set; }
        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string Nombre { get; set; }
        public bool IsInactive { get; set; } = false;
    }
}