using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppProject.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopAppProject.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly DataContext _context;

        public CartController(DataContext context)
        {
            _context = context;
        }

        private string GetUserId()
        {
            return HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private async Task<int> AddToCartAsync(int productId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return -1; // Kullanıcı kimliği yoksa işlem yapılamaz.
            }

            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                {
                    return -1; // Ürün bulunamadı.
                }

                // CartItem ekleme işlemleri

                await _context.SaveChangesAsync();
                return await _context.CartItems.CountAsync(c => c.UserId == userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to cart: {ex.Message}");
                return -1;
            }
        }

        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return Content("0");
            }

            var cartItemCount = _context.CartItems.Count(c => c.UserId == userId);
            return Content(cartItemCount.ToString());
        }

        private Dictionary<string, decimal> CalculateTotalAmount(string userId)
        {
            var totalAmounts = new Dictionary<string, decimal>();

            var cartItems = _context.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .ThenInclude(p => p.User) // Sadece gerekli veriyi dahil edin
                .ToList();

            foreach (var cartItem in cartItems)
            {
                if (cartItem.Product?.User != null)
                {
                    var sellerId = cartItem.Product.UserId;

                    if (!totalAmounts.ContainsKey(sellerId))
                    {
                        totalAmounts[sellerId] = 0;
                    }

                    totalAmounts[sellerId] += (decimal)cartItem.Quantity * cartItem.Product.ProductPrice;
                }
            }

            return totalAmounts;
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Index");
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int decreaseQuantity, int increaseQuantity)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                var cartItem = await _context.CartItems
                    .Include(c => c.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

                if (cartItem != null)
                {
                    if (decreaseQuantity > 0)
                    {
                        cartItem.Quantity = Math.Max(0, decreaseQuantity);
                    }
                    else if (increaseQuantity > 0 && cartItem.Product != null)
                    {
                        if (cartItem.Quantity + 1 <= cartItem.Product.ProductStock)
                        {
                            cartItem.Quantity++;
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Üzgünüz, stok yetersiz.";
                            return RedirectToAction("Index");
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating quantity: {ex.Message}");
                return View("Error");
            }
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
            ViewBag.WalletBalance = wallet?.Balance ?? 0;

            var cartItems = await _context.CartItems
                                    .Where(c => c.UserId == userId)
                                    .Include(c => c.Product)
                                        .ThenInclude(p => p.Images) // Eğer ürün resimleri de gerekliyse bu satırı ekleyin
                                    .ToListAsync();

            var totalAmounts = CalculateTotalAmount(userId);
            var hasInactiveProducts = cartItems.Any(ci => ci.Product != null && !ci.Product.IsActive);

            var cartModel = new CartViewModel
            {
                CartItems = cartItems,
                TotalAmounts = totalAmounts,
                HasInactiveProducts = hasInactiveProducts,
                TotalAmount = totalAmounts.Values.Sum()
            };

            return View(cartModel);
        }
    }
}
