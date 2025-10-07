namespace Manager.Domain.Responses.ComprobanteResponses
{
    public class ComprobanteResponse
    {
        public Guid Id { get; set; }
        public string Ruc { get; set; }
        public string RazonSocial { get; set; }
        public string Periodo { get; set; }
        public string CarSunat { get; set; }
        public string FechaEmision { get; set; }
        public string FechaVencimiento { get; set; }
        public string TipoComprobante { get; set; }
        public string Serie { get; set; }
        public string Anio { get; set; }
        public string Numero { get; set; }
        public string NumeroFinalRango { get; set; }
        public string TipoDocIdentidad { get; set; }
        public string NumeroDocIdentidad { get; set; }
        public string NombreProveedor { get; set; }
        public decimal? BiGravadoDG { get; set; }
        public decimal? IgvDG { get; set; }
        public decimal? BiGravadoDGNG { get; set; }
        public decimal? IgvDGNG { get; set; }
        public decimal? BiGravadoDNG { get; set; }
        public decimal? IgvDNG { get; set; }
        public decimal? ValorAdqNG { get; set; }
        public decimal? Isc { get; set; }
        public decimal? Icbper { get; set; }
        public decimal? OtrosTributos { get; set; }
        public decimal? Total { get; set; }
        public string Moneda { get; set; }
        public decimal? TipoCambio { get; set; }
        public string FechaEmisionMod { get; set; }
        public string TipoCPMod { get; set; }
        public string SerieCPMod { get; set; }
        public string CodDam { get; set; }
        public string NumeroCPMod { get; set; }
        public string Clasificacion { get; set; }
        public string IdProyecto { get; set; }
        public decimal? PorcPart { get; set; }
        public decimal? Imb { get; set; }
        public string CarOrigen { get; set; }
        public string Detraccion { get; set; }
        public string TipoNota { get; set; }
        public string EstadoComprobante { get; set; }
        public string Incal { get; set; }
        public List<string> Clus { get; set; } = new();
        public bool TieneGlosa { get; set; }
        public string Glosa { get; set; }

    }
}
