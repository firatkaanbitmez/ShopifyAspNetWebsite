using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppProject.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace ShopAppProject.Controllers
{
    public class DealsController : Controller
    {
        private readonly DataContext _context;

        public DealsController(DataContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var deals = await _context.Deals.ToListAsync();
            return View(deals);
        }

        // GET: Deals/Create
        public IActionResult Create()
        {
            ViewBag.DealsType = new SelectList(Enum.GetValues(typeof(DealsType)));
            ViewBag.ProductCategories = new SelectList(_context.Products.Select(p => p.ProductCategory).Distinct());

            return View();
        }


        // POST: Deals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DealsId,DealsType,DealsName,DealsDescription,StartDate,EndDate,DPercentage,DPercentageMinimumPrice,DPrice,DPriceMinimumPrice,DCouponCode,DCodeMinimumPrice,FSMinimumPrice,GProductId,CDiscountPercentage,CDMinimumPrice,Point")] Deals deals)
        {
            if (ModelState.IsValid)
            {
                _context.Add(deals);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DealsType = new SelectList(Enum.GetValues(typeof(DealsType)), deals.DealsType);
            return View(deals);
        }



        // GET: Deals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var deal = await _context.Deals.FindAsync(id);
            if (deal == null) return NotFound();

            ViewBag.DealsType = new SelectList(Enum.GetValues(typeof(DealsType)), deal.DealsType);
            ViewBag.ProductCategories = new SelectList(_context.Products.Select(p => p.ProductCategory).Distinct(), deal.ProductCategory);

            return View(deal);
        }

        // POST: Deals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DealsId,DealsType,DealsName,DealsDescription,StartDate,EndDate,DPercentage,DPercentageMinimumPrice,DPrice,DPriceMinimumPrice,DCouponCode,DCodeMinimumPrice,FSMinimumPrice,GProductId,CDiscountPercentage,CDMinimumPrice,Point,ProductCategory")] Deals deal)
        {
            if (id != deal.DealsId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DealExists(deal.DealsId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.DealsType = new SelectList(Enum.GetValues(typeof(DealsType)), deal.DealsType);
            ViewBag.ProductCategories = new SelectList(_context.Products.Select(p => p.ProductCategory).Distinct(), deal.ProductCategory);
            return View(deal);
        }

        private bool DealExists(int id)
        {
            return _context.Deals.Any(e => e.DealsId == id);
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
            if (deal == null)
            {
                return NotFound();
            }

            _context.Deals.Remove(deal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Deals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var deal = await _context.Deals.FirstOrDefaultAsync(m => m.DealsId == id);
            if (deal == null) return NotFound();

            return View(deal);
        }


    }
}
