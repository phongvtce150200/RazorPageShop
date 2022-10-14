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

namespace SignalRAssignment.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly SignalRAssignment.Data.ApplicationDbContext _context;

        public IndexModel(SignalRAssignment.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Product> Product { get; set; }

        public async Task<IActionResult> OnGetAsync(string keyword)
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
                Product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier).ToListAsync();
                if (keyword == null)
                {
                    return Page();
                }
                else
                {
                    var productsName = from s in _context.Products select s;

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        productsName = productsName.Where(s => s.ProductName.Contains(keyword));

                    }

                    Product = await productsName.ToListAsync();
                }
                return Page();
            }
        }
    }
}
