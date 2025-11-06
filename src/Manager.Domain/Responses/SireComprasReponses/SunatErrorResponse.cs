namespace Manager.Domain.Responses.SireComprasReponses
{
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
