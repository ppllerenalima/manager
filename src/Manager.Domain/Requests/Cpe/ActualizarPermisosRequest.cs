namespace Manager.Domain.Requests.Cpe
{
    public class ActualizarPermisosRequest
    {
        public string Id { get; set; } = string.Empty;
        public string ExpFlujoAutoriz { get; set; } = "100000";
        public string NomApp { get; set; } = string.Empty;
        public string DesUrlApp { get; set; } = string.Empty;
    }
}
