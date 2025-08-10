namespace Manager.Domain.Requests.Grupo
{
    public class EditGrupoRequest
    {
        public Guid Id { get; set; }
        public bool IsInactive { get; set; } = false;

        public string Descripcion { get; set; }
    }
}