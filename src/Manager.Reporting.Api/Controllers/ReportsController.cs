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
        private readonly IConfiguration _configuration;

        public ReportsController(IConfiguration configuration)
        {
            _configuration = configuration;

            // Leer la URL desde appsettings
            string reportUrl = _configuration["ReportingSettings:Url"];

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
            {
                Security = { Transport = { ClientCredentialType = HttpClientCredentialType.Ntlm } },
                MaxReceivedMessageSize = 50_000_000
            };

            var endpoint = new EndpointAddress($"{reportUrl}/ReportServer/ReportExecution2005.asmx");

            _client = new ReportExecutionServiceSoapClient(binding, endpoint);

            // Opcional: También puedes jalar las credenciales desde el JSON
            _client.ClientCredentials.Windows.ClientCredential = new NetworkCredential(
                _configuration["ReportingSettings:User"],
                _configuration["ReportingSettings:Password"],
                _configuration["ReportingSettings:Domain"]
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
                        DataSourceName = _configuration["ReportingSettings:DataSource:Name"],
                        UserName = _configuration["ReportingSettings:DataSource:User"],
                        Password = _configuration["ReportingSettings:DataSource:Password"]
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
