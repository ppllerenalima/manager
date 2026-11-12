namespace Manager.Domain.Entities
{
    public class SunatPadronHistorial : EntityBase
    {
        public string NombrePadron { get; set; } = string.Empty;
        public string TextoFechaSunat { get; set; } = string.Empty;
        public int RegistrosProcesados { get; set; }
        public DateTime FechaEjecucion { get; set; } = DateTime.Now;
    }
}
