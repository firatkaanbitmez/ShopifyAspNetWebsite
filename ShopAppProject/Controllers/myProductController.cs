using ShopAppProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace ShopAppProject.Controllers
{
    [Authorize(Roles = "Admin,Company")]
    public class myProductController : Controller
    {
        private readonly DataContext _context;

        public myProductController(DataContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // Check if the user has the "Admin" or "Company" role
            if (User.IsInRole("Admin") || User.IsInRole("Company"))
            {
                var products = await _context.Products.ToListAsync();
                return View(products);
            }
            else
            {
                // Redirect to the home page or another page
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Create()
        {
            ViewBag.CategoryList = GetCategorySelectList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product model)
        {
            if (!ModelState.IsValid)
            {
                // If the model state is not valid, return to the create view with errors
                ViewBag.CategoryList = GetCategorySelectList(); // Ensure ViewBag.CategoryList is set
                return View(model);
            }

            // Get the current user ID
            var userId = HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            // Set the user ID for the product
            model.UserId = userId;

            // Add category handling here
            if (string.IsNullOrEmpty(model.ProductCategory))
            {
                ModelState.AddModelError("ProductCategory", "Please select a category");
                ViewBag.CategoryList = GetCategorySelectList(); // Ensure ViewBag.CategoryList is set
                return View(model);
            }

            // Add the product to the context
            _context.Products.Add(model);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the Index action
            return RedirectToAction("Index");
        }
        public IActionResult Details(int id)
        {
            var product = _context.Products.Include(p => p.Comments).FirstOrDefault(p => p.ProductId == id);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int productId, string content)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
#pragma warning disable CS8602 // Dereference of a possibly null reference.

            var userName = HttpContext.User.Identity.Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.


            var comment = new Comment
            {
                ProductId = productId,
                UserId = userId,
                UserName = userName,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = productId });
        }

        public SelectList GetCategorySelectList()
        {
            var categories = new List<SelectListItem>
            {
                new SelectListItem { Value = "Elektronik", Text = "Elektronik" },
                new SelectListItem { Value = "Moda", Text = "Moda" },
                new SelectListItem { Value = "Ev, Yaşam, Kırtasiye, Ofis", Text = "Ev, Yaşam, Kırtasiye, Ofis" },
                new SelectListItem { Value = "Oto, Bahçe, Yapı Market", Text = "Oto, Bahçe, Yapı Market" },
                new SelectListItem { Value = "Anne, Bebek, Oyuncak", Text = "Anne, Bebek, Oyuncak" },
                new SelectListItem { Value = "Spor, Outdoor", Text = "Spor, Outdoor" },
                new SelectListItem { Value = "Kozmetik, Kişisel Bakım", Text = "Kozmetik, Kişisel Bakım" },
                new SelectListItem { Value = "Süpermarket, Pet Shop", Text = "Süpermarket, Pet Shop" },
                new SelectListItem { Value = "Kitap, Müzik, Film, Hobi", Text = "Kitap, Müzik, Film, Hobi" },
                // ... other categories ...
            };

            return new SelectList(categories, "Value", "Text");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Check if the user is an Admin
            if (!User.IsInRole("Admin") && product.UserId != userId)
            {
                return Forbid(); // If not an Admin and not the owner, deny access
            }

            ViewBag.CategoryList = GetCategorySelectList(); // Set the ViewBag for categories

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product model)
        {
            if (id != model.ProductId)
            {
                return NotFound();
            }

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Check if the user is an Admin
            if (!User.IsInRole("Admin") && product.UserId != userId)
            {
                return Forbid(); // If not an Admin and not the owner, deny access
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CategoryList = GetCategorySelectList(); // Set the ViewBag for categories
                return View(model);
            }

            // Update all properties including the category
            product.ProductTitle = model.ProductTitle;
            product.ProductDesc = model.ProductDesc;
            product.ProductStock = model.ProductStock;
            product.ProductPrice = model.ProductPrice;
            product.ProductImage = model.ProductImage;
            product.ProductCategory = model.ProductCategory; // Update the category

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Check if the user is an Admin
            if (!User.IsInRole("Admin") && product.UserId != userId)
            {
                return Forbid(); // If not an Admin and not the owner, deny access
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Check if the user is an Admin
            if (!User.IsInRole("Admin") && product.UserId != userId)
            {
                return Forbid(); // If not an Admin and not the owner, deny access
            }

            // Remove the product from the context and save changes
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // Redirect to the Index action after the product has been deleted
            return RedirectToAction("Index");
        }


    }
}
