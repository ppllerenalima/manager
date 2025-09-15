namespace Manager.Domain.Models.XmlDocuments
{
    public class Invoice
    {
        public string Id { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public Supplier Supplier { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
        public List<InvoiceLine> InvoiceLines { get; set; } = new();
        public TaxTotal TaxTotal { get; set; } = null!;
        public LegalMonetaryTotal LegalMonetaryTotal { get; set; } = null!;
        public string Status { get; set; } = string.Empty;

        public Invoice(
            string id,
            DateTime issueDate,
            Supplier supplier,
            Customer customer,
            List<InvoiceLine> invoiceLines,
            TaxTotal taxTotal,
            LegalMonetaryTotal legalMonetaryTotal,
            string status)
        {
            Id = id;
            IssueDate = issueDate;
            Supplier = supplier;
            Customer = customer;
            InvoiceLines = invoiceLines;
            TaxTotal = taxTotal;
            LegalMonetaryTotal = legalMonetaryTotal;
            Status = status;
        }
    }
}
