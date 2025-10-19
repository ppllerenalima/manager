namespace Manager.Domain.Entities
{
    public class Grupo : EntityBase
    {
        public string Descripcion { get; set; } = default!;
        public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    }
}
