namespace Investry.Application.Features.Investments.Commands.CreateInvestment
{
    public class CreateInvestmentRequest
    {
        public Guid ProjectId { get; set; }
        public decimal Amount {  get; set; }
        public Guid? RewardTierId { get; set; }      // للـ Reward
        //public decimal? EquityPercentage { get; set; } // للـ Equity
        //public decimal? ProfitSharePercentage { get; set; } // للـ Mudarabah
    }
}
