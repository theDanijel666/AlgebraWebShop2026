using AlgebraWebShop2026.Data;
using AlgebraWebShop2026.Extensions;
using AlgebraWebShop2026.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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

            Order order = HttpContext.Session.GetObjectFromJson<Order>("OrderDetails") ?? new Order();

            if (_signInManager.IsSignedIn(User) && order.BillingFirstname=="")
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
            HttpContext.Session.SetObjectAsJson("OrderDetails", order);
            var modelErrors = new List<string>();

            string msg = CheckCart();
            if (msg != "OK")
            {
                msg = "Cart: " + msg;
                modelErrors.Add(msg);
            }

            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName) 
                ?? new List<CartItem>();
            if (cart.Count == 0)
            {
                return RedirectToAction(nameof(Order), new { errors = modelErrors });
            }

            ModelState.Remove("OrderItems");
            ModelState.Remove("ShippingSameAsBilling");
            if (order.Message.IsNullOrEmpty()) order.Message = "";
            ModelState.Remove("Message");

            if (_signInManager.IsSignedIn(User))
            {
                var userid = _userManager.GetUserId(User);
                order.UserId = userid;
                ModelState.Remove("UserId");
            }
            else
            {
                modelErrors.Add("User: You have to be signed in to make an order!");
                return RedirectToAction(nameof(Order), new { errors = modelErrors });
            }

            if (ShippingSameAsBilling == "on")
            {
                order.ShippingFirstname = order.BillingFirstname;
                ModelState.Remove("ShippingFirstname");
                order.ShippingLastname = order.BillingLastname;
                ModelState.Remove("ShippingLastname");
                order.ShippingEmail = order.BillingEmail;
                ModelState.Remove("ShippingEmail");
                order.ShippingPhone = order.BillingPhone;
                ModelState.Remove("ShippingPhone");
                order.ShippingAddress = order.BillingAddress;
                ModelState.Remove("ShippingAddress");
                order.ShippingCity = order.BillingCity;
                ModelState.Remove("ShippingCity");
                order.ShippingZIP = order.BillingZIP;
                ModelState.Remove("ShippingZIP");
                order.ShippingCountry = order.BillingCountry;
                ModelState.Remove("ShippingCountry");
                HttpContext.Session.SetObjectAsJson("OrderDetails", order);
            }

            if(ModelState.IsValid && modelErrors.Count == 0)
            {
                int orderNum = _context.Order.Max(o => o.OrderNumber)+1;
                order.OrderNumber= orderNum;
                order.Created= DateTime.Now;
                _context.Order.Add(order);
                _context.SaveChanges();

                int order_id = order.Id;

                foreach(var item in cart)
                {
                    OrderItem oi = new OrderItem()
                    {
                        OrderId = order_id,
                        ProductId = item.Product.Id,
                        Quantity = item.Quantity,
                        Price = item.Product.Price,
                        MesuringUnit= item.Product.MesuringUnit,
                        Discount= item.Product.Discount,
                    };

                    var prod = _context.Product.Find(oi.ProductId);
                    prod.Quantity -= oi.Quantity;
                    _context.Update(prod);

                    _context.OrderItem.Add(oi);
                    _context.SaveChanges();
                }

                HttpContext.Session.SetObjectAsJson(SessionKeyName, "");
                HttpContext.Session.SetObjectAsJson("OrderDetails", "");

                return RedirectToAction(nameof(Index), "Home", new { message = "Thank you for your order :)" });
            }
            else
            {
                foreach(var modelState in ModelState.Values)
                {
                    foreach(var error in modelState.Errors)
                    {
                        modelErrors.Add(error.ErrorMessage);
                    }
                }
            }

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

        [Authorize]
        public IActionResult MyOrders()
        {
            var user_id = _userManager.GetUserId(User);
            var orders=_context.Order.Where(o=>o.UserId == user_id);
            return View(orders);
        }

        [Authorize]
        public IActionResult MyOrderDetails(int id)
        {
            var order=_context.Order.Where(o=>o.Id == id).FirstOrDefault();
            if (order == null) return NotFound();

            var user_id=_userManager.GetUserId(User);

            if (order.UserId != user_id)
            {
                return RedirectToAction("Index", "Home", 
                    new { message = "Not allowed to view orders you did not make!" });
            }

            order.OrderItems=_context.OrderItem.Where(o=>o.OrderId==order.Id).ToList();

            return View(order);
        }
    }
}
