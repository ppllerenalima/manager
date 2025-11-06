namespace Manager.Domain.Requests.Cpe
{
    public class DescargarZipRequest
    {
        public string RucEmisor { get; set; }
        public string TipoComprobante { get; set; }
        public string Serie { get; set; }
        public int Numero { get; set; }

        // "01" = PDF ZIP, "02" = XML ZIP, vacío = solo consulta
        public string? Tipo { get; set; }

        // true = venta (-1), false = compra (-2)
        public bool EsVenta { get; set; } = false;
    }
}
