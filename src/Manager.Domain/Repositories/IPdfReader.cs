namespace Manager.Domain.Repositories
{
    public interface IPdfReader
    {
        string ExtractTextFromPdf(byte[] pdfBytes);
    }
}
