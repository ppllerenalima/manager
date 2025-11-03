namespace Manager.Domain.Responses.ComprobanteResponses
{
    public class Comprobante_ImportarGlosaResponse
    {
        public int TotalProcesados { get; set; }
        public int Exitosos { get; set; }
        public int Fallidos { get; set; }
        public ICollection<Comprobante_GlosaResponse> Detalle { get; set; } = new List<Comprobante_GlosaResponse>();
    }
}
