using System.ServiceModel;
using Manager.Reporting.SsrsClient;

namespace Manager.Reporting.Api.Services
{
    public class SsrsReportService
    {
        private readonly ReportExecutionServiceSoapClient _client;

        public SsrsReportService(string reportServerUrl, string username, string password)
        {
            // Configuración del binding
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly)
            {
                MaxReceivedMessageSize = 2147483647, // Para PDFs grandes
                Security =
                {
                    Transport = { ClientCredentialType = HttpClientCredentialType.Basic }
                }
            };

            var endpoint = new EndpointAddress(reportServerUrl);

            _client = new ReportExecutionServiceSoapClient(binding, endpoint);

            // Autenticación (usuario SQL o Windows según config SSRS)
            _client.ClientCredentials.UserName.UserName = username;
            _client.ClientCredentials.UserName.Password = password;
        }

        public async Task<byte[]> RenderComprobantesReportAsync(Guid perTributarioId)
        {
            var trustedHeader = new TrustedUserHeader();

            // 1. Cargar el reporte
            var loadResponse = await _client.LoadReportAsync(
                trustedHeader,
                "/manager.reportes/Rpt_ComprobantesListadoPorPeriodoTributario",
                null
            );

            string executionId = loadResponse.executionInfo.ExecutionID;

            var execHeader = new ExecutionHeader { ExecutionID = executionId };

            // 2. Pasar parámetros al reporte
            var parameters = new[]
            {
                new ParameterValue
                {
                    Name = "perTributarioId",
                    Value = perTributarioId.ToString()
                }
            };

            await _client.SetExecutionParametersAsync(execHeader, trustedHeader, parameters, "en-US");

            // 3. Renderizar el reporte en PDF
            var request = new RenderRequest
            {
                ExecutionHeader = execHeader,
                TrustedUserHeader = trustedHeader,
                Format =  "PDF", // Formato (PDF, EXCEL, WORD, etc.)
                DeviceInfo = null
            };

            var renderResponse = await _client.RenderAsync(request);

            return renderResponse.Result;
        }
    }
}
