// Controllers/OrderController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopAppProject.Data;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ShopAppProject.Models; // Assuming Order class is in the Models namespace


namespace ShopAppProject.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly DataContext _context;

        public OrderController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null || order.UserId != HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (cartItems == null || cartItems.Count == 0)
            {
                // Sepet boşsa, Cart sayfasına yönlendir
                return RedirectToAction("Index", "Cart");
            }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (cartItems.Any(c => !c.Product.IsActive))
            {
                // Handle the case where one or more products are not active
                TempData["ErrorMessage"] = "Sepetinizdeki bazı ürünler artık mevcut değil.";
                return RedirectToAction("Index", "Cart");
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var groupedCartItems = cartItems.GroupBy(c => c.Product.UserId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var defaultAddress = await _context.Addresses
                 .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);

            if (defaultAddress == null)
            {
                // Handle case where no default address is set
                TempData["ErrorMessage"] = "Lütfen Hesabım Sayfasından Varsayılan Adresinizi Ayarlayın!";
                return RedirectToAction("Index", "Cart");
            }
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                foreach (var sellerGroup in groupedCartItems)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    var order = new Order
                    {
                        UserId = userId,
                        TotalAmount = sellerGroup.Sum(c => c.Quantity * c.Product.ProductPrice),
                        OrderDetails = sellerGroup.Select(c => new OrderDetail
                        {
                            ProductId = c.ProductId,
                            Quantity = c.Quantity,
                            UnitPrice = c.Product.ProductPrice
                        }).ToList()
                    };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                    // Var olan Order örneğini takip etmeyi bırak
                    _context.Entry(order).State = EntityState.Detached;

                    _ = _context.Orders.Add(order);
                    _ = await _context.SaveChangesAsync();

                    var buyer = await _context.Users.FindAsync(userId);

                    // Cüzdan kontrolü
                    var wallet = await _context.Wallets
                        .Include(w => w.Transactions)
                        .FirstOrDefaultAsync(w => w.UserId == userId);

                    if (wallet != null)
                    {
                        // Cüzdan bakiyesi yeterli mi kontrol et
                        if (wallet.Balance < order.TotalAmount)
                        {
                            // Yeterli bakiye yoksa işlemi gerçekleştirme ve hata mesajı gönder
                            transaction.Rollback();
                            TempData["ErrorMessage"] = "Insufficient funds in the wallet.";
                            return RedirectToAction("Index", "Cart");
                        }

                        // Yeterli bakiye varsa, cüzdan bakiyesinden düş ve işlemi kaydet
                        wallet.Balance -= order.TotalAmount;
                        wallet.Transactions.Add(new Transaction
                        {
                            Date = DateTime.Now,
                            Amount = -order.TotalAmount,
                            Info = "Sipariş Ödeme Ücreti Sipariş No: " + order.OrderId,
                            OrderIdLink = order.OrderId.ToString()
                        });

                        _ = await _context.SaveChangesAsync();
                    }
                    bool soldOperationPerformed = false;


                    foreach (var orderDetail in order.OrderDetails)
                    {
                        var sellerId = orderDetail.Product?.UserId;

                        if (sellerId != null)
                        {
                            var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.ProductId == orderDetail.ProductId);

                            // Check if the product exists and has sufficient stock
                            if (product != null && product.ProductStock >= orderDetail.Quantity)
                            {
                                // Update the product stock
                                product.ProductStock -= orderDetail.Quantity;
                                _ = await _context.SaveChangesAsync();
                            }
                            else
                            {
                                // Rollback transaction and show error message
                                transaction.Rollback();
                                TempData["ErrorMessage"] = "Insufficient stock for product.";
                                return RedirectToAction("Index", "Cart");
                            }

                            if (!soldOperationPerformed)
                            {
                                var sold = new Sold
                                {
                                    OrderId = order.OrderId,
                                    SellerId = sellerId,
                                    BuyerId = userId,
                                    BuyerName = defaultAddress.AdSoyad,
                                    BuyerPhoneNumber = defaultAddress.mobilephone,
                                    BuyerAddress = defaultAddress.Street + "   " + defaultAddress.DetayliAdres,
                                    BuyerCountry = defaultAddress.State + " /" + defaultAddress.City + " /" + defaultAddress.Country,
                                    BuyerZipcode = defaultAddress.ZipCode

                                };

                                // Var olan Sold örneğini takip etmeyi bırak
                                _context.Entry(sold).State = EntityState.Detached;

                                _ = _context.Solds.Add(sold);


                                sold.SoldDate = DateTime.Now;
                                sold.SoldId = sold.SoldId; // Ensure that SoldId is updated in the context
                                sold.SoldIdLink = sold.SoldId.ToString();

                                _ = await _context.SaveChangesAsync();
                                // Satıcı ödeme işlemi
                                var sellerWallet = await _context.Wallets
                                    .Include(w => w.Transactions)
                                    .FirstOrDefaultAsync(w => w.UserId == sellerId);

                                if (sellerWallet != null)
                                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                                    sellerWallet.Balance += sellerGroup.Sum(c => c.Quantity * c.Product.ProductPrice);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                                    sellerWallet.Transactions.Add(new Transaction
                                    {
                                        Date = DateTime.Now,
                                        Amount = sellerGroup.Sum(c => c.Quantity * c.Product.ProductPrice),
                                        Info = "Ödeme Alındı - Satış No: " + sold.SoldId,
                                        SoldIdLink = sold.SoldId.ToString()
                                    });
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                                    _ = await _context.SaveChangesAsync();
                                }
                                // Mark that the sold operation has been performed for this seller group
                                soldOperationPerformed = true;
                            }
                        }
                    }

                    // Sepeti temizle
                    _context.CartItems.RemoveRange(sellerGroup);
                    _ = await _context.SaveChangesAsync();
                }

                // Tüm işlemler başarılı oldu, işlemi tamamla
                transaction.Commit();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Veritabanı hatası, işlemi geri al ve hata sayfasına yönlendir
                transaction.Rollback();
                // Hata sayfasına yönlendir...
                return RedirectToAction("Index", "Error");
            }
            catch (Exception)
            {
                // Genel bir hata durumu, işlemi geri al ve hata sayfasına yönlendir
                transaction.Rollback();
                // Hata sayfasına yönlendir...
                return RedirectToAction("Index", "Error");
            }

            return RedirectToAction("Index", "Order");
        }




    }
}