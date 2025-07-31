using System.Text.Json.Serialization;

namespace Manager.Domain.Responses
{
    public class DescargarPropuestaResponse
    {
        public bool EsExito { get; set; }
        public int StatusCode { get; set; }
        public ResultDescargarPropuesta Result { get; set; }
        public List<ErrorDetail> Errores { get; set; } = new List<ErrorDetail>();
    }

    public class ResultDescargarPropuesta
    {
        [JsonPropertyName("numTicket")]
        public string NumTicket { get; set; }
    }


}
