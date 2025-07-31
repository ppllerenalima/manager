namespace Manager.Domain.Responses.CpeResponses
{
    public class StatusResponse
    {
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public byte[] Content { get; set; }
        public bool Success { get; set; } = true;
        public string ErrorMessage { get; set; }
    }
}
