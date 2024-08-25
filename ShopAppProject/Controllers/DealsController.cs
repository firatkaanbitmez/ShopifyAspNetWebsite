using Microsoft.AspNetCore.Mvc;
using ShopAppProject.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

//test
//test


namespace ShopAppProject.Controllers
{
    public class DealsController : Controller
    {
        private readonly DataContext _context;

        public DealsController(DataContext context)
        {
            _context = context;
        }

        // GET: Deals
        public async Task<IActionResult> Index()
        {
            return View(await _context.Deals.ToListAsync());
        }

        // GET: Deals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deal = await _context.Deals
                .FirstOrDefaultAsync(m => m.DealsId == id);
            if (deal == null)
            {
                return NotFound();
            }

            return View(deal);
        }

        // GET: Deals/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Deals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DealsId,DealsType,DealsName,DealsDescription,StartDate,EndDate,minimumsepettutari,DPercentage,DPrice,xProduct,yProduct,DCouponCode,ShippingId,Point,GProductId")] Deals deal)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ = _context.Add(deal);
                    _ = await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    // Log the exception details
                    // Return to the view with an error message
                }
            }
            return View(deal);
        }

        // GET: Deals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var deal = await _context.Deals.FindAsync(id);
            if (deal == null) return NotFound();

            ViewBag.DealsType = new SelectList(Enum.GetValues(typeof(DealsType)), deal.DealsType);

            return View(deal);
        }

        // POST: Deals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DealsId,DealsType,DealsName,DealsDescription,StartDate,EndDate,minimumsepettutari,DPercentage,DPrice,xProduct,yProduct,DCouponCode,ShippingId,Point,GProductId")] Deals deal)
        {
            if (id != deal.DealsId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _ = _context.Update(deal);
                    _ = await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DealExists(deal.DealsId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DealsType = new SelectList(Enum.GetValues(typeof(DealsType)), deal.DealsType);
            return View(deal);
        }


        // GET: Deals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deal = await _context.Deals
                .FirstOrDefaultAsync(m => m.DealsId == id);
            if (deal == null)
            {
                return NotFound();
            }

            return View(deal);
        }

        // POST: Deals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deal = await _context.Deals.FindAsync(id);
#pragma warning disable CS8604 // Possible null reference argument.
            _ = _context.Deals.Remove(deal);
#pragma warning restore CS8604 // Possible null reference argument.
            _ = await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DealExists(int id)
        {
            return _context.Deals.Any(e => e.DealsId == id);
        }
    }
}
