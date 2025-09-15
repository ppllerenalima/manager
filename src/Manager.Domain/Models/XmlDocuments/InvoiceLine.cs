namespace Manager.Domain.Models.XmlDocuments
{
    public class InvoiceLine
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineExtensionAmount { get; set; }

        public InvoiceLine(
            string id,
            string description,
            decimal quantity,
            decimal unitPrice,
            decimal lineExtensionAmount)
        {
            Id = id;
            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
            LineExtensionAmount = lineExtensionAmount;
        }
    }
}
