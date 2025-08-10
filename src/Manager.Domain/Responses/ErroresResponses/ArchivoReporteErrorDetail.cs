namespace Manager.Domain.Responses.ErroresResponses
{
    public class ArchivoReporteErrorDetail
    {
        public int cod { get; set; }
        public string msg { get; set; }
    }

    public class ArchivoReporteErrorMessage
    {
        public int cod { get; set; }
        public string msg { get; set; }
        public List<ArchivoReporteErrorDetail> errors { get; set; } = new();
    }
}
