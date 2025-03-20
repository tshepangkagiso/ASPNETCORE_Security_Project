using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using System.ComponentModel.DataAnnotations;
using WebApp_Identity.Data.Account;

namespace WebApp_Identity.Pages.Account
{
    [Authorize]
    public class AuthenticatorWithMFASetupModel : PageModel
    {
        [BindProperty]
        public SetupMFAViewModel SetupMFAViewModel { get; set; } = new();

        [BindProperty]
        public bool IsSucceeded { get; set; }

        private readonly UserManager<User> userManager;

        public AuthenticatorWithMFASetupModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.IsSucceeded = false;
        }

        public async Task OnGetAsync()
        {
            var user = await this.userManager.GetUserAsync(base.User);
            if (user != null)
            {
                var key = await this.userManager.GetAuthenticatorKeyAsync(user);

                if (string.IsNullOrEmpty(key))
                {
                    await this.userManager.ResetAuthenticatorKeyAsync(user);
                    key = await this.userManager.GetAuthenticatorKeyAsync(user);
                }

                this.SetupMFAViewModel.Key = key ?? string.Empty;
                this.SetupMFAViewModel.QRCodeBytes = GenerateQRCodeBytes("My Web App", key??"", user.Email ?? string.Empty);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = await this.userManager.GetUserAsync(base.User);
            if (user == null)
            {
                ModelState.AddModelError("MFA", "User not found.");
                return Page();
            }

            var result = await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, this.SetupMFAViewModel.SecurityCode ?? "");
            if (result)
            {
                await userManager.SetTwoFactorEnabledAsync(user, true);
                IsSucceeded = true;
            }
            else
            {
                ModelState.AddModelError("MFA", "Invalid verification code. Please try again.");
                return Page();
            }

            return Page();
        }

        private byte[] GenerateQRCodeBytes(string provider, string key, string userEmail)
        {
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerator.CreateQrCode($"otpauth://totp/{provider}:{userEmail}?secret={key}&issuer={provider}", QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }


    public class SetupMFAViewModel
    {
        public string? Key { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string? SecurityCode { get; set; } = string.Empty;

        public byte[]? QRCodeBytes { get; set; }

    }
}
