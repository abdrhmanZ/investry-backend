using System.ComponentModel.DataAnnotations.Schema;

namespace Investry.Domain.Entities
{
    public class EquityConfig
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProjectId { get; set; }
        public Project Project { get; set; }

        public decimal EquityPercentage { get; set; } // نسبة الاسهم المخصصة للمستثمرين

        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public decimal CompanyValuation { get
            {
                if (EquityPercentage <= 0 || Project is null) return 0;
                return Project.TargetAmount / (EquityPercentage / 100);
            } 
        } // قيمة الشركة بالكامل

        public ICollection<InvestorShare>? InvestorShares { get; set; } = new List<InvestorShare>();
    }
}
