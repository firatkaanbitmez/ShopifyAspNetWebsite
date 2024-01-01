//Data/CartViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace ShopAppProject.Data
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Parola gereklidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        // Optionally, if you want to provide a redirect functionality after login
        public string? ReturnUrl { get; set; }
    }
}
