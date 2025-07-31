using System.Text.Json.Serialization;

namespace Manager.Domain.Responses
{
    public class AceptarPropuestaResponse
    {
        [JsonPropertyName("numTicket")]
        public string NumTicket { get; set; }
    }

    public class AceptarPropuestaResultado
    {
        public bool FueAceptada { get; set; }
        public string NumTicket { get; set; }
        public string Mensaje { get; set; }
    }

    public class SunatErrorResponse
    {
        [JsonPropertyName("cod")]
        public int Cod { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("errors")]
        public List<SunatErrorDetail> Errors { get; set; }
    }

    public class SunatErrorDetail
    {
        [JsonPropertyName("cod")]
        public int Cod { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }
    }
}
