namespace Manager.Domain.Requests.ConfiguracionGlobal
{
    public class AddConfiguracionGlobalRequest
    {
        public int MaxCaracteresGlosa { get; set; }
        public bool IsInactive { get; set; }
    }
}