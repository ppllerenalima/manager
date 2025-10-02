using Manager.Reporting.SsrsClient;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.ServiceModel;

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

            var endpoint = new EndpointAddress("http://192.168.1.38/ReportServer/ReportExecution2005.asmx");

            _client = new ReportExecutionServiceSoapClient(binding, endpoint);

            // Credenciales NTLM (Windows)
            _client.ClientCredentials.Windows.ClientCredential = new NetworkCredential(
                "Administrador", "Aa1234*", "server01"
            );
        }

        [HttpGet("comprobantes")]
        public async Task<IActionResult> GetComprobantesReport([FromQuery] Guid perTributarioId, [FromQuery] string format = "EXCELOPENXML")
        {
            try
            {
                var loadResponse = await _client.LoadReportAsync(
                    new TrustedUserHeader(),
                    "/Manager.Reporting/Rpt_ComprobantesListadoPorPeriodoTributario",
                    null
                );

                string execId = loadResponse.executionInfo.ExecutionID;

                var credentials = new[]
                {
                    new DataSourceCredentials
                    {
                        DataSourceName = "dsServer01",
                        UserName = "sa",
                        Password = "P@ssw0rd"
                    }
                };
                await _client.SetExecutionCredentialsAsync(new ExecutionHeader { ExecutionID = execId }, null, credentials);

                var parameters = new[]
                {
                    new ParameterValue { Name = "perTributarioId", Value = perTributarioId.ToString() }
        };
                await _client.SetExecutionParametersAsync(new ExecutionHeader { ExecutionID = execId }, null, parameters, "es-PE");

                // Usar EXCELOPENXML para .xlsx
                var renderResponse = await _client.RenderAsync(
                    new RenderRequest
                    {
                        ExecutionHeader = new ExecutionHeader { ExecutionID = execId },
                        TrustedUserHeader = null,
                        Format = format, // "EXCELOPENXML"
                        DeviceInfo = null
                    });

                byte[] result = renderResponse.Result;
                string mimeType = renderResponse.MimeType ?? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string extension = format == "EXCELOPENXML" ? "xlsx" : "xls";

                return File(result, mimeType, $"Comprobantes_{perTributarioId}.{extension}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al generar reporte: {ex.Message}");
            }
        }
    }
}
