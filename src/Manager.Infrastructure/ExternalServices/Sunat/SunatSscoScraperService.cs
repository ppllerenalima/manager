using ClosedXML.Excel;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Infrastructure.ExternalServices.Sunat
{
    public class SunatSscoScraperService : ISunatSscoService
    {
        private readonly HttpClient _httpClient;
        private readonly ISunatSscoRepository _repository;
        private readonly ILogger<SunatSscoScraperService> _logger;

        public SunatSscoScraperService(
            HttpClient httpClient,
            ISunatSscoRepository repository,
            ILogger<SunatSscoScraperService> logger)
        {
            _httpClient = httpClient;
            _repository = repository;
            _logger = logger;
        }

        public async Task<(bool Actualizado, string Mensaje)> SincronizarPadronAsync(CancellationToken cancellationToken = default)
        {
            const string urlPagina = "https://www.sunat.gob.pe/padronesnotificaciones/sujeSinCapacidadOperativa.html";

            var html = await _httpClient.GetStringAsync(urlPagina, cancellationToken);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodoFecha = doc.DocumentNode.SelectSingleNode("//*[contains(text(), 'actualizada') or contains(text(), 'Actualizada')]");
            string textoFechaSunat = nodoFecha?.InnerText.Trim() ?? $"SinFecha_{DateTime.Now:yyyyMM}";

            if (await _repository.ExisteHistorialAsync("SSCO", textoFechaSunat, cancellationToken))
            {
                _logger.LogInformation("El padrón SSCO ya está actualizado ({Fecha}).", textoFechaSunat);
                return (false, $"El padrón ya se encuentra actualizado ({textoFechaSunat}).");
            }

            var nodoEnlace = doc.DocumentNode.SelectSingleNode("//a[contains(@href, '.xls') or contains(text(), 'Descargue')]");
            string href = nodoEnlace?.GetAttributeValue("href", "") ?? "";
            if (string.IsNullOrEmpty(href))
                throw new Exception("No se encontró el enlace al archivo Excel en la página de SUNAT.");

            var urlExcel = new Uri(new Uri(urlPagina), href).ToString();

            using var stream = await _httpClient.GetStreamAsync(urlExcel, cancellationToken);
            using var workbook = new XLWorkbook(stream);
            var hoja = workbook.Worksheet(1);

            var registrosExcel = new List<SujetoSinCapacidadOperativa>();

            foreach (var fila in hoja.RowsUsed())
            {
                string ruc = fila.Cell(1).GetString().Trim();
                if (ruc.Length != 11 || !long.TryParse(ruc, out _))
                    continue;

                registrosExcel.Add(new SujetoSinCapacidadOperativa
                {
                    Ruc = ruc,
                    RazonSocial = LimpiarTexto(fila.Cell(2).GetString()),
                    DomicilioFiscal = LimpiarTexto(fila.Cell(3).GetString()),
                    NumeroResolucion = LimpiarTexto(fila.Cell(4).GetString()),
                    FechaEmisionResolucion = ObtenerFechaCelda(fila.Cell(5)),
                    FechaFirmezaResolucion = ObtenerFechaCelda(fila.Cell(6)),
                    DocumentoRepresentanteLegal = LimpiarTexto(fila.Cell(7).GetString()),
                    NombreRepresentanteLegal = LimpiarTexto(fila.Cell(8).GetString()),
                    FechaPublicacion = ObtenerFechaCelda(fila.Cell(9)),
                    FechaActualizacionSistema = DateTime.Now
                });
            }

            if (registrosExcel.Count == 0)
                throw new Exception("No se encontraron registros válidos en el archivo Excel de SUNAT.");

            int nuevos = await _repository.GuardarOActualizarLoteAsync(registrosExcel, textoFechaSunat, cancellationToken);

            _logger.LogInformation("Padrón SSCO actualizado. Nuevos: {Nuevos}, Total procesado: {Total}", nuevos, registrosExcel.Count);
            return (true, $"Sincronización exitosa. Nuevos registros: {nuevos} (Total padrón: {registrosExcel.Count}).");
        }

        private static string LimpiarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return string.Empty;
            return Regex.Replace(texto.Trim(), @"\s+", " ");
        }

        private static DateOnly? ObtenerFechaCelda(IXLCell celda)
        {
            if (celda.IsEmpty()) return null;

            if (celda.DataType == XLDataType.DateTime)
                return DateOnly.FromDateTime(celda.GetDateTime());

            string valor = celda.GetString().Trim();
            string[] formatos = { "yyyy-MM-dd", "dd/MM/yyyy", "d/M/yyyy", "MM/dd/yyyy" };

            if (DateOnly.TryParseExact(valor, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                return fecha;

            return null;
        }
    }
}
