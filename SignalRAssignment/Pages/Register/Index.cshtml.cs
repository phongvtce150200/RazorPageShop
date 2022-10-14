using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SignalRAssignment.Data;
using SignalRAssignment.Models;

namespace SignalRAssignment.Pages.Regitser
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
            return Page();
        }

        [BindProperty]
        [Required(ErrorMessage = "Cant be null")]
        [Display(Name = "User Name")]
        public string username { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Cant be null")]
        [Display(Name = "Password")]
        public string password { get; set; }

        [BindProperty]
        [Compare(nameof(password), ErrorMessage = "Confirm password must equal with password")]
        [Required(ErrorMessage = "Cant be null")]
        [Display(Name = "Confirm Password")]
        public string confirmPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Cant be null")]
        [Display(Name = "Full Name")]
        public string fullname { get; set; }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                if (password.Equals(confirmPassword))
                {
                    Account account = new Account()
                    {
                        UserName = username,
                        Password = password,
                        FullName = fullname,
                        Type = false
                    };
                    await _context.AddAsync(account);
                    await _context.SaveChangesAsync();
                    return RedirectToPage("/Login/Index");
                }
            }
            return Page();
        }
    }
}
