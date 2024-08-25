using ShopAppProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
//test


namespace ShopAppProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DataContext _context; // Add this line

        // Add DataContext as a parameter in the constructor
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            DataContext context) // Add this line
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context; // Add this line
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    user.UserProductLists = _context.UserProductLists
                                                       .Include(upl => upl.Product)
                                                        .ThenInclude(p => p.Images)

                                                       .Where(upl => upl.UserId == user.Id)
                                                       .ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                    var roles = await _userManager.GetRolesAsync(user);
                    ViewBag.Roles = roles;

                    // Load the wallet balance and transactions
                    var wallet = _context.Wallets.Include(w => w.Transactions).FirstOrDefault(w => w.UserId == user.Id);
                    ViewBag.WalletBalance = wallet?.Balance;
                    user.Addresses = _context.Addresses.Where(a => a.UserId == user.Id).ToList();


                    return View(user);
                }
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Wallet()
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                // Eagerly load the Transactions property using Include
                var wallet = _context.Wallets.Include(w => w.Transactions).FirstOrDefault(w => w.UserId == user.Id);

                // Set the ViewBag.WalletBalance for the view
                ViewBag.WalletBalance = wallet?.Balance;

                return View(wallet);
            }

            return RedirectToAction("Login");
        }



        [HttpPost]
        public IActionResult AddBalance(decimal amount)
        {
            var user = _userManager.GetUserAsync(User).Result;
            if (user != null)
            {
                var wallet = _context.Wallets.Include(w => w.Transactions).FirstOrDefault(w => w.UserId == user.Id);
                if (wallet != null)
                {
                    // Update the wallet balance
                    wallet.Balance += amount;

                    // Create a new transaction and add it to the Transactions list
                    var transaction = new Transaction
                    {
                        Date = DateTime.Now,
                        Amount = amount,
                        Info = $"Bakiye Yükleme: {amount}", // Provide a meaningful Info value
                        OrderIdLink = null, // Set a default value or a valid value here

                        WalletId = wallet.Id
                    };

                    // Ensure the Transactions list is initialized before adding the transaction
                    wallet.Transactions = wallet.Transactions ?? new List<Transaction>();
                    wallet.Transactions.Add(transaction);

                    _context.SaveChanges();

                    // Pass the updated balance to the view
                    ViewBag.WalletBalance = wallet.Balance;
                }
            }

            // Redirect to the Wallet action
            return RedirectToAction("Wallet");
        }


        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("logout", "Account");
            }

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,

                };



#pragma warning disable CS8604 // Possible null reference argument.
                var result = await _userManager.CreateAsync(user, model.Password);
#pragma warning restore CS8604 // Possible null reference argument.

                if (result.Succeeded)
                {
                    // Kullanıcıyı "Custom" rolüne ekle
                    await _userManager.AddToRoleAsync(user, "Customer");
                    // Create a Wallet for the user with an initial balance (e.g., 0.0)
                    var wallet = new Wallet { Balance = 0.0m, UserId = user.Id };
                    _context.Wallets.Add(wallet);
                    await _context.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["UserEmail"] = user.Email;

                    return RedirectToAction("RegisterSuccess");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult CompanyRegister()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("logout", "Account");
            }

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> CompanyRegister(CompanyRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    BusinessCompany = model.BusinessCompany,
                    BusinessID = model.BusinessID,
                    BusinessMail = model.BusinessMail,
                    BusinessAddress = model.BusinessAddress,
                    BusinessPhoneNumber = model.BusinessPhoneNumber
                };



#pragma warning disable CS8604 // Possible null reference argument.
                var result = await _userManager.CreateAsync(user, model.Password);
