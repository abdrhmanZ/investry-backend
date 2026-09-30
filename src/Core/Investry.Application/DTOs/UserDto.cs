using Investry.Domain.Enums;

namespace Investry.Application.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = null!;
        public KycStatus KycStatus { get; set; } = KycStatus.Pending;
    }
}
