using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SignalRAssignment.Data;
using SignalRAssignment.Models;

namespace SignalRAssignment.Pages.Products
{
    public class CreateModel : PageModel
    {
        [Obsolete]
        private IHostingEnvironment _environment;

        private readonly SignalRAssignment.Data.ApplicationDbContext _context;

        [Obsolete]
        public CreateModel(SignalRAssignment.Data.ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult OnGet()
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
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CatagoryName");
                ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierId");
                return Page();
            }
        }

        [BindProperty]
        public Product Product { get; set; }



        [Obsolete]
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(IFormFile image)
        {
            if (image != null)
            {
                try
                {
                    string ext = Path.GetExtension(image.FileName);
                    if (ext == ".jpg" || ext == ".png" || ext == "jpeg" || ext == ".gif")
                    {
                        string Name = $"{Product.ProductName}_{image.FileName}";

                        string AbsoluteSaveDirectory = $"/Medias/{Product.ProductName}/";

                        string AbsoluteSaveFullPath = Directory.GetCurrentDirectory().Replace("\\", "/") + "/wwwroot" + AbsoluteSaveDirectory + Name;

                        //var AbsoluteSaveFullPath = Path.Combine(_environment.WebRootPath, AbsoluteSaveDirectory, image.FileName);

                        if (Directory.Exists(Path.GetDirectoryName(AbsoluteSaveFullPath)) == false)
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(AbsoluteSaveFullPath));
                        }

                        using (FileStream stream = new FileStream(AbsoluteSaveFullPath, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }

                        Product.ProductImage = $"{AbsoluteSaveDirectory}{Name}";
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
            else
            {
                Product.ProductImage = null;
            }

            _context.Products.Add(Product);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
