
using AlgebraWebShop2026.Data;
using AlgebraWebShop2026.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[Area("Admin")]
[Authorize(Roles ="Admin")]
public class ProductCategoryController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductCategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: PRODUCTCATEGORYS
    public async Task<IActionResult> Index(int productId)    
    {
        if (productId <= 0) return RedirectToAction("Index", "Product");
        var product=_context.Product.Where(p=>p.Id == productId).FirstOrDefault();
        if(product == null) return RedirectToAction("Index", "Product");

        var list=_context.ProductCategory.Where(p=>p.ProductId== productId).ToList();
        foreach(var item in list)
        {
            item.ProductTitle = _context.Product.Where(p => p.Id == item.ProductId).First().Title;
            item.CategoryTitle=_context.Category.Where(c=>c.Id==item.CategoryId).First().Title;
        }

        ViewBag.ProductId = productId;

        return View(list);
    }

    // GET: PRODUCTCATEGORYS/Details/5
    //public async Task<IActionResult> Details(int? id)
    //{
    //    if (id == null)
    //    {
    //        return NotFound();
    //    }

    //    var productcategory = await _context.ProductCategory
    //        .FirstOrDefaultAsync(m => m.Id == id);
    //    if (productcategory == null)
    //    {
    //        return NotFound();
    //    }

    //    return View(productcategory);
    //}

    // GET: PRODUCTCATEGORYS/Create
    public IActionResult Create(int productId)
    {
        if (productId <= 0) return RedirectToAction("Index", "Product");
        var product = _context.Product.Where(p => p.Id == productId).FirstOrDefault();
        if (product == null)
        {
            return NotFound();
        }


        var addedcategories=_context.ProductCategory.Where(pc=>pc.ProductId== productId).ToList().Select(
            pc=> pc.CategoryId
        ).ToList();

        var categories = _context.Category.Where(c=>!addedcategories.Contains(c.Id)).Select(
            cat=> new SelectListItem
            {
                Value=cat.Id.ToString(),
                Text=cat.Title
            }).ToList();

        categories.InsertRange(0, new SelectListItem { Value = "0", Text = "Odaberite kategoriju za dodijeliti!" });

        ViewBag.Categories = categories;
        ViewBag.ProductId=productId;

        return View();
    }

    // POST: PRODUCTCATEGORYS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,CategoryId,ProductId,ProductTitle,CategoryTitle")] ProductCategory productcategory)
    {
        ModelState.Remove("ProductTitle");
        ModelState.Remove("CategoryTitle");
        if (ModelState.IsValid)
        {
            _context.Add(productcategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index),new {productId= productcategory.ProductId});
        }

        var addedcategories = _context.ProductCategory.Where(pc => pc.ProductId == productcategory.ProductId).ToList().Select(
            pc => pc.CategoryId
        ).ToList();

        var categories = _context.Category.Where(c => !addedcategories.Contains(c.Id)).Select(
            cat => new SelectListItem
            {
                Value = cat.Id.ToString(),
                Text = cat.Title
            }).ToList();

        categories.InsertRange(0, new SelectListItem { Value = "0", Text = "Odaberite kategoriju za dodijeliti!" });

        ViewBag.Categories = categories;
        ViewBag.ProductId = productcategory.ProductId;

        return View(productcategory);
    }

    // GET: PRODUCTCATEGORYS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var productcategory = await _context.ProductCategory.FindAsync(id);
        if (productcategory == null)
        {
            return NotFound();
        }

        var addedcategories = _context.ProductCategory.Where(pc => pc.ProductId == productcategory.ProductId).ToList().Select(
            pc => pc.CategoryId
        ).ToList();
        addedcategories.Remove(productcategory.CategoryId);

        var categories = _context.Category.Where(c => !addedcategories.Contains(c.Id)).Select(
            cat => new SelectListItem
            {
                Value = cat.Id.ToString(),
                Text = cat.Title
            }).ToList();

        categories.InsertRange(0, new SelectListItem { Value = "0", Text = "Odaberite kategoriju za dodijeliti!" });

        ViewBag.Categories = categories;
        ViewBag.ProductId = productcategory.ProductId;

        return View(productcategory);
    }

    // POST: PRODUCTCATEGORYS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,CategoryId,ProductId,ProductTitle,CategoryTitle")] ProductCategory productcategory)
    {
        if (id != productcategory.Id)
        {
            return NotFound();
        }

        ModelState.Remove("ProductTitle");
        ModelState.Remove("CategoryTitle");
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(productcategory);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductCategoryExists(productcategory.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index), new {productId=productcategory.ProductId});
        }

        var addedcategories = _context.ProductCategory.Where(pc => pc.ProductId == productcategory.ProductId).ToList().Select(
            pc => pc.CategoryId
        ).ToList();
        addedcategories.Remove(productcategory.CategoryId);

        var categories = _context.Category.Where(c => !addedcategories.Contains(c.Id)).Select(
            cat => new SelectListItem
            {
                Value = cat.Id.ToString(),
                Text = cat.Title
            }).ToList();

        categories.InsertRange(0, new SelectListItem { Value = "0", Text = "Odaberite kategoriju za dodijeliti!" });

        ViewBag.Categories = categories;
        ViewBag.ProductId = productcategory.ProductId;

        return View(productcategory);
    }

    // GET: PRODUCTCATEGORYS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var productcategory = await _context.ProductCategory
            .FirstOrDefaultAsync(m => m.Id == id);
        if (productcategory == null)
        {
            return NotFound();
        }

        productcategory.ProductTitle = _context.Product.Find(productcategory.ProductId).Title;
        productcategory.CategoryTitle = _context.Category.Find(productcategory.CategoryId).Title;

        return View(productcategory);
    }

    // POST: PRODUCTCATEGORYS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var productcategory = await _context.ProductCategory.FindAsync(id);
        if (productcategory != null)
        {
            _context.ProductCategory.Remove(productcategory);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index),new {productId=productcategory.ProductId});
    }

    private bool ProductCategoryExists(int? id)
    {
        return _context.ProductCategory.Any(e => e.Id == id);
    }
}
