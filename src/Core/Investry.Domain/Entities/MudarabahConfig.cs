using Investry.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investry.Domain.Entities
{
    public class MudarabahConfig
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProjectId { get; set; }
        public Project Project { get; set; }

        public decimal InvestorsProfitSharePercentage { get; set; }

        [NotMapped]
        public decimal FounderProfitSharePercentage => 100 - InvestorsProfitSharePercentage;

        public int DurationInMonths { get; set; }

        public ProfitDistributionFrequency ProfitDistributionFrequency { get; set; }

        // ده تاريخ المضاربة الفعلية بعد نجاح التمويل 
        public DateTime? MudarabahStartDate { get; set; }
        // تاريخ نهاية المضاربة
        public DateTime? MudarabahEndDate { get; set; }

        public bool IsDeleted { get; set; } = false;

        public ICollection<ProfitDistribution> ProfitDistributions { get; set; } = new List<ProfitDistribution>();
        public ICollection<CapitalReturn> CapitalReturns { get; set; } = new List<CapitalReturn>();
    }
}
