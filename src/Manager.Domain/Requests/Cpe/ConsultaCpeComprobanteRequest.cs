namespace Manager.Domain.Requests.Cpe
{
    public class ConsultaCpeComprobanteRequest
    {
        public string RucEmisor { get; set; }
        public string TipoComprobante { get; set; }
        public string Serie { get; set; }
        public int Numero { get; set; }
        public string Tipo { get; set; }
    }
}
