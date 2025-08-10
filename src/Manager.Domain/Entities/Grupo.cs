namespace Manager.Domain.Entities
{
    public class Grupo : EntityBase
    {
        public string Descripcion { get; set; }
        public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    }
}
