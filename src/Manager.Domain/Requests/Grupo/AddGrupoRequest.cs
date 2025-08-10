namespace Manager.Domain.Requests.Grupo
{
    public class AddGrupoRequest
    {
        public string Descripcion { get; set; }
        public bool IsInactive { get; set; }
    }
}