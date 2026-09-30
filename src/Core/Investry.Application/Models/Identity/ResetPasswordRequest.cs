using System.ComponentModel.DataAnnotations;

namespace Investry.Application.Models.Identity
{
    public record ResetPasswordRequest
    {
        public string UserId { get; set; }
        public string Token { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; }
    }

}
