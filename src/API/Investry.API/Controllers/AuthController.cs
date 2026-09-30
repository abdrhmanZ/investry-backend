using Investry.API.Common;
using Investry.API.Common.Extensions;
using Investry.Application.Features.Authentication.Commands.ConfirmEmail;
using Investry.Application.Features.Authentication.Commands.ForgotPassword;
using Investry.Application.Features.Authentication.Commands.FounderGoogleSignIn;
using Investry.Application.Features.Authentication.Commands.InvestorGoogleSignIn;
using Investry.Application.Features.Authentication.Commands.Login;
using Investry.Application.Features.Authentication.Commands.Logout;
using Investry.Application.Features.Authentication.Commands.RefreshToken;
using Investry.Application.Features.Authentication.Commands.RegisterFounder;
using Investry.Application.Features.Authentication.Commands.ResendConfirmationEmail;
using Investry.Application.Features.Authentication.Commands.ResetPassword;
using Investry.Application.Features.Authentication.Commands.RevokeToken;
using Investry.Application.Models.Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Investry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Register a new founder account.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/register-founder
        ///     {
        ///        "firstName": "Abd Alrahman",
        ///        "lastName": "Okeil",
        ///        "email": "user@example.com",
        ///        "phoneNumber": "1234567890",
        ///        "userName": "AbdAlrahman",
        ///        "password": "P@ssword123",
        ///        "confirmPassword": "P@ssword123"
        ///      }
        /// </remarks>
        /// <response code="200">Registration successful</response>
        /// <response code="400">Validation error</response>    
        /// <response code="409">Email already exists</response>
        [HttpPost("register-founder")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterFounder([FromBody] RegisterFounderCommand command)
            => (await _mediator.Send(command)).ToApiResponse();

        /// <summary>
        /// Register a new investor account.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/register-investor
        ///     {
        ///        "firstName": "Abd Alrahman",
        ///        "lastName": "Okeil",
        ///        "email": "user@example.com",
        ///        "phoneNumber": "1234567890",
        ///        "userName": "AbdAlrahman",
        ///        "password": "P@ssword123",
        ///        "confirmPassword": "P@ssword123"
        ///      }
        /// </remarks>
        /// <response code="200">Registration successful</response>
        /// <response code="400">Validation error</response>
        /// <response code="409">Email already exists</response>
        [HttpPost("register-investor")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterInvestor([FromBody] RegisterInvestorCommand command)
            => (await _mediator.Send(command)).ToApiResponse();

        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegistrationRequest request, string role)
        //{
        //    var result = await _mediator.Send(new RegisterCommand(request, role));
        //    if (!result.IsSuccess) return BadRequest(result.Error);
        //    return Ok(result.Value);
        //}

        /// <summary>
        /// Sign in or register a founder using Google OAuth
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/auth/signin-google-founder
        ///     {
        ///         "idToken": ""
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Sign-In successful, returns AuthResponse</response>
        /// <response code="400">Invalid ID Token or validation failed</response>
        /// <response code="409">User creation failed</response>
        [HttpPost("signin-google-founder")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> GoogleSignInFounder([FromBody] FounderGoogleSignInCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);

            return result.ToApiResponse();
        }

        /// <summary>
        /// Sign in or register a investor using Google OAuth
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/auth/signin-google-investor
        ///     {
        ///         "idToken": ""
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Sign-In successful, returns AuthResponse</response>
        /// <response code="400">Invalid ID Token or validation failed</response>
        /// <response code="409">User creation failed</response>
        [HttpPost("signin-google-investor")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object?>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> GoogleSignInInvestor([FromBody] InvestorGoogleSignInCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);

            return result.ToApiResponse();
        }

        //[HttpPost("signin-google")]
        //public async Task<IActionResult> SignIn([FromBody] GoogleSignInRequest request)
        //{
        //    var result = await _mediator.Send(new GoogleSignInCommand (request.IdToken, request.Role));
        //    if (!result.IsSuccess) return BadRequest(result.Error);
        //    return Ok(result.Value);
        //}

        /// <summary>
        /// Login user using email and password
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/auth/login
        ///     {
        ///         "email": "user@example.com",
        ///         "password": "P@ssword123"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Login successful</response>
        /// <response code="401">Invalid email or password</response>
        /// <response code="400">Email not confirmed</response>
        [HttpPost("login")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);
             //Console.WriteLine("Refresh Token: " + result.Value.RefreshToken);
            return result.ToApiResponse();
        }


        /// <summary>
        /// Refresh JWT using refresh token stored in cookie
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/auth/refresh-token
        ///
        /// </remarks>
        /// <response code="200">Token refreshed successfully</response>
        /// <response code="404">Refresh token missing</response>
        /// <response code="400">Invalid or expired refresh token</response>
        [HttpPost("refresh-token")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await _mediator.Send(new RefreshTokenCommand(refreshToken));

            if (result.IsSuccess)
                SetRefreshTokenInCookie(result.Value.RefreshToken, result.Value.RefreshTokenExpiration);

            return result.ToApiResponse();
        }

        /// <summary>
        /// Confirm user's email using userId and token from query parameters
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /api/auth/confirm-email?userId=123token=
        ///
        /// </remarks>
        /// <response code="200">Token refreshed successfully</response>
        /// <response code="400">Bad request</response>
        /// <response code="404">User not found</response>
        [HttpGet("confirm-email")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
            => (await _mediator.Send(new ConfirmEmailCommand(userId, token))).ToApiResponse();

        /// <summary>
        /// Resend confirmation email to the user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/auth/resend-confirmation-email
        ///     {
        ///         "email": "user@example.com"
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Confirmation email sent successfully</response>
        /// <response code="400">Email already confirmed or retry limit exceeded</response>
        /// <response code="404">User not found</response>
        [HttpPost("resend-confirmation-email")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailCommand command)
            => (await _mediator.Send(command)).ToApiResponse();

        /// <summary>
        /// Logout the user by revoking their refresh token
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/auth/logout
        ///
        /// </remarks>
        /// <response code="200">Logout successful</response>
        /// <response code="400">Refresh token missing or invalid</response>
        /// <response code="404">User not found</response>
        [HttpPost("logout")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout()
        {
            var result = await _mediator.Send(new LogoutCommand(Request.Cookies["refreshToken"]));

            if (result.IsSuccess)
                Response.Cookies.Delete("refreshToken");

            return result.ToApiResponse();
        }

        /// <summary>
        /// Revoke a refresh token manually
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/revoke-token
        ///     {
        ///         "token": "hgfdsjhfsd4h5j43hj34h5jk4h5..."
        ///     }
        /// </remarks>
        /// <response code="200">Token revoked successfully</response>
        /// <response code="400">Token missing in the request</response>
        /// <response code="404">Token invalid or not found</response>
        [HttpPost("revoke-token")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenCommand command)
            => (await _mediator.Send(command)).ToApiResponse();

        /// <summary>
        /// Sends a password reset email to the user
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/forgot-password
        ///     {
        ///         "email": "user@example.com"
        ///     }
        /// </remarks>
        /// <response code="200">Password reset email sent successfully</response>
        /// <response code="400">User email is not confirmed</response>
        /// <response code="404">User not found</response>
        [HttpPost("forgot-password")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
            => (await _mediator.Send(command)).ToApiResponse();

        /// <summary>
        /// Reset user password using the token sent via email
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/auth/reset-password
        ///     {
        ///         "userId": "jg54jthj4h43jk5hj4k3h54h5jhhjk34hj...",
        ///         "token": "jh5kj435h34g534g5jh34gh5ghhgrghghth...",
        ///         "newPassword": "NewP@ssword123",
        ///         "confirmPassword": "NewP@ssword123"
        ///     }
        /// </remarks>
        /// <response code="200">Password reset successfully</response>
        /// <response code="400">Password reset failed (invalid token or password rules)</response>
        /// <response code="404">User not found</response>
        [HttpPost("reset-password")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
            => (await _mediator.Send(command)).ToApiResponse();

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                SameSite = SameSiteMode.None,
                Secure = true
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
