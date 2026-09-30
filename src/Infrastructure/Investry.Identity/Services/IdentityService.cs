using AutoMapper;
using Investry.Application.Common;
using Investry.Application.Contracts.Identity;
using Investry.Application.Features.Admin.Queries.GetAdminAccounts;
using Investry.Application.Features.Admin.Queries.GetAdminUsers;
using Investry.Application.Features.Users.Commands.UpdateProfile;
using Investry.Domain.Enums;
using Investry.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Investry.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public IdentityService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Dictionary<string, string>> GetUserNamesByIdsAsync(List<string> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return new Dictionary<string, string>();
            }

            var users = await _userManager.Users
                                          .Where(u => userIds.Contains(u.Id))
                                          .ToDictionaryAsync(u => u.Id, u => $"{u.FirstName} {u.LastName}");

            return users;
        }

        public async Task<Dictionary<string, (string Name, string Email)>> GetUserInfoByIdsAsync(List<string> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return new Dictionary<string, (string Name, string Email)>();
            }

            var users = await _userManager.Users
                                          .Where(u => userIds.Contains(u.Id))
                                          .ToDictionaryAsync(
                                              u => u.Id,
                                              u => ($"{u.FirstName} {u.LastName}", u.Email ?? string.Empty));

            return users;
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> UpdateUserProfilePictureAsync(string userId, string url, string publicId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.ProfilePictureUrl = url;
            user.ProfilePicturePublicId = publicId;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<Result<bool>> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return Result<bool>.Failure(new List<Error> { new Error("User.NotFound", "User not found.", ErrorType.NotFound) });
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!changePasswordResult.Succeeded)
            {
                var errors = changePasswordResult.Errors
                    .Select(e => new Error("Identity.Error", e.Description, ErrorType.Validation))
                    .ToList();
                return Result<bool>.Failure(errors);
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> UpdateProfileAsync(UpdateProfileCommand command)
        {
            var user = await _userManager.FindByIdAsync(command.UserId);
            if (user is null)
            {
                return Result<bool>.Failure(new List<Error> { new Error("User.NotFound", "User not found.", ErrorType.NotFound) });
            }

            bool hasChanges = false;

            if (command.FirstName != null && user.FirstName != command.FirstName)
            {
                user.FirstName = command.FirstName;
                hasChanges = true;
            }

            if (command.LastName != null && user.LastName != command.LastName)
            {
                user.LastName = command.LastName;
                hasChanges = true;
            }

            if (command.PhoneNumber != null && user.PhoneNumber != command.PhoneNumber)
            {
                user.PhoneNumber = command.PhoneNumber;
                hasChanges = true;
            }

            if (!hasChanges)
            {
                return Result<bool>.Success(true);
            }

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors
                    .Select(e => new Error("Identity.Error", e.Description, ErrorType.Validation))
                    .ToList();
                return Result<bool>.Failure(errors);
            }

            return Result<bool>.Success(true);
        }

        // ─── Admin User Management ──────────────────────────────────────

        public async Task<(List<AdminUserDto> Users, int TotalCount)> GetAdminUsersAsync(
            int page, int pageSize, UserRole? role, AccountStatus? status, string? search)
        {
            var query = _userManager.Users
                .Where(u => u.Role != UserRole.Administrator)
                .AsNoTracking();

            if (role.HasValue)
                query = query.Where(u => u.Role == role.Value);

            if (status.HasValue)
                query = query.Where(u => u.AccountStatus == status.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(term) ||
                    u.LastName.ToLower().Contains(term) ||
                    u.Email!.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new AdminUserDto
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email ?? string.Empty,
                    Role = u.Role.ToString(),
                    Status = u.AccountStatus == AccountStatus.Suspended ? "Banned" : "Active",
                    KycStatus = MapKycStatus(u.KycStatus),
                    CreatedAt = u.CreatedAt,
                    BanReason = u.BanReason,
                    BanExpiry = u.BanExpiry
                })
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<AdminUserDto?> GetAdminUserByIdAsync(string userId)
        {
            var user = await _userManager.Users
                .AsNoTracking()
                .Where(u => u.Id == userId && u.Role != UserRole.Administrator)
                .Select(u => new AdminUserDto
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email ?? string.Empty,
                    Role = u.Role.ToString(),
                    Status = u.AccountStatus == AccountStatus.Suspended ? "Banned" : "Active",
                    KycStatus = MapKycStatus(u.KycStatus),
                    CreatedAt = u.CreatedAt,
                    BanReason = u.BanReason,
                    BanExpiry = u.BanExpiry
                })
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<Result<bool>> BanUserAsync(string userId, string reason, DateTime? banExpiry)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result<bool>.Failure(new List<Error>
                {
                    new("User.NotFound", "User not found.", ErrorType.NotFound)
                });

            if (user.Role == UserRole.Administrator)
                return Result<bool>.Failure(new List<Error>
                {
                    new("Ban.Forbidden", "Cannot ban an administrator.", ErrorType.Forbidden)
                });

            if (user.AccountStatus == AccountStatus.Suspended)
                return Result<bool>.Failure(new List<Error>
                {
                    new("Ban.AlreadyBanned", "User is already banned.", ErrorType.Validation)
                });

            user.AccountStatus = AccountStatus.Suspended;
            user.BanReason = reason;
            user.BanExpiry = banExpiry;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(new List<Error>
                  {
                      new("Ban.Failed", "Failed to ban user.", ErrorType.Failure)
                  });
        }

        public async Task<Result<bool>> UnbanUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result<bool>.Failure(new List<Error>
                {
                    new("User.NotFound", "User not found.", ErrorType.NotFound)
                });

            if (user.AccountStatus != AccountStatus.Suspended)
                return Result<bool>.Failure(new List<Error>
                {
                    new("Unban.NotBanned", "User is not currently banned.", ErrorType.Validation)
                });

            user.AccountStatus = AccountStatus.Active;
            user.BanReason = null;
            user.BanExpiry = null;

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(new List<Error>
                  {
                      new("Unban.Failed", "Failed to unban user.", ErrorType.Failure)
                  });
        }

        // ─── Helpers ────────────────────────────────────────────────────

        private static string MapKycStatus(KycStatus status) => status switch
        {
            KycStatus.Approved => "Verified",
            KycStatus.Declined or KycStatus.Expired => "Declined",
            KycStatus.Pending or KycStatus.InReview or KycStatus.Resubmitted => "Pending",
            _ => "NotStarted"
        };

        // ─── Admin Accounts Management ──────────────────────────────────

        public async Task<List<AdminAccountDto>> GetAllAdminsAsync()
        {
            return await _userManager.Users
                .AsNoTracking()
                .Where(u => u.Role == UserRole.Administrator)
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new AdminAccountDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email ?? string.Empty,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin
                })
                .ToListAsync();
        }

        public async Task<AdminAccountDto?> GetAdminByIdAsync(string adminId)
        {
            return await _userManager.Users
                .AsNoTracking()
                .Where(u => u.Id == adminId && u.Role == UserRole.Administrator)
                .Select(u => new AdminAccountDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email ?? string.Empty,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Result<AdminAccountDto>> CreateAdminAsync(
            string firstName, string lastName, string email, string password)
        {
            if (await _userManager.FindByEmailAsync(email) is not null)
                return Result<AdminAccountDto>.Failure(new List<Error>
                {
                    new("Admin.EmailExists", "An admin with this email already exists.", ErrorType.Conflict)
                });

            var user = new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                Role = UserRole.Administrator,
                EmailConfirmed = true,
                AccountStatus = AccountStatus.Active
            };

            var createResult = await _userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors
                    .Select(e => new Error("Admin.CreationFailed", e.Description, ErrorType.Failure))
                    .ToList();
                return Result<AdminAccountDto>.Failure(errors);
            }

            await _userManager.AddToRoleAsync(user, "Administrator");

            return Result<AdminAccountDto>.Success(new AdminAccountDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                CreatedAt = user.CreatedAt,
                LastLogin = null
            });
        }

        public async Task<Result<bool>> DeleteAdminAsync(string adminId, string currentAdminId)
        {
            var user = await _userManager.FindByIdAsync(adminId);

            if (user is null || user.Role != UserRole.Administrator)
                return Result<bool>.Failure(new List<Error>
                {
                    new("Admin.NotFound", "Admin not found.", ErrorType.NotFound)
                });

            var adminCount = await _userManager.Users
                .CountAsync(u => u.Role == UserRole.Administrator);

            if (adminCount <= 1)
                return Result<bool>.Failure(new List<Error>
                {
                    new("Admin.LastAdmin", "Cannot delete the last admin account.", ErrorType.Validation)
                });

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded
                ? Result<bool>.Success(true)
                : Result<bool>.Failure(new List<Error>
                  {
                      new("Admin.DeleteFailed", "Failed to delete admin.", ErrorType.Failure)
                  });
        }
    }
}
