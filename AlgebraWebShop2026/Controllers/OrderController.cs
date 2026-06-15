using AlgebraWebShop2026.Data;
using AlgebraWebShop2026.Extensions;
using AlgebraWebShop2026.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AlgebraWebShop2026.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public const string SessionKeyName = "_cart";

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Order(List<string> errors)
        {
            if (errors == null) errors = new List<string>();
            string msg = CheckCart();
            if (msg != "OK")
            {
                msg = "Cart: " + msg;
                errors.Add(msg);
            }

            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName)
                ?? new List<CartItem>();
            if (cart.Count == 0)
            {
                ViewBag.OrderButton = "disabled=\"disabled\"";
            }

            decimal total = 0;
            foreach(var item in cart)
            {
                item.Product.Images=_context.Image.Where(i=>i.ProductId==item.Product.Id).ToList();
                total += item.getTotal();
            }

            ViewBag.TotalPrice= total;

            ViewBag.Errors = errors;

            Order order = new Order();

            if (_signInManager.IsSignedIn(User))
            {
                var userid = _userManager.GetUserId(User);
                order.UserId = userid;
                var user=await _userManager.GetUserAsync(User);
                order.BillingFirstname = user.Ime;
                order.BillingLastname = user.Prezime;
                order.BillingEmail = user.Email;
                order.BillingPhone = user.PhoneNumber;
                order.BillingAddress = user.Adresa;
                order.BillingCity = user.Grad;
                order.BillingCountry = user.Drzava;
                order.BillingZIP = user.PB;
            }

            ViewBag.Order = order;

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrder([Bind("Total, BillingFirstname,BillingLastname," +
            "BillingEmail, BillingPhone, BillingAddress, BillingCity, BillingZIP, BillingCountry," +
            "ShippingFirstname, ShippingLastname, ShippingEmail, ShippingPhone, ShippingAddress," +
            "ShippingCity, ShippingZIP, ShippingCountry, Message")] Order order, string ShippingSameAsBilling)
        {
            var modelErrors = new List<string>();



            ViewBag.Order = order;
            return RedirectToAction(nameof(Order), new { errors = modelErrors });
        }

        private string CheckCart()
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName)
                ?? new List<CartItem>();
            if (cart.Count == 0) return "Cart is empty!";

            string message = "";
            for(int i = 0; i < cart.Count; i++)
            {
                var item = _context.Product.Find(cart[i].Product.Id);
                if (item.Quantity < cart[i].Quantity)
                {
                    cart[i].Quantity = item.Quantity;
                    message += " Cart item " + item.Title + " quantiti set to available quantity!";
                }

                if (cart[i].Quantity == 0)
                {
                    message += " The item " + item.Title + " was removed from cart because no stock available.";
                    cart.RemoveAt(i);
                    i--;
                }
            }

            return "OK";
        }
    }
}
