// Controllers/ShippingController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppProject.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//test

namespace ShopAppProject.Controllers
{
    public class ShippingController : Controller
    {
        private readonly DataContext _context;

        public ShippingController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var shippingOptions = await _context.Shippings.ToListAsync();
            return View(shippingOptions);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShippingMethodName,ShippingCost")] Shipping shipping)
        {
            if (ModelState.IsValid)
            {
                _ = _context.Add(shipping);
                _ = await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(shipping);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipping = await _context.Shippings.FindAsync(id);
            if (shipping == null)
            {
                return NotFound();
            }

            return View(shipping);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShippingId,ShippingMethodName,ShippingCost")] Shipping shipping)
        {
            if (id != shipping.ShippingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _ = _context.Update(shipping);
                    _ = await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShippingExists(shipping.ShippingId))
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
            return View(shipping);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shipping = await _context.Shippings.FirstOrDefaultAsync(m => m.ShippingId == id);
            if (shipping == null)
            {
                return NotFound();
            }

            return View(shipping);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shipping = await _context.Shippings.FindAsync(id);
#pragma warning disable CS8604 // Possible null reference argument.
            _ = _context.Shippings.Remove(shipping);
#pragma warning restore CS8604 // Possible null reference argument.
            _ = await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShippingExists(int id)
        {
            return _context.Shippings.Any(e => e.ShippingId == id);
        }
    }
}
