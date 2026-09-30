using Google.Apis.Auth;
using Investry.Application.Common;
using Investry.Application.Constants;
using Investry.Application.Contracts.Identity;
using Investry.Application.Contracts.Infrastructure;
using Investry.Application.Contracts.Persistence;
using Investry.Application.Models.Email;
using Investry.Application.Models.Identity;
using Investry.Domain.Entities;
using Investry.Domain.Enums;
using Investry.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Investry.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailService _emailService;
        private readonly IUrlGenerator _urlGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings,
            IEmailService emailService,
            IUrlGenerator urlGenerator,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _urlGenerator = urlGenerator;
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtSettings.Value;
        }
        public async Task<Result<AuthResponse>> LoginAsync(AuthRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Result<AuthResponse>.Failure(new List<Error>{
                    new Error("Auth.InvalidCredentials",
                    "Email or password is incorrect!"
                    , ErrorType.Validation)
                });
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Result<AuthResponse>.Failure(new List<Error>
                {
                    new Error("Auth.EmailNotConfirmed",
                              "Email is not confirmed. Please confirm your email before logging in.",
                              ErrorType.Validation)
                });
            }

            JwtSecurityToken jwtSecurityToken = await GenerateToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            var oldRefreshToken = user.RefreshTokens
                .FirstOrDefault(t => t.RevokedOn == null && t.ExpiresOn > DateTime.UtcNow);

            if (oldRefreshToken != null)
            {
                oldRefreshToken.RevokedOn = DateTime.UtcNow;
            }

            var plainToken = GenerateRefreshToken(out var refreshToken);
            user.RefreshTokens.Add(refreshToken);
            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            var response = new AuthResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,
                Roles = roles.ToList(),
                IsAuthenticated = true,
                RefreshToken = plainToken,
                RefreshTokenExpiration = refreshToken.ExpiresOn,
                Message = "Login successful"
            };

            return Result<AuthResponse>.Success(response);
        }

        public async Task<Result<string>> LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Result<string>.Failure(new List<Error>
                {
                    new Error("Auth.AlreadyLoggedOut",
                              "User already logged out",
                              ErrorType.Validation)
                });
            }

            var tokenHash = ComputeSha256Hash(refreshToken);

            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u =>
                    u.RefreshTokens.Any(t =>
                        t.Token == tokenHash &&
                        t.RevokedOn == null &&
                        t.ExpiresOn > DateTime.UtcNow));

            if (user is null)
            {
                return Result<string>.Failure(new List<Error>
                {
                    new Error("Auth.InvalidToken",
                              "Invalid token",
                              ErrorType.NotFound)
                });
            }

            var token = user.RefreshTokens.Single(t => t.Token == tokenHash);

            token.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return Result<string>.Success("Logged out successfully");
        }

        public async Task<Result<AuthResponse>> RefreshTokenAsync(string plainToken)
        {
            if (string.IsNullOrEmpty(plainToken))
            {
                return Result<AuthResponse>.Failure(new List<Error>
                {
                    new Error("Auth.RefreshTokenMissing",
                              "Refresh token is missing.",
                              ErrorType.Validation)
                });
            }

            var tokenHash = ComputeSha256Hash(plainToken);

            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u =>
                    u.RefreshTokens.Any(t =>
                        t.Token == tokenHash &&
                        t.RevokedOn == null &&
                        t.ExpiresOn > DateTime.UtcNow));

            if (user is null)
            {
                return Result<AuthResponse>.Failure(new List<Error>
                {
                    new Error("Auth.InvalidToken",
                              "Invalid token",
                              ErrorType.NotFound)
                });
            }

            var oldRefreshToken = user.RefreshTokens
                .SingleOrDefault(t => t.Token == tokenHash);

            if (oldRefreshToken != null)
            {
                oldRefreshToken.RevokedOn = DateTime.UtcNow;
            }

            var newPlainToken = GenerateRefreshToken(out var newRefreshToken);
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await GenerateToken(user);

            var roles = await _userManager.GetRolesAsync(user);

            AuthResponse response = new AuthResponse
            {
                Id = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpiresOn = jwtToken.ValidTo,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roles.ToList(),
                IsAuthenticated = true,
                RefreshToken = newPlainToken,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn,
                Message = "Token refreshed successfully"
            };

            return Result<AuthResponse>.Success(response);
        }

        public async Task<Result<bool>> RevokeTokenAsync(string plainToken)
        {
            if (string.IsNullOrEmpty(plainToken))
            {
                return Result<bool>.Failure(new List<Error>
                {
                    new Error("Auth.TokenMissing", "Token is required to revoke.", ErrorType.Validation)
                });
            }

            var tokenHash = ComputeSha256Hash(plainToken);

            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u =>
                    u.RefreshTokens.Any(t =>
                        t.Token == tokenHash &&
                        t.RevokedOn == null &&
                        t.ExpiresOn > DateTime.UtcNow));

            if (user is null)
            {
                return Result<bool>.Failure(new List<Error>
                {
                    new Error("Auth.InvalidToken", "Invalid token", ErrorType.NotFound)
                });
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == tokenHash);

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return Result<bool>.Success(true);
        }

        public async Task<Result<string>> RegisterAsync(RegistrationRequest request, string role)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not null)
            {
                return Result<string>.Failure(new List<Error>
                {
                    new Error("Auth.EmailAlreadyRegistered",
                              "Email is already registered!",
                              ErrorType.Validation)
                });
            }

            if (await _userManager.FindByNameAsync(request.UserName) is not null)
            {
                return Result<string>.Failure(new List<Error>
                {
                    new Error("Auth.UserNameAlreadyRegistered",
                              "UserName is already taken!",
                              ErrorType.Validation)
                });
            }

            // من المستخدم من برا role ده كنت هحتاجه لو كنت هستقبل ال
            /*
            if (role == "Admin")
            {
                return Result<string>.Failure(new List<Error>
                {
                    new Error("Auth.InvalidRole", "You cannot register as Admin.", ErrorType.Validation)
                });
            }
            */

            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                Role = Enum.Parse<UserRole>(role)
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => new Error("Auth.UserCreationFailed", e.Description, ErrorType.Failure))
                    .ToList();

                return Result<string>.Failure(errors);
            }

            if (role == UserRole.Founder.ToString())
            {
                var founder = new Founder { UserId = user.Id };
                await _unitOfWork.FounderRepository.AddAsync(founder);
                var wallet = new Wallet
                {
                    FounderId = founder.Id,
                    OwnerType = WalletOwnerType.Founder
                };
                await _unitOfWork.WalletRepository.AddAsync(wallet);
            }
            else if (role == UserRole.Investor.ToString())
            {
                var investor = new Investor
                {
                    UserId = user.Id,
                };
                await _unitOfWork.InvestorRepository.AddAsync(investor);
                var wallet = new Wallet
                {
                    InvestorId = investor.Id,
                    OwnerType = WalletOwnerType.Investor
                };
                await _unitOfWork.WalletRepository.AddAsync(wallet);
            }
            await _unitOfWork.SaveAsync();
            await _userManager.AddToRoleAsync(user, role);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink =
                _urlGenerator.GenerateEmailConfirmationLink(user.Id, token);

            var email = new Email
            {
                To = user.Email,
                Subject = "Confirm your email address",
                Body = $"Please confirm your email by clicking the following link:\n{confirmationLink}\n\nIf you did not create this account, you can safely ignore this email.",
                HtmlBody = $@"
                    <div style='font-family:Arial,sans-serif; line-height:1.6;'>
                        <h2>Welcome!</h2>
                        <p>Thank you for signing up. Please confirm your email address to activate your account.</p>
                        
                        <p>
                            <a href='{confirmationLink}' 
                               style='display:inline-block; padding:10px 16px; background:#007bff; color:#fff; 
                                      text-decoration:none; border-radius:5px;'>
                                Confirm Email
                            </a>
                        </p>

                        <p style='color:#666; font-size:12px;'>
                            If you did not create this account, you can ignore this email.
                        </p>
                    </div>"
            };

            await _emailService.SendAsync(email);

            return Result<string>.Success("Registration successful. Please confirm your email.");
        }

        public async Task<Result<string>> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return Result<string>.Failure(new List<Error>
                {
                    new Error("Auth.UserNotFound", "User not found", ErrorType.NotFound)
                });
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => new Error("Auth.InvalidToken", e.Description, ErrorType.Validation))
                    .ToList();

                return Result<string>.Failure(errors);
            }

            return Result<string>.Success("Email confirmed successfully");
        }

        public async Task<Result<string>> ResendConfirmationEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Result<string>.Failure(new List<Error>
                {
                    new Error("Auth.UserNotFound", "User not found", ErrorType.NotFound)
                });
            }

            if (user.EmailConfirmed)
            {
                return Result<string>.Failure(new List<Error>
                {
                    new Error("Auth.EmailAlreadyConfirmed", "Email is already confirmed", ErrorType.Validation)
                });
            }

            if (user.EmailConfirmationRetryCount >= 5 &&
                user.LastEmailSentOn.HasValue &&
                user.LastEmailSentOn.Value.AddHours(1) > DateTime.UtcNow)
            {
                return Result<string>.Failure(new List<Error>
                {
                    new Error("Auth.EmailRetryLimitExceeded",
                              "You have exceeded the number of confirmation email requests. Please try again later.",
                              ErrorType.Validation)
                });
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = _urlGenerator.GenerateEmailConfirmationLink(user.Id, token);

            var emailMessage = new Email
            {
                To = user.Email,
                Subject = "Confirm your email",
                Body = $"Click here to confirm your email: {confirmationLink}",
                HtmlBody = $"<p>Click <a href='{confirmationLink}'>here</a> to confirm your email!</p>"
            };

            await _emailService.SendAsync(emailMessage);

            user.EmailConfirmationRetryCount++;
            user.LastEmailSentOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return Result<string>.Success("Confirmation email has been sent.");
        }

        public async Task<Result<string>> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Result<string>.Failure(new List<Error>
                {
                    new Error("Auth.UserNotFound", "User not found", ErrorType.NotFound)
                });
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Result<string>.Failure(new List<Error>
                {
                    new Error("Auth.EmailNotConfirmed", "Email is not confirmed", ErrorType.Validation)
                });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = _urlGenerator.GeneratePasswordResetLink(user.Id, token);

            var emailMessage = new Email
            {
                To = user.Email,
                Subject = "Reset your password",
                Body = $"Click here to reset your password: {resetLink}",
                HtmlBody = $"<p>Click <a href='{resetLink}'>here</a> to reset your password</p>"
            };

            await _emailService.SendAsync(emailMessage);

            return Result<string>.Success("Password reset email has been sent.");
        }

        public async Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user is null)
            {
                return Result<string>.Failure(new List<Error>
                {
                    new Error("Auth.UserNotFound", "User not found", ErrorType.NotFound)
                });
            }

            var tokenFromQuery = HttpUtility.UrlDecode(request.Token);
            var result = await _userManager.ResetPasswordAsync(user, tokenFromQuery, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => new Error("Auth.PasswordResetFailed", e.Description, ErrorType.Validation))
                    .ToList();

                return Result<string>.Failure(errors);
            }

            return Result<string>.Success("Password has been reset successfully.");
        }

        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(CustomClaimTypes.Uid, user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }

        private string GenerateRefreshToken(out RefreshToken refreshToken)
        {
            var randomNumber = new byte[32];

            RandomNumberGenerator.Fill(randomNumber);

            var plainToken = Convert.ToBase64String(randomNumber);

            using var sha256 = SHA256.Create();
            var tokenHash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(plainToken)));

            refreshToken = new RefreshToken
            {
                Token = tokenHash,
                PlainToken = plainToken,
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow,
                
            };

            return plainToken;
        }
        private string ComputeSha256Hash(string rawData)
        {
            if (string.IsNullOrEmpty(rawData))
                throw new ArgumentException("Token cannot be null or empty", nameof(rawData));

            using var sha256 = SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData)));
        }

        public async Task<Result<AuthResponse>> SignInWithGoogleAsync(string idToken, string role)
        {
            if (string.IsNullOrEmpty(idToken))
            {
                return Result<AuthResponse>.Failure(new List<Error>
                {
                    new Error("Auth.InvalidToken", "ID Token cannot be null or empty", ErrorType.Validation)
                });
            }

            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _jwtSettings.GoogleClientId }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (Exception ex)
            {
                return Result<AuthResponse>.Failure(new List<Error>
                {
                    new Error("Auth.InvalidGoogleToken", "Invalid Google ID token: " + ex.Message, ErrorType.Validation)
                });
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = payload.Email,
                    UserName = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName,
                    EmailConfirmed = true,
                    Role = Enum.Parse<UserRole>(role)
                };

                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors
                        .Select(e => new Error("Auth.UserCreationFailed", e.Description, ErrorType.Failure))
                        .ToList();

                    return Result<AuthResponse>.Failure(errors);
                }
                if (role == UserRole.Founder.ToString())
                {
                    var founder = new Founder { UserId = user.Id };
                    await _unitOfWork.FounderRepository.AddAsync(founder);
                }
                else if (role == UserRole.Investor.ToString())
                {
                    var investor = new Investor
                    {
                        UserId = user.Id,
                    };
                    await _unitOfWork.InvestorRepository.AddAsync(investor);
                }
                await _unitOfWork.SaveAsync();
                await _userManager.AddToRoleAsync(user, role);

                var email = new Email
                {
                    To = user.Email,
                    Subject = "Welcome to Investry 🚀",
                    Body = $@"
                        Welcome to Investry!
                        
                        Hi {user.UserName},
                        
                        We're excited to have you join our community.
                        
                        Investry connects visionary founders with ambitious investors.
                        You can now explore opportunities, build connections, and start growing your journey with us.
                        
                        If you have any questions, feel free to reach out to our support team.
                        
                        Let’s build the future together!
                        
                        — The Investry Team
                        ",

                    HtmlBody = $@"
                        <div style='font-family: Arial, sans-serif; padding:20px; background-color:#f4f6f9;'>
                            <div style='max-width:600px; margin:auto; background:white; padding:30px; border-radius:8px;'>
                                <h2 style='color:#2c3e50;'>Welcome to Investry 🚀</h2>
                                <p>Hi <strong>{user.UserName}</strong>,</p>
                                <p>
                                    We're excited to have you join our community.
                                </p>
                                <p>
                                    <strong>Investry</strong> connects visionary founders with ambitious investors.
                                    You can now explore opportunities, build connections, and start growing your journey with us.
                                </p>
                                <p>
                                    If you have any questions, feel free to reach out to our support team.
                                </p>
                                <hr style='margin:30px 0;'/>
                                <p style='color:gray; font-size:14px;'>
                                    Let’s build the future together!<br/>
                                    — The Investry Team
                                </p>
                            </div>
                        </div>"
                };

                await _emailService.SendAsync(email);

            }
            var jwtSecurityToken = await GenerateToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            var oldRefreshToken = user.RefreshTokens
                .FirstOrDefault(t => t.RevokedOn == null && t.ExpiresOn > DateTime.UtcNow);

            if (oldRefreshToken != null)
            {
                oldRefreshToken.RevokedOn = DateTime.UtcNow;
            }

            var plainToken = GenerateRefreshToken(out var refreshToken);
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var response = new AuthResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,
                Roles = roles.ToList(),
                IsAuthenticated = true,
                RefreshToken = plainToken,
                RefreshTokenExpiration = refreshToken.ExpiresOn,
                Message = "Google Sign-In successful"
            };

            return Result<AuthResponse>.Success(response);
        }

    }
}
