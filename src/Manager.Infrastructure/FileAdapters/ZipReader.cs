namespace Manager.Infrastructure.FileAdapters
{
    public class ZipReader : IZipReader
    {
        // 🔹 Extrae el contenido de un archivo .xml dentro del ZIP
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

        // 🔹 Extrae el contenido de un archivo .json dentro del ZIP
        public string ExtractJsonFromZip(byte[] zipBytes)
        {
            using var memoryStream = new MemoryStream(zipBytes);
            using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

            var jsonEntry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase));
            if (jsonEntry == null)
                throw new InvalidOperationException("No se encontró ningún archivo JSON dentro del ZIP.");

            using var stream = jsonEntry.Open();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        // 🔹 Extrae el contenido de un archivo .pdf dentro del ZIP
        public byte[] ExtractPdfFromZip(byte[] zipBytes)
        {
            using var memoryStream = new MemoryStream(zipBytes);
            using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

            var entry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase));
            if (entry == null)
                throw new FileNotFoundException("No se encontró ningún archivo PDF dentro del ZIP.");

            using var pdfStream = entry.Open();
            using var ms = new MemoryStream();
            pdfStream.CopyTo(ms);
            return ms.ToArray();
        }
    }

}
