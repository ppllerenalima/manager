namespace Manager.Domain.Requests.ConfiguracionGlobal
{
    public class EditConfiguracionGlobalRequest
    {
        public Guid Id { get; set; }
        public bool IsInactive { get; set; } = false;

        public string Descripcion { get; set; }
    }
}