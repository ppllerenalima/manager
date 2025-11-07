namespace Manager.Domain.Repositories
{
    public interface IZipReader
    {
        string ExtractXmlFromZip(byte[] zipFile);
        string ExtractJsonFromZip(byte[] zipBytes);
        byte[] ExtractPdfFromZip(byte[] zipBytes);
    }
}
