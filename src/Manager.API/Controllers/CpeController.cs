using Manager.Domain.Requests.Cpe;
using Manager.Domain.Responses.CpeResponses;
using Manager.Domain.Services.Interfaces;
using System.IO.Compression;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CpeController : ControllerBase
    {
        private readonly ICpeService _cpeService;
        private readonly ITokenService _tokenService;
        private readonly ITokenBaseService _tokenBaseService;
        private readonly ICuentaBaseSolService _cuentaBaseSolService;


        public CpeController(ICpeService cpeService, ITokenService tokenService, ITokenBaseService tokenBaseService, ICuentaBaseSolService cuentaBaseSolService)
        {
            _cpeService = cpeService;
            _tokenService = tokenService;
            _tokenBaseService = tokenBaseService;
            _cuentaBaseSolService = cuentaBaseSolService;
        }

        [HttpPost("descargar-zip")]
        public async Task<IActionResult> DescargarZip([FromQuery] Guid clienteId, [FromBody] DescargarZipRequest request)
        {
            // 1️⃣ Obtener token limpio
            var token = await _tokenService.GetOrGenerateActiveTokenAsync(clienteId);

            // 2️⃣ Ejecutar el proceso con fallback (ControlCpe → ConsultaCpe)
            var result = await _cpeService.DescargarZipAsync(token.Data.AccessToken, request);

            // 3️⃣ Validar si la operación fue exitosa
            if (!result.Success || result.Data == null)
                return StatusCode(result.StatusCode, result); // Devuelve el mismo response estándar

            // 4️⃣ Preparar el archivo descargado
            var fileName = $"{result.Data.NombreArchivo}.zip";
            return File(result.Data.Archivo, "application/zip", fileName);
        }

        /// <summary>
        /// Descarga el archivo ZIP o PDF asociado a un comprobante electrónico,
        /// consultando los servicios de SUNAT según los parámetros enviados.
        /// </summary>
        /// <param name="clienteId">Identificador único del cliente solicitante.</param>
        /// <param name="request">
        /// Objeto que contiene los datos necesarios del comprobante a descargar.
        /// </param>
        /// <returns>
        /// Retorna un objeto <see cref="IActionResult"/> con el resultado de la operación:
        /// - <c>200 OK</c> si la descarga se realiza correctamente.
        /// - <c>400 Bad Request</c> si los parámetros son inválidos o la solicitud es nula.
        /// - <c>500 Internal Server Error</c> si ocurre un error durante el proceso.
        /// </returns>
        /// <remarks>
        /// Este método obtiene un token activo del cliente, consulta el servicio de descarga
        /// y devuelve el archivo procesado en formato PDF o ZIP.
        /// </remarks>
        [HttpPost("descargar-zip-pdf")]
        public async Task<IActionResult> DescargarZip_Pdf([FromQuery] Guid clienteId, [FromBody] DescargarZipRequest request)
        {
            if (request == null)
            {
                return StatusCode(400,
                    ResponseFactory.Error<DescargarZipResponse>(
                        "La solicitud no puede ser nula.",
                        "REQ_NULL",
                        400
                    )
                );
            }

            try
            {
                // 1️⃣ Obtener token válido del cliente
                var token = await _tokenService.GetOrGenerateActiveTokenAsync(clienteId);

                // 2️⃣ Descargar desde los servicios SUNAT (con fallback)
                var response = await _cpeService.DescargarPorConsultaCpeAsync(token.Data.AccessToken, request);

                // 3️⃣ Retornar archivo o error
                if (response.Success)
                {
                    return ProcesarPdfDesdeZip(response.Data);
                }

                return StatusCode(response.StatusCode > 0 ? response.StatusCode : 500, response);
            }
            catch (Exception ex)
            {
                // 4️⃣ Manejo global de excepciones
                return StatusCode(
                    500,
                    ResponseFactory.Error<DescargarZipResponse>(
                        $"Error inesperado: {ex.Message}",
                        "EX",
                        500
                    )
                );
            }
        }

        /// <summary>
        /// Actualiza los permisos de una aplicación registrada ante SUNAT.
        /// </summary>
        /// <remarks>
        /// Este endpoint envía una solicitud HTTP PUT al servicio de SUNAT para actualizar los
        /// recursos autorizados de una aplicación.
        /// 
        /// Se debe pasar el token de autenticación (Bearer) y los datos de la aplicación
        /// mediante parámetros de consulta (query string).
        /// 
        /// Ejemplo de uso:
        /// PUT /api/cpe/actualizar-permisos?token={token}&id={idApp}&nomApp={nombre}&desUrlApp={url}
        /// </remarks>
        /// <param name="token">Token JWT emitido por SUNAT (autorización tipo Bearer).</param>
        /// <param name="request">Objeto con los datos de la aplicación que se desea actualizar.</param>
        /// <returns>
        /// Retorna un objeto <see cref="BaseResponseGeneric{T}"/> con el resultado de la operación.
        /// - <c>Success = true</c> si la actualización fue exitosa.
        /// - <c>Success = false</c> si ocurrió un error al comunicarse con SUNAT.
        /// </returns>
        [HttpPut("actualizar-permisos")]
        public async Task<IActionResult> ActualizarPermisos(
            [FromQuery] string token,
            [FromQuery] ActualizarPermisosRequest request)
        {
            try
            {
                // 🔹 Llamar al servicio que hace el PUT a SUNAT
                var response = await _cpeService.ActualizarPermisosAsync(token, request);

                // 🔹 Retornar el resultado al cliente
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFactory.Error<string>(ex.Message, "EXCEPTION", 500));
            }
        }


        private IActionResult ProcesarPdfDesdeZip(DescargarZipResponse result)
        {
            try
            {
                using var zipStream = new MemoryStream(result.Archivo);
                using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

                var entry = archive.Entries.FirstOrDefault(e =>
                    e.FullName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase));

                if (entry == null)
                    return StatusCode(404, ResponseFactory.Error<DescargarZipResponse>("No se encontró un PDF dentro del ZIP.", "NO_PDF", 404));

                using var entryStream = entry.Open();
                using var ms = new MemoryStream();
                entryStream.CopyTo(ms);
                ms.Position = 0; // 🔑 necesario

                return File(ms.ToArray(), "application/pdf", entry.Name);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ResponseFactory.Error<DescargarZipResponse>(ex.Message, "EX", 500));
            }
        }
    }
}
