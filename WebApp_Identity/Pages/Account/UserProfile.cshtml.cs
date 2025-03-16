using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using WebApp_Identity.Data.Account;

namespace WebApp_Identity.Pages.Account
{
    [Authorize]
    public class UserProfileModel : PageModel
    {
        [BindProperty]
        public UserProfileViewModel userProfile { get; set; }

        //By default all non-nullable fields are required fields and on form it will show this field is required.
        [BindProperty]
        public string? SuccessMessage { get; set; }

        private readonly UserManager<User> userManager;
        public UserProfileModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
            this.userProfile = new UserProfileViewModel();
        }
        public async Task<IActionResult> OnGetAsync()
        {
            this.SuccessMessage = string.Empty;

            var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();

            if (user != null)
            {
                this.userProfile.Email = User.Identity?.Name ?? string.Empty;
                this.userProfile.Department = departmentClaim?.Value ?? string.Empty;
                this.userProfile.Position = positionClaim?.Value ?? string.Empty;
            }

            return Page();
        }

        //Using an Post method instead of a Put method because Identity has a replace claim method instead of update.
        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid) return Page();

            try
            {
                var (user, departmentClaim, positionClaim) = await GetUserInfoAsync();

                if (user != null && departmentClaim != null)
                {
                    await this.userManager.ReplaceClaimAsync(user, departmentClaim, new Claim(departmentClaim.Type, userProfile.Department));
                }

                if (user != null && positionClaim != null)
                {
                    await this.userManager.ReplaceClaimAsync(user, positionClaim, new Claim(positionClaim.Type, userProfile.Position));
                }
            }
            catch
            {
                ModelState.AddModelError("UserProfile", "Error occured during updating user profile");
            }

            this.SuccessMessage = "User profile is updated successfully.";
            return Page();
        }


        // method that returns a tuple
        private async Task<(User? user, Claim? departmentClaim, Claim? positionClaim)> GetUserInfoAsync()
        {
            var user = await userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);

            if (user != null)
            {
                var claims = await userManager.GetClaimsAsync(user);
                var departmentClaim = claims.FirstOrDefault(x => x.Type == "Department");
                var positionClaim = claims.FirstOrDefault(x => x.Type == "Position");

                return (user, departmentClaim, positionClaim);
            }
            else
            {
                return (null, null, null);
            }
        }
    }

    public class UserProfileViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Department { get; set; } = string.Empty;
        [Required]
        public string Position { get; set; } = string.Empty;
    }
}
