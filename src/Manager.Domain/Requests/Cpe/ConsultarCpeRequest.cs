namespace Manager.Domain.Requests.Cpe
{
    public class ConsultarCpeRequest
    {
        public string RucConsulta { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string RucEmisor { get; set; }
        public string TipoComprobante { get; set; }
        public string Serie { get; set; }
        public int Numero { get; set; }
    }
}
