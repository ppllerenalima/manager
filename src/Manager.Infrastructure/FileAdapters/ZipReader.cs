namespace Manager.Infrastructure.FileAdapters
{
    public class ZipReader : IZipReader
    {
        public string? ExtractXmlFromZip(byte[] zipFile)
        {
            using (var ms = new MemoryStream(zipFile))
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Read))
            {
                var entry = archive.Entries
                    .FirstOrDefault(e => e.FullName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));

                if (entry == null)
                    return null;

                using (var reader = new StreamReader(entry.Open()))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

}
