using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using SignalRAssignment.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRAssignment.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly SignalRAssignment.Data.ApplicationDbContext _context;

        public IndexModel(SignalRAssignment.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        
        public List<Item> cart;
        public IActionResult OnGetBuyNow(int id)
        {
            string Usname = HttpContext.Session.GetString("UsName");
            IQueryable<Account> AccountIQ = from s in _context.Accounts select s;
            var AccountObj = AccountIQ.FirstOrDefault(x => x.UserName == Usname);
            ViewData["UsName"] = Usname;
            IQueryable<Customer> CustomerIQ = from s in _context.Customers select s;
            //check chua dang nhap thi bat dang nhap
            if (Usname == null)
            {
                ViewData["LoginWarning"] = "Login to buy";
                return RedirectToPage("/Login/Index");
            }
            //dang nhap xong thi check chua co info thi yeu cau dk info de mua hang
            var checkInfo = CustomerIQ.FirstOrDefault(x => x.AccountId == AccountObj.AccountId);
            if (checkInfo == null)
            {
                return RedirectToPage("/CustomerRequest/Index");
            }
            IQueryable<Product> ProductIQ = from s in _context.Products select s;
            cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            if (cart == null)
            {
                cart = new List<Item>();
                cart.Add(new Item
                {
                    Product = ProductIQ.FirstOrDefault(x => x.ProductId == id),
                    Quantity = 1
                });
                SessionHelper.SetObjectASJson(HttpContext.Session, "cart", cart);
                string cartObj = JsonConvert.SerializeObject(cart);
                HttpContext.Session.SetString("cartObj", cartObj);
            }
            else
            {
                int index = Exits(cart, id);
                if (index == -1)
                {
                    cart.Add(new Item
                    {
                        Product = ProductIQ.FirstOrDefault(x => x.ProductId == id),
                        Quantity = 1
                    });
                }
                else
                {
                    cart[index].Quantity++;
                }
                SessionHelper.SetObjectASJson(HttpContext.Session, "cart", cart);
                string cartObj = JsonConvert.SerializeObject(cart);
                HttpContext.Session.SetString("cartObj", cartObj);
            }
            return Page();
        }
        public void OnGetDelete(int id)
        {
            cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = Exits(cart, id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectASJson(HttpContext.Session, "cart", cart);
        }
        private int Exits(List<Item> cart, int id)
        {
            for (var i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.ProductId == id)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
