using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.Data;
using SignalRAssignment.Models;

namespace SignalRAssignment.Pages.Members
{
    public class DetailsModel : PageModel
    {
        private readonly SignalRAssignment.Data.ApplicationDbContext _context;

        public DetailsModel(SignalRAssignment.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Customer Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            string UsName = HttpContext.Session.GetString("UsName");
            ViewData["UsName"] = UsName;
            if (UsName == null)
            {
                return RedirectToPage("/Home/Index");
            }
            else
            {
                if (HttpContext.Session.GetInt32("IsAdmin") == 0)
                {
                    return RedirectToPage("/Home/Index");
                }
                if (id == null)
                {
                    return NotFound();
                }

                Customer = await _context.Customers
                    .Include(c => c.Account).FirstOrDefaultAsync(m => m.CustomerId == id);

                if (Customer == null)
                {
                    return NotFound();
                }
                return Page();
            }
        }
    }
}
