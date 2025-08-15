namespace Manager.Domain.Requests.Grupo
{
    public class AddPersonaRequest
    {
        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string Nombre { get; set; }
        public bool IsInactive { get; set; }
    }
}