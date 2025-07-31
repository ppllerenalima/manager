namespace Manager.Domain.Entities
{
    public class EntityBase
    {
        public Guid Id { get; set; }
        public bool IsInactive { get; set; } = true;
    }
}
