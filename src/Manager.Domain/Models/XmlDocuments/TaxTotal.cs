namespace Manager.Domain.Models.XmlDocuments
{
    public class TaxTotal
    {
        public decimal TaxAmount { get; set; }

        public TaxTotal(decimal taxAmount)
        {
            TaxAmount = taxAmount;
        }
    }
}
