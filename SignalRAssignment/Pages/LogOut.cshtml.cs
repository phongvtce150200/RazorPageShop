using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SignalRAssignment.Pages
{
    public class LogOutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Remove("UsName");
            Response.Cookies.Delete("UsName");

            HttpContext.Session.Remove("cart");
            Response.Cookies.Delete("cart");


            HttpContext.Session.Remove("cartObj");
            Response.Cookies.Delete("cartObj");

            HttpContext.SignOutAsync();
            return RedirectToPage("/Home/Index");
        }
    }
}
