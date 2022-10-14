using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using SignalRAssignment.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalRAssignment.Pages.Payment
{
    public class IndexModel : PageModel
    {

        private readonly SignalRAssignment.Data.ApplicationDbContext _context;

        public IndexModel(SignalRAssignment.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        public Order Order { get; set; }


        public void OnGet()
        {
            //lay thong tin tu bang account sang bang customer
            string Usname = HttpContext.Session.GetString("UsName");
            IQueryable<Account> AccountIQ = from s in _context.Accounts select s;
            var AccountObj = AccountIQ.FirstOrDefault(x => x.UserName == Usname);
            ViewData["UsName"] = Usname;
            IQueryable<Customer> CustomerIQ = from s in _context.Customers select s;
            var checkInfo = CustomerIQ.FirstOrDefault(x => x.AccountId == AccountObj.AccountId);
           
            //Deserialize Cart
            string getCartObj = HttpContext.Session.GetString("cartObj");
            List<Item> items = JsonConvert.DeserializeObject<List<Item>>(getCartObj);
            
            Order order = new Order()
            {
                //orderID tu tang
                CustomerId = checkInfo.CustomerId,
                OrderDate = DateTime.Now,
                RequiredDate = DateTime.Now + TimeSpan.FromDays(7),
                ShippedDate = DateTime.Now + TimeSpan.FromDays(1),
                Freight = 20000,
                ShipAddress = checkInfo.Address

            };
            order.OrderDetails = new List<OrderDetail>();
            foreach (var item in items)
            {
                OrderDetail orderDetail = new OrderDetail()
                { 
                    ProductId = item.Product.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.UnitPrice
                };

                order.OrderDetails.Add(orderDetail);
            };
            
            _context.Orders.Add(order);
            _context.SaveChangesAsync();
           
            HttpContext.Session.Remove("cart");
            Response.Cookies.Delete("cart");


            HttpContext.Session.Remove("cartObj");
            Response.Cookies.Delete("cartObj");

        }

    }
}
