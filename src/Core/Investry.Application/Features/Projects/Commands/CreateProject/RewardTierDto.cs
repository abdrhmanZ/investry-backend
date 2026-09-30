namespace Investry.Application.Features.Projects.Commands.CreateProject
{
    public class RewardTierDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int? MaxBackers { get; set; }
    }
}
