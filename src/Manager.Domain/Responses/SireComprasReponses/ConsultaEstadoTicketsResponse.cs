namespace Manager.Domain.Responses.SireComprasReponses
{
    public class ConsultaEstadoTicketsResponse
    {
        public Paginacion Paginacion { get; set; }
        public List<Registro> Registros { get; set; }
    }

    public class Paginacion
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int TotalRegistros { get; set; }
    }

    public class Registro
    {
        public string ShowReporteDescarga { get; set; }
        public string PerTributario { get; set; }
        public string NumTicket { get; set; }
        public string FecCargaImportacion { get; set; }
        public string FecInicioProceso { get; set; }
        public string CodProceso { get; set; }
        public string DesProceso { get; set; }
        public string CodEstadoProceso { get; set; }
        public string DesEstadoProceso { get; set; }
        public string NomArchivoImportacion { get; set; }
        public DetalleTicket DetalleTicket { get; set; }
        public List<ArchivoReporte> ArchivoReporte { get; set; }
        public List<SubProceso> SubProcesos { get; set; }
    }

}
