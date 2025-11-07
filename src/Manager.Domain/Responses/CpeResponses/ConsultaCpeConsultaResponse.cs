
namespace Manager.Domain.Responses.CpeResponses
{
    public class ConsultaCpeConsultaResponse
    {
        [JsonProperty("cntTotalReg")]
        public int CntTotalReg { get; set; }

        [JsonProperty("numPag")]
        public int NumPag { get; set; }

        [JsonProperty("numRegPag")]
        public int NumRegPag { get; set; }

        [JsonProperty("numeroPaginas")]
        public List<NumeroPagina> NumeroPaginas { get; set; } = new();

        [JsonProperty("comprobantes")]
        public List<ComprobanteCpe> Comprobantes { get; set; } = new();
    }

    public class NumeroPagina
    {
        [JsonProperty("numPagFront")]
        public int NumPagFront { get; set; }
    }

    public class ComprobanteCpe
    {
        [JsonProperty("datosEmisor")]
        public DatosEmisorCpe DatosEmisor { get; set; } = new();

        [JsonProperty("datosReceptor")]
        public DatosReceptorCpe DatosReceptor { get; set; } = new();

        [JsonProperty("codCpe")]
        public string CodCpe { get; set; }

        [JsonProperty("numSerie")]
        public string NumSerie { get; set; }

        [JsonProperty("numCpe")]
        public int NumCpe { get; set; }

        [JsonProperty("numCpeHasta")]
        public int NumCpeHasta { get; set; }

        [JsonProperty("codMoneda")]
        public string CodMoneda { get; set; }

        [JsonProperty("placaVehicular")]
        public string PlacaVehicular { get; set; }

        [JsonProperty("fecEmision")]
        public string FecEmision { get; set; }

        [JsonProperty("fecRegistro")]
        public string FecRegistro { get; set; }

        [JsonProperty("codTipTransaccion")]
        public string CodTipTransaccion { get; set; }

        [JsonProperty("indEstadoCpe")]
        public string IndEstadoCpe { get; set; }

        [JsonProperty("informacionItems")]
        public List<InformacionItemCpe> InformacionItems { get; set; } = new();

        [JsonProperty("procedenciaIndivual")]
        public ProcedenciaMasivaCpe ProcedenciaIndivual { get; set; } = new();
    }

    public class DatosEmisorCpe
    {
        [JsonProperty("numRuc")]
        public string NumRuc { get; set; }

        [JsonProperty("desRazonSocialEmis")]
        public string DesRazonSocialEmis { get; set; }

        [JsonProperty("desNomComercialEmis")]
        public string DesNomComercialEmis { get; set; }

        [JsonProperty("desDirEmis")]
        public string DesDirEmis { get; set; }

        [JsonProperty("ubigeoEmis")]
        public string UbigeoEmis { get; set; }
    }

    public class DatosReceptorCpe
    {
        [JsonProperty("codDocIdeRecep")]
        public string CodDocIdeRecep { get; set; }

        [JsonProperty("numDocIdeRecep")]
        public string NumDocIdeRecep { get; set; }

        [JsonProperty("desRazonSocialRecep")]
        public string DesRazonSocialRecep { get; set; }

        [JsonProperty("dirDetCliente")]
        public string DirDetCliente { get; set; }

        [JsonProperty("dirDetRecepFactura")]
        public string DirDetRecepFactura { get; set; }
    }

    public class InformacionItemCpe
    {
        [JsonProperty("cntItems")]
        public decimal CntItems { get; set; }

        [JsonProperty("codUnidadMedida")]
        public string CodUnidadMedida { get; set; }

        [JsonProperty("desUnidadMedida")]
        public string DesUnidadMedida { get; set; }

        [JsonProperty("desCodigo")]
        public string DesCodigo { get; set; }

        [JsonProperty("desItem")]
        public string DesItem { get; set; }

        [JsonProperty("mtoValUnitario")]
        public decimal MtoValUnitario { get; set; }

        [JsonProperty("mtoImpTotal")]
        public decimal MtoImpTotal { get; set; }
    }

    public class ProcedenciaMasivaCpe
    {
        [JsonProperty("mtoTotalValVentaGrabado")]
        public decimal MtoTotalValVentaGrabado { get; set; }

        [JsonProperty("mtoTotalValVentaExonerado")]
        public decimal MtoTotalValVentaExonerado { get; set; }

        [JsonProperty("mtoImporteTotal")]
        public decimal MtoImporteTotal { get; set; }
    }
}
