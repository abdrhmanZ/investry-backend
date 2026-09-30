using Investry.Domain.Enums;

namespace Investry.Domain.Entities
{
    public class ProjectMedia
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProjectId { get; set; }
        public Project Project { get; set; }

        public string MediaUrl { get; set; }
        public string PublicId { get; set; }

        public MediaType Type { get; set; }

        public bool IsCover { get; set; } = false;

        public bool IsDeleted { get; set; } = false;
    }
}
