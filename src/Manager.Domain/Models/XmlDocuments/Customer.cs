namespace Manager.Domain.Models.XmlDocuments
{
    public class Customer
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public Customer(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
