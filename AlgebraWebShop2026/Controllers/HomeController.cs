using AlgebraWebShop2026.Data;
using AlgebraWebShop2026.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AlgebraWebShop2026.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Product(int? categoryId, string? sort, decimal? priceFrom,decimal? priceTo, int? per_page,int? page)
        {
            List<Product> products = _context.Product.Include(p => p.Images).Include(p => p.ProductCategories).ToList();

            if (categoryId != null)
            {
                products = products.Where(p => p.ProductCategories.Any(c => c.CategoryId == categoryId)).ToList();
            }

            ViewBag.Categories = _context.Category.ToList();

            if (priceFrom != null)
            {
                products = products.Where(p => p.Price >= priceFrom).ToList();
            }

            if (priceTo != null)
            {
                products = products.Where(p => p.Price <= priceTo).ToList();
            }

            if(sort != null)
            {
                if (sort == "Price High to Low") products=products.OrderByDescending(p=>p.Price).ToList();
                if (sort == "Price Low to High") products = products.OrderBy(p => p.Price).ToList();
                if (sort == "Name A to Z") products = products.OrderBy(p => p.Title).ToList();
                if (sort == "Name Z to A") products = products.OrderByDescending(p => p.Title).ToList();
            }

            if (per_page == null) per_page = 100;
            if (page == null) page = 1;
            ViewBag.NumberOfPages = (int)Math.Ceiling((decimal)products.Count/(int)per_page);
            products = products.Skip((int)((page - 1) * per_page)).Take((int)per_page).ToList();

            return View(products);
        }

        public IActionResult SingleProduct(int id)
        {
            var product=_context.Product.Find(id);
            if(product==null) return NotFound();
            
            product.Images=_context.Image.Where(i=>i.ProductId == id).ToList();
            product.ProductCategories = _context.ProductCategory.Where(p => p.ProductId == id).ToList();
            foreach(var category in product.ProductCategories)
            {
                category.CategoryTitle = _context.Category.Find(category.CategoryId).Title;
            }

            return View(product);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
