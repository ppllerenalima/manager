namespace Manager.Domain.Responses
{
    public class ConsultarEstadoTicketResponse
    {
        public bool EsExito { get; set; }
        public int StatusCode { get; set; }
        public Result Result { get; set; }
        public List<ErrorDetail> Errores { get; set; }
    }

    public class Error422Response
    {
        public string Cod { get; set; }
        public string Msg { get; set; }
        public List<ErrorDetail> Errors { get; set; }
    }

    public class ErrorDetail
    {
        public string Cod { get; set; }
        public string Msg { get; set; }
    }


    public class Result
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

    public class ArchivoReporte
    {
        public string CodTipoAchivoReporte { get; set; }
        public string NomArchivoReporte { get; set; }
        public string NomArchivoContenido { get; set; }
    }

    public class DetalleTicket
    {
        public string NumTicket { get; set; }
        public string FecCargaImportacion { get; set; }
        public string HoraCargaImportacion { get; set; }
        public string CodEstadoEnvio { get; set; }
        public string DesEstadoEnvio { get; set; }
        public string NomArchivoReporte { get; set; }
        public int CntFilasvalidada { get; set; }
        public int CntCPError { get; set; }
        public int CntCPInformados { get; set; }
    }

    public class SubProceso
    {
        public string CodTipoSubProceso { get; set; }
        public string DesTipoSubProceso { get; set; }
        public string CodEstado { get; set; }
        public int? NumIntentos { get; set; }
    }

}
