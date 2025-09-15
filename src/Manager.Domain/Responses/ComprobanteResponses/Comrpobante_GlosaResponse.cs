namespace Manager.Domain.Responses.ComprobanteResponses
{
    public class Comrpobante_GlosaResponse
    {
        /// <summary>
        /// Indica si el proceso fue exitoso.
        /// </summary>
        public bool EsExito { get; set; }

        /// <summary>
        /// Código de estado de la respuesta (HTTP o interno).
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Nombre del archivo recibido (ejemplo: RUC-Tipo-Serie-Numero.zip).
        /// </summary>
        public string? NombreArchivo { get; set; }

        /// <summary>
        /// Lista de errores ocurridos durante el proceso.
        /// </summary>
        public List<ErrorComrpobante_Response> Errores { get; set; } = new();
    }

    public class ErrorComrpobante_Response
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
    }
}
