namespace Manager.Domain.Models.XmlDocuments
{
    public class Supplier
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public Supplier(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
