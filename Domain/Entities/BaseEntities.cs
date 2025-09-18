namespace Domain.Entities
{
    public abstract class BaseEntities
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTime DateCreated { get; init; } = DateTime.UtcNow;
        public DateTime DateModified { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}