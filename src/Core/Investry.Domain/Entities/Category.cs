namespace Investry.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string? Description { get; set; }
        
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
