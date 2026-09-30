namespace Investry.Domain.Entities
{
    public class Founder
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; }

        // Navigation property

        public Wallet Wallet { get; set; }

        public List<Project> Projects { get; set; } = new();

        public int ReputationScore { get; set; } = 0;
        public int SuccessfulProjects { get; set; } = 0;
    }
}
