
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlgebraWebShop2026.Models;
using AlgebraWebShop2026.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Rendering;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OrderItemsController : Controller
{
    private readonly ApplicationDbContext _context;

    public OrderItemsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: ORDERITEMS
    public async Task<IActionResult> Index(int orderid)
    {
        var order=_context.Order.Where(o=>o.Id == orderid).FirstOrDefault();
        if (order == null) return NotFound();

        var items=await _context.OrderItem.Where(oi=>oi.OrderId== orderid).ToListAsync();

        decimal total = 0;
        foreach(var item in items) 
        {
            item.ProductTitle = _context.Product.Find(item.ProductId).Title;
            total += (item.Quantity * item.Price * (1 - item.Discount / 100));
        }
        ViewBag.Total = total;
        ViewBag.OrderId=orderid;

        return View(items);
    }

    // GET: ORDERITEMS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var orderitem = await _context.OrderItem
            .FirstOrDefaultAsync(m => m.Id == id);
        if (orderitem == null)
        {
            return NotFound();
        }

        orderitem.ProductTitle = _context.Product.Find(orderitem.ProductId).Title;

        return View(orderitem);
    }

    // GET: ORDERITEMS/Create
    public IActionResult Create(int orderid)
    {
        var order = _context.Order.Where(o => o.Id == orderid).FirstOrDefault();
        if (order == null) return NotFound();

        OrderItem item=new OrderItem();
        item.OrderId= orderid;
        item.Price = 0;
        item.Discount = 0;
        item.Quantity = 1;
        item.MesuringUnit = "KOM";

        var products = new SelectList(_context.Product, "Id", "Title");
        ViewBag.Products = products;

        return View(item);
    }

    // POST: ORDERITEMS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,OrderId,ProductId,Quantity,Price,MesuringUnit,Discount,ProductTitle")] OrderItem orderitem)
    {
        ModelState.Remove("ProductTitle");
        if (ModelState.IsValid)
        {
            var product = _context.Product.Find(orderitem.ProductId);
            orderitem.Price= product.Price;
            orderitem.Discount=product.Discount;
            orderitem.MesuringUnit=product.MesuringUnit;
            _context.Add(orderitem);
            product.Quantity -= orderitem.Quantity;
            _context.Update(product);
            await _context.SaveChangesAsync();
            UpdateOrderTotal(orderitem.OrderId);
            return RedirectToAction(nameof(Edit), new { id = orderitem.Id});
        }
        return View(orderitem);
    }

    // GET: ORDERITEMS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var orderitem = await _context.OrderItem.FindAsync(id);
        if (orderitem == null)
        {
            return NotFound();
        }

        var product=await _context.Product.FindAsync(orderitem.ProductId);
        orderitem.ProductTitle = product.Title;

        ViewBag.QuantityMessage = "Awailable quantity: " + product.Quantity;

        return View(orderitem);
    }

    // POST: ORDERITEMS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,OrderId,ProductId,Quantity,Price,MesuringUnit,Discount,ProductTitle")] OrderItem orderitem)
    {
        if (id != orderitem.Id)
        {
            return NotFound();
        }

        var product = await _context.Product.FindAsync(orderitem.ProductId);

        ModelState.Remove("ProductTitle");

        if (ModelState.IsValid)
        {
            try
            {
                var oldOrderItem = await _context.OrderItem.FindAsync(id);
                var oldquantity = oldOrderItem.Quantity;

                oldOrderItem.Price = orderitem.Price;
                oldOrderItem.Discount = orderitem.Discount;
                oldOrderItem.Quantity = orderitem.Quantity;
                oldOrderItem.MesuringUnit = orderitem.MesuringUnit;

                _context.Update(oldOrderItem);

                product.Quantity -= (orderitem.Quantity - oldquantity);
                _context.Update(product);
                //_context.Update(orderitem);
                await _context.SaveChangesAsync();
                UpdateOrderTotal(oldOrderItem.OrderId);

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderItemExists(orderitem.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index),new {orderid=orderitem.OrderId});
        }

        orderitem.ProductTitle = product.Title;
        ViewBag.QuantityMessage = "Available quantity: " + product.Quantity;
        return View(orderitem);
    }

    // GET: ORDERITEMS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var orderitem = await _context.OrderItem
            .FirstOrDefaultAsync(m => m.Id == id);
        if (orderitem == null)
        {
            return NotFound();
        }

        orderitem.ProductTitle = _context.Product.Find(orderitem.ProductId).Title;

        return View(orderitem);
    }

    // POST: ORDERITEMS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var orderitem = await _context.OrderItem.FindAsync(id);
        if (orderitem != null)
        {
            _context.OrderItem.Remove(orderitem);
            var product= await _context.Product.FindAsync(orderitem.ProductId);
            product.Quantity += orderitem.Quantity;
            _context.Update(product);
        }
        else
        {
            return RedirectToAction(nameof(Index), "Order");
        }

        int orderid = orderitem.OrderId;

        await _context.SaveChangesAsync();
        UpdateOrderTotal(orderid);
        return RedirectToAction(nameof(Index),new {orderid=orderid});
    }

    private bool OrderItemExists(int? id)
    {
        return _context.OrderItem.Any(e => e.Id == id);
    }

    private void UpdateOrderTotal(int orderid)
    {
        var order = _context.Order.Find(orderid);
        if (order == null) return;
        var orderitems=_context.OrderItem.Where(oi=>oi.OrderId==orderid).ToList();
        decimal total= 0;
        foreach(var item in orderitems)
        {
            total += (item.Price * item.Quantity * (1 - item.Discount / 100));
        }
        order.Total = total;
        _context.Update(order);
        _context.SaveChanges();
    }
}
