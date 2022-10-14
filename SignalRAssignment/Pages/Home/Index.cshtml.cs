using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRAssignment.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly SignalRAssignment.Data.ApplicationDbContext _context;

        public IndexModel(SignalRAssignment.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Product> Products { get; set; }
        public async Task<IActionResult> OnGetAsync(string keyword)
        {
            string UsName = HttpContext.Session.GetString("UsName");

            ViewData["UsName"] = UsName;

            Products = await _context.Products.ToListAsync();
            if(keyword != null)
            {
                var productsName = from s in _context.Products select s;

                if (!string.IsNullOrEmpty(keyword))
                {
                    productsName = productsName.Where(s => s.ProductName.Contains(keyword));

                }
                Products = await productsName.ToListAsync();
            }

            if (UsName == null)
            {
                return Page();
            }
            else
            {
                IQueryable<Account> AccountIQ = from s in _context.Accounts select s;
                var user = AccountIQ.Where(x => x.UserName == UsName).FirstOrDefault();
                HttpContext.Session.SetInt32("UsID", user.AccountId);
                bool checkRole = user.Type;
                if (checkRole)
                {
                    HttpContext.Session.SetInt32("IsAdmin", 1);
                    ViewData["IsAdmin"] = true;
                    //ViewData["AlertMessage"] = "Hello admin";
                    return RedirectToPage("/Products/Index");
                }
                else
                {
                    HttpContext.Session.SetInt32("IsAdmin", 0);
                    ViewData["IsAdmin"] = false;
                   // ViewData["AlertMessage"] = "Hello " + UsName;
                    return Page();
                }
            }

           
        }
    }
}
