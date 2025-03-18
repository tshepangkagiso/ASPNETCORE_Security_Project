using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using WebApp_Identity.Data.Account;

namespace WebApp_Identity.Pages.Account
{
    public class RecoverPasswordModel : PageModel
    {
        [BindProperty]
        public ResetPasswordViewModel ResetPassword { get; set; } = new ResetPasswordViewModel();
        public string? SuccessMessage { get; set; }

        private readonly UserManager<User> userManager;
        public RecoverPasswordModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            this.SuccessMessage = string.Empty;

            var user = await this.userManager.FindByIdAsync(userId);

            if(user == null)
            {
                ModelState.AddModelError("", "Invalid user ID.");
                return Page();
            }

            ResetPassword.Email = user.Email ?? "";
            ResetPassword.ConfirmationToken = token;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (string.IsNullOrWhiteSpace(ResetPassword.ConfirmationToken))
            {
                ModelState.AddModelError("RecoverPassword", "Validation token is empty.");
                return Page();
            }

            if (ResetPassword.Password != ResetPassword.ConfirmPassword)
            {
                ModelState.AddModelError("RecoverPassword", "Ensure your password and confirm password are the same.");
                return Page();
            }

            var user = await userManager.FindByEmailAsync(ResetPassword.Email);
            if (user == null)
            {
                ModelState.AddModelError("RecoverPassword", "Something went wrong, contact our offices.");
                return Page();
            }

            // Verify token before resetting the password
            var isValidToken = await userManager.VerifyUserTokenAsync(
                user,
                userManager.Options.Tokens.PasswordResetTokenProvider,
                "ResetPassword",
                ResetPassword.ConfirmationToken
            );

            if (!isValidToken)
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired token.");
                return Page();
            }

            var result = await userManager.ResetPasswordAsync(user, ResetPassword.ConfirmationToken, ResetPassword.Password);
            if (result.Succeeded)
            {
                this.SuccessMessage = "Successfully reset password, try logging in.";
                return Page();
            }
            ModelState.AddModelError("RecoverPassword", "Something went wrong resetting password.");
            return Page();

        }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string ConfirmationToken {  get; set; } = string.Empty ;
    }

}
