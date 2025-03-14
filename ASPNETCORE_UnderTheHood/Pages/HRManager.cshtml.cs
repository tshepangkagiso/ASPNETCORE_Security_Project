using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ASPNETCORE_UnderTheHood.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRMangerModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
