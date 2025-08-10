namespace Manager.Domain.Entities
{
    public class Persona : EntityBase
    {
        public string ApePaterno { get; set; }
        public string ApeMaterno { get; set; }
        public string Nombre { get; set; }

        public User User { get; set; }
    }
}
