namespace Investry.Application.Features.Projects.Queries.GetProjectDetails
{
    public class RewardTierDetailsDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public int? MaxBackers { get; set; }
        public int CurrentBackers { get; set; }

        public int RemainingBackers => MaxBackers.HasValue ? (MaxBackers.Value - CurrentBackers) : int.MaxValue;
        public bool IsAvailable => !MaxBackers.HasValue || (CurrentBackers < MaxBackers.Value);
    }
}
