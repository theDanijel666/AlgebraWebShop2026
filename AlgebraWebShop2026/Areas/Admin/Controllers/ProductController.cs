
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlgebraWebShop2026.Models;
using AlgebraWebShop2026.Data;
using Microsoft.AspNetCore.Authorization;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: PRODUCTS
    public async Task<IActionResult> Index()    
    {
        var products = _context.Product.
            Include(p => p.Images).
            Include(p => p.ProductCategories).
            ToList();
        foreach (var product in products) 
        {
            foreach(var category in product.ProductCategories)
            {
                category.CategoryTitle = _context.Category.Find(category.CategoryId).Title;
            }
        }
        return View(products);
    }

    // GET: PRODUCTS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Product
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        product.Images = await _context.Image.Where(i => i.ProductId == id).ToListAsync();
        product.ProductCategories = await _context.ProductCategory.Where(pc => pc.ProductId == id).ToListAsync();
        foreach (var category in product.ProductCategories)
        {
            category.CategoryTitle = _context.Category.Where(c => c.Id == category.CategoryId).First().Title;
        }

        return View(product);
    }

    // GET: PRODUCTS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: PRODUCTS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Description,Quantity,Price,MesuringUnit,Discount,ProductCategories,Images,OrderItems")] Product product)
    {
        ModelState.Remove("ProductCategories");
        ModelState.Remove("Images");
        ModelState.Remove("OrderItems");
        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: PRODUCTS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Product.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        product.Images = await _context.Image.Where(i => i.ProductId == id).ToListAsync();
        product.ProductCategories = await _context.ProductCategory.Where(pc => pc.ProductId == id).ToListAsync();
        foreach (var category in product.ProductCategories)
        {
            category.CategoryTitle = _context.Category.Where(c => c.Id == category.CategoryId).First().Title;
        }

        return View(product);
    }

    // POST: PRODUCTS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Description,Quantity,Price,MesuringUnit,Discount,ProductCategories,Images,OrderItems")] Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        ModelState.Remove("OrderItems");
        ModelState.Remove("Images");
        ModelState.Remove("ProductCategories");

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    // GET: PRODUCTS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Product
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        product.Images = await _context.Image.Where(i => i.ProductId == id).ToListAsync();
        product.ProductCategories = await _context.ProductCategory.Where(pc => pc.ProductId == id).ToListAsync();
        foreach (var category in product.ProductCategories)
        {
            category.CategoryTitle = _context.Category.Where(c => c.Id == category.CategoryId).First().Title;
        }

        return View(product);
    }

    // POST: PRODUCTS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var product = await _context.Product.FindAsync(id);
        List<string> files = new List<string>();
        if (product != null)
        {
            product.Images = await _context.Image.Where(i => i.ProductId == id).ToListAsync();
            foreach(var image in product.Images)
            {
                files.Add(image.URL);
            }
            _context.Product.Remove(product);
        }

        await _context.SaveChangesAsync();

        foreach (var file in files)
        {
            string todelete = file.Remove(0, 1).Replace("/", "\\\\");
            todelete = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", todelete);
            if(System.IO.File.Exists(todelete)) System.IO.File.Delete(todelete);
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ProductExists(int? id)
    {
        return _context.Product.Any(e => e.Id == id);
    }
}
