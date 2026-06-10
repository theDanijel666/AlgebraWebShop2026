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

            var product = _context.Product.Find(productId);
            if (product == null) return RedirectToAction(nameof(Index),
                new { message = "Can't add non existing product to cart!" });

            if (cart.Count == 0)
            {
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
            }
            else
            {
                int product_index = IsExistingInCart(productId);
                if (product_index < 0)
                {
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
                }
                else
                {
                    cart[product_index].Quantity += quantity;
                    msg = "Quantity updated.";
                    if (product.Quantity < cart[product_index].Quantity)
                    {
                        cart[product_index].Quantity = product.Quantity;
                        msg = "Quantity set to available quantity!";
                    }
                }
            }

            HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);

            return RedirectToAction(nameof(Index), new { message = msg });
        }

        public IActionResult RemoveFromCart(int productId)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName)
                ?? new List<CartItem>();
            int product_index=IsExistingInCart(productId);

            if(product_index>=0) cart.RemoveAt(product_index);

            HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId,decimal quantity)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName);
            int product_index = IsExistingInCart(productId);
            if(product_index<0) return RedirectToAction(nameof(Index),new {message="Updated product not in cart!"});
            string msg = "";

            if (quantity < 0)
            {
                msg = "Quantity can't be negative!";
            }
            else
            {
                decimal available_quanity = _context.Product.Find(productId).Quantity;
                if (available_quanity < quantity){
                    msg = "Quantity set to available quantity. ";
                    quantity = available_quanity;
                }
                cart[product_index].Quantity = quantity;
                if (quantity == 0)
                {
                    cart.RemoveAt(product_index);
                    msg += "Quantity is 0, so product is removed from cart.";
                }
                HttpContext.Session.SetObjectAsJson(SessionKeyName, cart);
            }

            return RedirectToAction(nameof(Index),new {message=msg});
        }

        private int IsExistingInCart(int productId)
        {
            List<CartItem> cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(SessionKeyName);
            for(int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id == productId) return i;
            }

            return -1;
        }
    }
}
