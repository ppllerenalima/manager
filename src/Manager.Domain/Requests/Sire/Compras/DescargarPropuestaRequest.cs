namespace Manager.Domain.Requests.Sire.Compras
{
    public class DescargarPropuestaRequest
    {
        public string Token { get; set; } // Requerido en el header

        public string PerTributario { get; set; } // Requerido
        public int CodTipoArchivo { get; set; } = 0; // Requerido
        public string CodOrigenEnvio { get; set; } = "2"; // Requerido (2)

        public string NumSerieCDP { get; set; } // Requerido
        public string NumCDP { get; set; } // Requerido

        public string FecEmisionIni { get; set; } // Opcional
        public string FecEmisionFin { get; set; } // Opcional
        public string CodTipoCDP { get; set; } = "01"; // Opcional
        public string CodInconsistencia { get; set; } // Opcional
        public string CodCar { get; set; } // Opcional
        public string NumDocAdquiriente { get; set; } // Opcional
        public decimal? MtoDesde { get; set; } // Opcional
        public decimal? MtoHasta { get; set; } // Opcional
    }
}
