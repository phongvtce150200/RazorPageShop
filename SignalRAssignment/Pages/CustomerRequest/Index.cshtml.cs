using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SignalRAssignment.Data;
using SignalRAssignment.Models;

namespace SignalRAssignment.Pages.CustomerRequest
{
    public class IndexModel : PageModel
    {
        private readonly SignalRAssignment.Data.ApplicationDbContext _context;

        public IndexModel(SignalRAssignment.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            string UsName = HttpContext.Session.GetString("UsName");
            if (UsName == null)
            {
                return RedirectToPage("/Home/Index");
            }
            ViewData["UsName"] = UsName;
            ViewData["AccountId"] = new SelectList(_context.Accounts, "AccountId", "FullName");
            return Page();
        }

        [BindProperty]
        [Required(ErrorMessage = "Cant be null")]
        public string ContactName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Cant be null")]
        public string Address { get; set; }

        [BindProperty]
        [Phone]
        [Required(ErrorMessage = "Cant be null")]
        public string Phone { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {

            int? UsID = HttpContext.Session.GetInt32("UsID");

            IQueryable<Customer> CustomerIQ = from s in _context.Customers select s;

            var checkInfo = CustomerIQ.Where(x => x.AccountId == UsID);

            if (checkInfo == null)
            {
                return Page();
            }

            if (ModelState.IsValid)
            {
                Customer cus = new Customer()
                {
                    CustomerId = (int)UsID,
                    ContactName = ContactName,
                    Address = Address,
                    Phone = Phone,
                    AccountId = UsID
                };
                _context.Customers.Add(cus);
                await _context.SaveChangesAsync();
            }


            return RedirectToPage("/Home/Index");
        }
    }
}
