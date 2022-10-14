using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.Data;
using SignalRAssignment.Models;

namespace SignalRAssignment.Pages.Products
{
    public class EditModel : PageModel
    {

        [Obsolete]
        private IHostingEnvironment _environment;
        private readonly SignalRAssignment.Data.ApplicationDbContext _context;

        [Obsolete]
        public EditModel(SignalRAssignment.Data.ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Product Product { get; set; }


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

                Product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier).FirstOrDefaultAsync(m => m.ProductId == id);

                if (Product == null)
                {
                    return NotFound();
                }
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CatagoryName");
                ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierId");
                return Page();
            }
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        [Obsolete]
        public async Task<IActionResult> OnPostAsync(IFormFile image)
        {
            _context.Attach(Product).State = EntityState.Modified;

            if (ModelState.IsValid)
            {
                try
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

                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(Product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            return RedirectToPage("/Products/Index");
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}


