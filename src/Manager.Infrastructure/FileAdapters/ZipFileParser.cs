namespace Manager.Infrastructure.FileAdapters
{
    public class ZipFileParser : IZipFileParser
    {
        public async Task<ICollection<string[]>> ExtractLinesAsync(byte[] archivoZip)
        {
            var result = new List<string[]>();

            using var stream = new MemoryStream(archivoZip);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            var txtEntry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith(".txt"));
            if (txtEntry == null)
                throw new ApplicationException("El ZIP no contiene un archivo .txt válido.");

            using var reader = new StreamReader(txtEntry.Open());
            string? line;
            bool isFirstLine = true;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (isFirstLine) { isFirstLine = false; continue; }
                if (string.IsNullOrWhiteSpace(line)) continue;

                result.Add(line.Split('|'));
            }

            return result;
        }
    }
}
