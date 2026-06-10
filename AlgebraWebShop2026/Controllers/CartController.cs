using AlgebraWebShop2026.Data;
using AlgebraWebShop2026.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AlgebraWebShop2026.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        public const string SessionKeyName = "_cart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string message)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName) 
                ?? new List<CartItem>();

            decimal total = 0;

            foreach(var item in cart)
            {
                total += item.getTotal();
                item.Product.Images = _context.Image.Where(i => i.ProductId == item.Product.Id).ToList();
            }

            ViewBag.TotalPrice = total;
            ViewBag.Cart = message;

            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId,decimal quantity)
        {
            if (quantity <= 0) return RedirectToAction(nameof(Index), 
                new { message = "Quantity must be grater than zero!" });

            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName)
                ?? new List<CartItem>();
            string msg = "";

            if (cart.Count == 0)
            {
                var product = _context.Product.Find(productId);
                if(product==null) return RedirectToAction(nameof(Index),
                    new { message = "Can't add non existing product to cart!" });

                CartItem cartItem = new CartItem()
                {
                    Product = product,
                    Quantity = quantity
                };

                if (product.Quantity < cartItem.Quantity)
                {
                    cartItem.Quantity = product.Quantity;
                    msg = "With available quantity, ";
                }

                cart.Add(cartItem);
                msg += cartItem.Product.Title + " added to cart";
                HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);
            }



            return RedirectToAction(nameof(Index), new { message = msg });
        }

        public IActionResult RemoveFromCart(int productId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId,decimal quantity)
        {
            throw new NotImplementedException();
        }

        private int IsExistingInCart(int productId)
        {
            throw new NotImplementedException();
        }
    }
}
