namespace Manager.Domain.Repositories
{
    public interface IZipFileParser
    {
        Task<ICollection<string[]>> ExtractLinesAsync(byte[] archivoZip);
    }
}
