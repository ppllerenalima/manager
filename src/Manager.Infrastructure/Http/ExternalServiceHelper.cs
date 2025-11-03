using Microsoft.AspNetCore.Http;

namespace Manager.Infrastructure.Http
{
    public static class ExternalServiceHelper
    {
        /// <summary>
        /// Limpia el contenido de error devuelto por un servicio externo (HTML o texto).
        /// </summary>
        public static string CleanErrorContent(string content, int statusCode)
        {
            if (string.IsNullOrWhiteSpace(content))
                return $"El servicio externo respondió con un error HTTP {statusCode}.";

            var trimmed = content.TrimStart();

            // 1️⃣ Si parece JSON → intentar parsearlo
            if (trimmed.StartsWith("{") || trimmed.StartsWith("["))
            {
                try
                {
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("message", out var messageElement))
                        return messageElement.GetString() ?? $"Error HTTP {statusCode}";

                    if (doc.RootElement.TryGetProperty("error", out var errorElement))
                        return errorElement.GetString() ?? $"Error HTTP {statusCode}";

                    return $"Error del servicio externo (HTTP {statusCode}).";
                }
                catch
                {
                    // Si no se puede parsear, devolvemos el texto crudo
                    return $"Error del servicio externo (HTTP {statusCode}).";
                }
            }

            // 2️⃣ Si parece HTML → extraer <title> o <h1>
            if (trimmed.StartsWith("<", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var titleMatch = Regex.Match(content, @"<title>(.*?)<\/title>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    var h1Match = Regex.Match(content, @"<h1>(.*?)<\/h1>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                    if (h1Match.Success)
                        return WebUtility.HtmlDecode(h1Match.Groups[1].Value.Trim());

                    if (titleMatch.Success)
                        return WebUtility.HtmlDecode(titleMatch.Groups[1].Value.Trim());

                    return $"El servicio externo devolvió una página de error (HTTP {statusCode}).";
                }
                catch
                {
                    return $"El servicio externo devolvió un error (HTTP {statusCode}).";
                }
            }

            // 3️⃣ Si no es JSON ni HTML → devolver texto plano
            return content.Length > 200
                ? $"El servicio externo devolvió un error extenso (HTTP {statusCode})."
                : content;
        }

        //public static string CleanErrorContent(string content, int statusCode)
        //{
        //    if (string.IsNullOrWhiteSpace(content))
        //        return $"El servicio externo respondió con un error HTTP {statusCode}.";

        //    var trimmed = content.TrimStart();

        //    // Si no parece HTML, devolverlo como está
        //    if (!trimmed.StartsWith("<", StringComparison.OrdinalIgnoreCase))
        //        return content;

        //    try
        //    {
        //        // Buscar <h1> o <title> para extraer mensaje legible
        //        var titleMatch = Regex.Match(content, @"<title>(.*?)<\/title>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        //        var h1Match = Regex.Match(content, @"<h1>(.*?)<\/h1>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        //        if (h1Match.Success)
        //            return WebUtility.HtmlDecode(h1Match.Groups[1].Value.Trim());

        //        if (titleMatch.Success)
        //            return WebUtility.HtmlDecode(titleMatch.Groups[1].Value.Trim());

        //        return $"El servicio externo devolvió una página de error (HTTP {statusCode}).";
        //    }
        //    catch
        //    {
        //        return $"El servicio externo devolvió un error (HTTP {statusCode}).";
        //    }
        //}
    }
}
