using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRAssignment.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SignalRAssignment.Pages.Login
{
    public class IndexModel : PageModel
    {
        private readonly SignalRAssignment.Data.ApplicationDbContext _context;

        public IndexModel(SignalRAssignment.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        [Required(ErrorMessage = "Can't be null")]
        public string USName { get; set; }
        [BindProperty]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Can't be null")]
        public string Pwd { get; set; }


        public string Message;

        public IActionResult OnPost()
        {
            IQueryable<Account> AccountIQ = from s in _context.Accounts select s;

            if (ModelState.IsValid)
            {
                var check = AccountIQ.Where(x => x.UserName == USName && x.Password == Pwd).FirstOrDefault();
                if (check == null)
                {
                    ViewData["AlertMessage"] = "Wrong user name or password please try again!!";
                    return Page();
                }
                else 
                {
                    HttpContext.Session.SetString("UsName", USName);
                    return RedirectToPage("/Home/Index");
                }
            }

            return Page();
        }
    }
}
