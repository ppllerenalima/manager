namespace Manager.Domain.Requests.Sire.Compras
{
    public class DescargarArchivoReporteRequest
    {
        public string Token { get; set; }
        public string NomArchivoReporte { get; set; }
        public string CodTipoArchivoReporte { get; set; }
        public string CodLibro { get; set; }
        public string PerTributario { get; set; }
        public string CodProceso { get; set; }
        public string NumTicket { get; set; }
    }
}
