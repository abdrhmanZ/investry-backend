using Investry.Application.Common;
using Investry.Application.Features.Admin.Queries.GetAdminUsers;
using Investry.Application.Features.Users.Commands.UpdateProfile;
using Investry.Domain.Enums;

namespace Investry.Application.Contracts.Identity
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public KycStatus KycStatus { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? ProfilePicturePublicId { get; set; }
    }

    public interface IIdentityService
    {
        Task<Dictionary<string, string>> GetUserNamesByIdsAsync(List<string> userIds);
        Task<Dictionary<string, (string Name, string Email)>> GetUserInfoByIdsAsync(List<string> userIds);
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<bool> UpdateUserProfilePictureAsync(string userId, string url, string publicId);
        Task<Result<bool>> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<Result<bool>> UpdateProfileAsync(UpdateProfileCommand command);

        // Admin User Management
        Task<(List<AdminUserDto> Users, int TotalCount)> GetAdminUsersAsync(
            int page, int pageSize, UserRole? role, AccountStatus? status, string? search);
        Task<AdminUserDto?> GetAdminUserByIdAsync(string userId);
        Task<Result<bool>> BanUserAsync(string userId, string reason, DateTime? banExpiry);
        Task<Result<bool>> UnbanUserAsync(string userId);

        // Admin Accounts Management
        Task<List<Features.Admin.Queries.GetAdminAccounts.AdminAccountDto>> GetAllAdminsAsync();
        Task<Features.Admin.Queries.GetAdminAccounts.AdminAccountDto?> GetAdminByIdAsync(string adminId);
        Task<Result<Features.Admin.Queries.GetAdminAccounts.AdminAccountDto>> CreateAdminAsync(
            string firstName, string lastName, string email, string password);
        Task<Result<bool>> DeleteAdminAsync(string adminId, string currentAdminId);
    }
}
