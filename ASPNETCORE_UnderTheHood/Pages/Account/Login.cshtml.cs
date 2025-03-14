using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ASPNETCORE_UnderTheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; } = new Credential();
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // This should check if the model is NOT valid
            if (!ModelState.IsValid) return Page();

            if (Credential.Username == "admin" && Credential.Password == "password")
            {
                //creating security context
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@mywebsite.com")
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

                // Sign in with cookie authentication
                await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                // Redirect to home page after successful login
                return RedirectToPage("/Index");
            }
            else
            {
                // Add error message for invalid login
                ModelState.AddModelError("", "Invalid username or password");

                // If credentials don't match, return to login page
                return Page();
            }
        }
    }

    public class Credential
    {
        [Required]
        [Display(Name = "User name")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

    }
}
