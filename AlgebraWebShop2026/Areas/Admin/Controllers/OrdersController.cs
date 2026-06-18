
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlgebraWebShop2026.Models;
using AlgebraWebShop2026.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrdersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrdersController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: ORDERS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Order.ToListAsync());
    }

    // GET: ORDERS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Order
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        order.OrderItems = await _context.OrderItem.Where(oi=>oi.OrderId== id).ToListAsync();
        foreach (var item in order.OrderItems) 
        {
            item.ProductTitle = _context.Product.Find(item.ProductId).Title;
        }

        return View(order);
    }

    // GET: ORDERS/Create
    public IActionResult Create()
    {
        Order order = new Order();
        order.Created= DateTime.Now;
        order.Total = 0;

        ViewBag.Users = new SelectList(_userManager.Users, "Id", "UserName");

        return View(order);
    }

    // POST: ORDERS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,UserId,Message,OrderNumber,Created,Total,BillingFirstname,BillingLastname,BillingEmail,BillingPhone,BillingAddress,BillingCity,BillingZIP,BillingCountry,ShippingFirstname,ShippingLastname,ShippingEmail,ShippingPhone,ShippingAddress,ShippingCity,ShippingZIP,ShippingCountry,OrderItems")] 
        Order order, string ShippingSameAsBilling)
    {
        ModelState.Remove("ShippingSameAsBilling");
        ModelState.Remove("OrderItems");

        if (order.Message.IsNullOrEmpty()) order.Message = "";
        ModelState.Remove("Message");

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
            order.ShippingCountry = order.BillingCountry;
            ModelState.Remove("ShippingCountry");
            order.ShippingZIP = order.BillingZIP;
            ModelState.Remove("ShippingZIP");
        }

        if (ModelState.IsValid)
        {
            order.OrderNumber = _context.Order.Max(o => o.OrderNumber) + 1;
            _context.Add(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index),"OrderItems",new { orderid=order.Id });
        }

        ViewBag.Users = new SelectList(_userManager.Users, "Id", "UserName");
        return View(order);
    }

    // GET: ORDERS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Order.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        order.OrderItems = await _context.OrderItem.Where(oi => oi.OrderId == id).ToListAsync();
        foreach(var item in order.OrderItems)
        {
            item.ProductTitle = _context.Product.Find(item.ProductId).Title;
        }

        ViewBag.Users = new SelectList(_userManager.Users, "Id", "UserName");

        return View(order);
    }

    // POST: ORDERS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,UserId,Message,OrderNumber,Created,Total,BillingFirstname,BillingLastname,BillingEmail,BillingPhone,BillingAddress,BillingCity,BillingZIP,BillingCountry,ShippingFirstname,ShippingLastname,ShippingEmail,ShippingPhone,ShippingAddress,ShippingCity,ShippingZIP,ShippingCountry,OrderItems")] Order order)
    {
        if (id != order.Id)
        {
            return NotFound();
        }

        if (String.IsNullOrEmpty(order.Message))
        {
            ModelState.Remove("Message");
            order.Message = string.Empty;
        }

        ModelState.Remove("OrderItems");

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(order.Id))
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


        order.OrderItems = await _context.OrderItem.Where(oi => oi.OrderId == id).ToListAsync();
        foreach (var item in order.OrderItems)
        {
            item.ProductTitle = _context.Product.Find(item.ProductId).Title;
        }

        ViewBag.Users = new SelectList(_userManager.Users, "Id", "UserName");

        return View(order);
    }

    // GET: ORDERS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.Order
            .FirstOrDefaultAsync(m => m.Id == id);
        if (order == null)
        {
            return NotFound();
        }

        order.OrderItems = await _context.OrderItem.Where(oi => oi.OrderId == id).ToListAsync();
        foreach (var item in order.OrderItems)
        {
            item.ProductTitle = _context.Product.Find(item.ProductId).Title;
        }

        return View(order);
    }

    // POST: ORDERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var order = await _context.Order.FindAsync(id);
        if (order != null)
        {
            var orderitems = _context.OrderItem.Where(oi => oi.OrderId == order.Id).ToList();
            foreach(var item in orderitems) 
            { 
                var prod=_context.Product.Find(item.ProductId);
                prod.Quantity += item.Quantity;
                _context.Update(prod);
            }
            _context.Order.Remove(order);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool OrderExists(int? id)
    {
        return _context.Order.Any(e => e.Id == id);
    }
}
