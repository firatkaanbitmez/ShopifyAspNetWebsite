using ShopAppProject.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ShopAppProject.Models;

namespace ShopAppProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }

        // Herkesin görebileceği bir şekilde ürün listesini getir
        [AllowAnonymous]
        public async Task<IActionResult> Index(string category, string query)
        {
            IQueryable<Product> products = _context.Products.Include(p => p.Images);

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.ProductCategory == category);
            }

            if (!string.IsNullOrEmpty(query))
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                products = products.Where(p =>
                    p.ProductTitle.IndexOf(query.ToUpper(), StringComparison.OrdinalIgnoreCase) >= 0 ||
                    p.ProductDesc.IndexOf(query.ToUpper(), StringComparison.OrdinalIgnoreCase) >= 0 ||
                    p.ProductCategory.IndexOf(query.ToUpper(), StringComparison.OrdinalIgnoreCase) >= 0
                );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            ViewBag.Category = category;
            return View(await products.ToListAsync());
        }
        public IActionResult AddToList(int id)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (!User.Identity.IsAuthenticated)
            {


                return RedirectToAction("Login", "Account");
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.


            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the product is already in the user's list
            var existingEntry = _context.UserProductLists
                .FirstOrDefault(entry => entry.UserId == userId && entry.ProductId == id);

            if (existingEntry == null)
            {
                // Create a new entry in the user's list
#pragma warning disable CS8601 // Possible null reference assignment.
                var user = _context.Users.Find(userId);
                var userProductListEntry = new UserProductList
                {
                    UserId = userId,
                    ProductId = id,
                    User = user // Set the User property
                };

#pragma warning restore CS8601 // Possible null reference assignment.

                _ = _context.UserProductLists.Add(userProductListEntry);
                _ = _context.SaveChanges();
            }

            // Redirect back to the product details page
            return RedirectToAction("Details", new { id });
        }
        public IActionResult RemoveFromList(int id)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userProduct = _context.UserProductLists
                .FirstOrDefault(upl => upl.UserId == userId && upl.ProductId == id);

            if (userProduct != null)
            {
                try
                {
                    _ = _context.UserProductLists.Remove(userProduct);
                    _ = _context.SaveChanges();
                    return RedirectToAction("Details", new { id });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error removing from list: {ex.Message}");
                    // Handle the error, log it, or return an error view
                    // You can also redirect to the same details page without removing the item
                    return RedirectToAction("Details", new { id });
                }
            }

            // Redirect back to the product details page if the item is not in the list
            return RedirectToAction("Details", new { id });
        }


        public IActionResult GetRandomProducts()
        {
            // Fetch all products from the database
            var allProducts = _context.Products.Include(p => p.Images).ToList();

            // Shuffle the products using a random seed
            var random = new Random();
            var shuffledProducts = allProducts.OrderBy(p => random.Next()).Take(4).ToList();

            return PartialView("_RandomProductsPartial", shuffledProducts);
        }


        [AllowAnonymous]
        public IActionResult GetSellerInfo(string userId)
        {
            var seller = _context.Users.Find(userId);

            if (seller != null)
            {
                return PartialView("_SellerInfoPartial", seller);
            }

            return NotFound();
        }

        public IActionResult Details(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var product = _context.Products
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.UserProductLists)
                .Include(p => p.Images)
                .FirstOrDefault(p => p.ProductId == id);

                var randomProducts = _context.Products
                .AsEnumerable()
                .OrderBy(p => Guid.NewGuid())
                .Take(5)
                .ToList();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var userHasPurchased = _context.OrderDetails
                .Any(od => od.ProductId == id && od.Order.UserId == userId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                ViewBag.ProductTitle = product.ProductTitle;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                ViewBag.Category = product.ProductCategory;
                ViewBag.UserHasPurchased = userHasPurchased;
                ViewData["randomProducts"] = randomProducts;
                return View(product);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception: {ex.Message}");
                throw; // Rethrow the exception to ensure it's logged in the application logs
            }
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

            _ = _context.Comments.Add(comment);
            _ = await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = productId });
        }

        public IActionResult Search(string query)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var products = _context.Products
            .Include(p => p.Images)

                .AsEnumerable()  // Explicitly switch to client-side evaluation
                .Where(p => string.IsNullOrEmpty(query) ||
                            p.ProductTitle.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            p.ProductDesc.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                            p.ProductCategory.Contains(query, StringComparison.OrdinalIgnoreCase));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            ViewBag.SearchQuery = query;

            return View("Index", products.ToList());
        }

        public IActionResult Order(int id)
        {
            int cartItemCount = AddToCart(id);

            if (cartItemCount != -1)
            {
                // Product added to cart successfully
                TempData["CartItemCount"] = cartItemCount;
            }
            else
            {
                // Failed to add product to cart

            }

            return RedirectToAction("Index");
        }
        private void UpdateCartItemCountInSession()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItemCount = _context.CartItems.Count(c => c.UserId == userId);

            // Store the cart item count in session
            HttpContext.Session.SetInt32("CartItemCount", cartItemCount);
        }

        private int AddToCart(int productId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var existingCartItem = _context.CartItems
                    .FirstOrDefault(c => c.ProductId == productId && c.UserId == userId);

                if (existingCartItem != null)
                {
                    existingCartItem.Quantity += 1;
                }
                else
                {
                    var newCartItem = new CartItem
                    {
                        ProductId = productId,
                        UserId = userId,
                        Quantity = 1
                    };

                    _ = _context.CartItems.Add(newCartItem);
                }

                _ = _context.SaveChanges();


                return _context.CartItems.Count(c => c.UserId == userId);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error adding to cart: {ex.Message}");
                return -1;
            }
        }


    }
}
