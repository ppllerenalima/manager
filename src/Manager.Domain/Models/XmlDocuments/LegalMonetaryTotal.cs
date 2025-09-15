namespace Manager.Domain.Models.XmlDocuments
{
    public class LegalMonetaryTotal
    {
        public decimal PayableAmount { get; set; }

        public LegalMonetaryTotal(decimal payableAmount)
        {
            PayableAmount = payableAmount;
        }
    }
}
