
using AlgebraWebShop2026.Data;
using AlgebraWebShop2026.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

[Area("Admin")]
[Authorize(Roles ="Admin")]
public class ImageController : Controller
{
    private readonly ApplicationDbContext _context;

    public ImageController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: IMAGES
    public async Task<IActionResult> Index(int productId)    
    {
        if (productId <= 0) return RedirectToAction("Index", "Product");
        var product = _context.Product.Where(p => p.Id == productId).FirstOrDefault();
        if (product == null) return RedirectToAction("Index", "Product");
        
        var images = await _context.Image.Where(i=>i.ProductId==productId).ToListAsync();

        ViewBag.ProductId=productId;

        return View(images);
    }

    // GET: IMAGES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var image = await _context.Image
            .FirstOrDefaultAsync(m => m.Id == id);
        if (image == null)
        {
            return NotFound();
        }

        return View(image);
    }

    // GET: IMAGES/Create
    public IActionResult Create(int productId)
    {
        if (productId <= 0) return RedirectToAction("Index", "Product");
        var product = _context.Product.Where(p => p.Id == productId).FirstOrDefault();
        if (product == null) return RedirectToAction("Index", "Product");

        var img=new Image { ProductId= productId };

        return View(img);
    }

    // POST: IMAGES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,IsMain,Title,Description,URL,ProductId")] Image image, IFormFile file)
    {
        if(file==null || file.Length==0)
        {
            ModelState.AddModelError("ProductId", "You must upload a file!");
        }
        ModelState.Remove("URL");
        if (ModelState.IsValid)
        {
            string ext=System.IO.Path.GetExtension(file.FileName).ToLower();
            if(ext!=".jpg" && ext!=".jpeg" && ext != ".png")
            {
                ModelState.AddModelError("ProductId", "Unknown file format, please use jpeg, jpeg or png.");
                return View(image);
            }

            _context.Add(image);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(image);
    }

    // GET: IMAGES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var image = await _context.Image.FindAsync(id);
        if (image == null)
        {
            return NotFound();
        }
        return View(image);
    }

    // POST: IMAGES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,IsMain,Title,Description,URL,ProductId")] Image image)
    {
        if (id != image.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(image);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageExists(image.Id))
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
        return View(image);
    }

    // GET: IMAGES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var image = await _context.Image
            .FirstOrDefaultAsync(m => m.Id == id);
        if (image == null)
        {
            return NotFound();
        }

        return View(image);
    }

    // POST: IMAGES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var image = await _context.Image.FindAsync(id);
        if (image != null)
        {
            _context.Image.Remove(image);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ImageExists(int? id)
    {
        return _context.Image.Any(e => e.Id == id);
    }
}
