using Microsoft.AspNetCore.Mvc;
using System.ServiceModel;
using Manager.Reporting.SsrsClient;
using System.Net;

namespace Manager.Reporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ReportExecutionServiceSoapClient _client;

        public ReportsController()
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
            {
                Security = { Transport = { ClientCredentialType = HttpClientCredentialType.Ntlm } },
                MaxReceivedMessageSize = 50_000_000
            };

            var endpoint = new EndpointAddress("http://server01/ReportServer/ReportExecution2005.asmx");

            _client = new ReportExecutionServiceSoapClient(binding, endpoint);

            // Credenciales NTLM (Windows)
            _client.ClientCredentials.Windows.ClientCredential = new NetworkCredential(
                "Administrador", "Aa1234*", "DOMAIN"
            );
        }

        [HttpGet("comprobantes")]
        public async Task<IActionResult> GetComprobantesReport([FromQuery] Guid perTributarioId, [FromQuery] string format = "EXCEL")
        {
            try
            {
                // 1️⃣ Cargar reporte
                var loadResponse = await _client.LoadReportAsync(
                    new TrustedUserHeader(),
                    "/manager.reportes/Rpt_ComprobantesListadoPorPeriodoTributario",
                    null
                );

                string execId = loadResponse.executionInfo.ExecutionID;

                // 2️⃣ Asignar credenciales del DataSource (SQL)
                var credentials = new[]
                {
                    new DataSourceCredentials
                    {
                        DataSourceName = "dsServer01",  // nombre del DataSource en SSRS
                        UserName = "sa",
                        Password = "41707564@*/"
                    }
                };

                await _client.SetExecutionCredentialsAsync(new ExecutionHeader { ExecutionID = execId }, null, credentials);

                // 3️⃣ Establecer parámetros
                var parameters = new[]
                {
                    new ParameterValue { Name = "perTributarioId", Value = perTributarioId.ToString() }
                };

                await _client.SetExecutionParametersAsync(
                    new ExecutionHeader { ExecutionID = execId },
                    null,
                    parameters,
                    "es-PE"
                );

                // 4️⃣ Renderizar reporte
                var renderResponse = await _client.RenderAsync(
                    new RenderRequest
                    {
                        ExecutionHeader = new ExecutionHeader { ExecutionID = execId },
                        TrustedUserHeader = null,
                        Format = format,
                        DeviceInfo = null
                    });

                byte[] result = renderResponse.Result;
                string mimeType = renderResponse.MimeType ?? "application/octet-stream";

                return File(result, mimeType, $"Comprobantes_{perTributarioId}.{format.ToLower()}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar reporte: {ex.Message}");
            }
        }
    }
}
