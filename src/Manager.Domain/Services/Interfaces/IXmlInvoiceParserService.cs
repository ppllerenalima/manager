namespace Manager.Domain.Services.Interfaces
{
    public interface IXmlInvoiceParserService
    {
        Invoice ParseInvoice(string xmlContent);
    }
}