#pragma warning restore CS8604 // Possible null reference argument.

                if (result.Succeeded)
                {

                    // Eğer kullanıcı "Müşteri" rolüne sahipse, onu kaldır
                    if (await _userManager.IsInRoleAsync(user, "Customer"))
                    {
                        await _userManager.RemoveFromRoleAsync(user, "Customer");
                    }

                    // Kullanıcıyı "Girişimci" rolüne ekle
                    await _userManager.AddToRoleAsync(user, "Company");
                    // Create a Wallet for the user with an initial balance (e.g., 0.0)
                    var wallet = new Wallet { Balance = 0.0m, UserId = user.Id };
                    _context.Wallets.Add(wallet);
                    await _context.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    TempData["UserEmail"] = user.Email;

                    return RedirectToAction("RegisterSuccess");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterSuccess()
        {
            if (User.Identity?.IsAuthenticated == false)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpGet]
        public IActionResult RegisterError()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

#pragma warning disable CS8604 // Possible null reference argument.
                var user = await _userManager.FindByEmailAsync(model.Email);
#pragma warning restore CS8604 // Possible null reference argument.

                if (user != null)
                {
                    await _signInManager.SignOutAsync();

#pragma warning disable CS8604 // Possible null reference argument.
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
#pragma warning restore CS8604 // Possible null reference argument.

                    if (result.Succeeded)
                    {
                        await _userManager.ResetAccessFailedCountAsync(user);
                        await _userManager.SetLockoutEndDateAsync(user, null);

                        return RedirectToAction("Index", "Home");
                    }
                    else if (result.IsLockedOut)
                    {
                        var lockouteDate = await _userManager.GetLockoutEndDateAsync(user);
                        var timeLeft = lockouteDate.Value - DateTime.UtcNow;
                        ModelState.AddModelError("", $"Hesabınız kilitlendi, {timeLeft.Minutes} dakika sonra tekrar deneyiniz!");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hatalı Email ya da Parola");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Bu email adresiyle bir hesap bulunamadı");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            if (User.Identity?.IsAuthenticated == false)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutConfirmed()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Edit()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (User.Identity.IsAuthenticated)
            {
                var user = _userManager.GetUserAsync(User).Result;
                if (user != null)
                {
                    return View(user);
                }

            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    // Update user properties with the edited values
                    user.FirstName = model.FirstName ?? user.FirstName;
                    user.LastName = model.LastName ?? user.LastName;
                    user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        // Redirect to the Index page after a successful update
                        return RedirectToAction("Index");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If ModelState is not valid, redisplay the edit form with validation errors
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditAddress([FromBody] AddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Geçersiz model." });
            }

            var address = await _context.Addresses.FindAsync(model.Id);
            if (address == null)
            {
                return Json(new { success = false, message = "Adres bulunamadı." });
            }

            // Modelden gelen verilerle adresi güncelle
            address.Street = model.Street;
            address.AdresBasligi = model.AdresBasligi;
            address.AdSoyad = model.AdSoyad;
            address.City = model.City;
            address.State = model.State;
            address.ZipCode = model.ZipCode;
            address.Country = model.Country;
            address.DetayliAdres = model.DetayliAdres;
            address.mobilephone = model.mobilephone;
            address.TCKimlikNo = model.TCKimlikNo;



            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }




        [HttpGet]
        public IActionResult AddAddress()
        {
            return View(new AddressViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var address = new Address
                    {
                        Street = model.Street,
                        City = model.City,
                        AdresBasligi = model.AdresBasligi,
                        AdSoyad = model.AdSoyad,
                        State = model.State,
                        ZipCode = model.ZipCode,
                        Country = model.Country,
                        DetayliAdres = model.DetayliAdres,
                        mobilephone = model.mobilephone,
                        TCKimlikNo = model.TCKimlikNo,

                        UserId = user.Id
                    };

                    _context.Addresses.Add(address);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true });
                }
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            var address = await _context.Addresses.FindAsync(addressId);
            if (address != null)
            {
                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public async Task<IActionResult> SetDefaultAddress(int addressId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Json(new { success = false, message = "User ID not found." });
                }

                // Önceki varsayılan adresi sıfırla
                var previousDefault = _context.Addresses.Where(a => a.UserId == userId && a.IsDefault).ToList();
                foreach (var address in previousDefault)
                {
                    address.IsDefault = false;
                }

                // Yeni varsayılan adresi ayarla
                var newDefaultAddress = await _context.Addresses.FindAsync(addressId);
                if (newDefaultAddress == null)
                {
                    return Json(new { success = false, message = "Address not found." });
                }
                newDefaultAddress.IsDefault = true;

                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (User.Identity?.IsAuthenticated == false)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound($"Kullanıcı ID'si '{_userManager.GetUserId(User)}' bulunamadı.");
                }

                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword!, model.NewPassword!);

                if (changePasswordResult.Succeeded)
                {
                    await _signInManager.SignOutAsync(); // Logout işlemi

                    return RedirectToAction("ChangePasswordSuccess");
                }

                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }


        [HttpGet]
        public IActionResult ChangePasswordSuccess()
        {
            return View();
        }
    }
}
