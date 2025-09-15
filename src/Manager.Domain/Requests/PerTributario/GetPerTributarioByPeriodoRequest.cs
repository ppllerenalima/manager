namespace Manager.Domain.Requests.PerTributario
{
    public class GetPerTributarioByPeriodoRequest
    {
        public int Anio { get; set; }
        public int Mes { get; set; }
        public Guid ClienteId { get; set; }
    }
}