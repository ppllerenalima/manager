using Manager.Domain.Services.Interfaces;

namespace Manager.Domain.Services
{
    public class XmlInvoiceParserService : IXmlInvoiceParserService
    {
        private static readonly XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        private static readonly XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2";

        public Invoice ParseInvoice(string xmlContent)
        {
            var doc = XDocument.Parse(xmlContent);

            var supplier = new Supplier(
                doc.Descendants(cac + "AccountingSupplierParty")
                   .Descendants(cbc + "ID").FirstOrDefault()?.Value ?? string.Empty,
                doc.Descendants(cac + "AccountingSupplierParty")
                   .Descendants(cbc + "Name").FirstOrDefault()?.Value ?? string.Empty
            );

            var customer = new Customer(
                doc.Descendants(cac + "AccountingCustomerParty")
                   .Descendants(cbc + "ID").FirstOrDefault()?.Value ?? string.Empty,
                doc.Descendants(cac + "AccountingCustomerParty")
                   .Descendants(cbc + "Name").FirstOrDefault()?.Value ?? string.Empty
            );

            var taxTotal = new TaxTotal(
                decimal.Parse(doc.Descendants(cac + "TaxTotal")
                                 .Descendants(cbc + "TaxAmount").FirstOrDefault()?.Value ?? "0")
            );

            var monetaryTotal = new LegalMonetaryTotal(
                decimal.Parse(doc.Descendants(cac + "LegalMonetaryTotal")
                                 .Descendants(cbc + "PayableAmount").FirstOrDefault()?.Value ?? "0")
            );

            var invoiceLines = doc.Descendants(cac + "InvoiceLine").Select(line =>
                new InvoiceLine(
                    line.Element(cbc + "ID")?.Value ?? string.Empty,
                    line.Descendants(cac + "Item").Descendants(cbc + "Description").FirstOrDefault()?.Value ?? string.Empty,
                    decimal.Parse(line.Element(cbc + "InvoicedQuantity")?.Value ?? "0"),
                    decimal.Parse(line.Descendants(cac + "Price").Descendants(cbc + "PriceAmount").FirstOrDefault()?.Value ?? "0"),
                    decimal.Parse(line.Element(cbc + "LineExtensionAmount")?.Value ?? "0")
                )
            ).ToList();

            var invoice = new Invoice(
                doc.Descendants(cbc + "ID").FirstOrDefault()?.Value ?? string.Empty,
                DateTime.Parse(doc.Descendants(cbc + "IssueDate").FirstOrDefault()?.Value ?? DateTime.Now.ToString("yyyy-MM-dd")),
                supplier,
                customer,
                invoiceLines,
                taxTotal,
                monetaryTotal,
                "" // status pendiente
            );

            return invoice;
        }
    }
}
