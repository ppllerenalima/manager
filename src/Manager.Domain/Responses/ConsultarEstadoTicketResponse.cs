namespace Manager.Domain.Responses
{
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
