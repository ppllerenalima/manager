namespace Manager.Infrastructure.FileAdapters
{
    public class PdfReader : IPdfReader
    {
        // 🔹 Extraer texto del PDF (para generar la glosa)
        public string ExtractTextFromPdf(byte[] pdfBytes)
        {
            using var pdf = PdfDocument.Open(pdfBytes);
            var textBuilder = new StringBuilder();

            foreach (var page in pdf.GetPages())
            {
                textBuilder.AppendLine(page.Text);
            }

            return textBuilder.ToString();
        }
    }
}
