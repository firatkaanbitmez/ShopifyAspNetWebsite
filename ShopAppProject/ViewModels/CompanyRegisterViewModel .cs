//Data/CompanyRegisterViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ShopAppProject.Data
{
    public class CompanyRegisterViewModel
    {
        [Required(ErrorMessage = "E-posta adresi gereklidir.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Parola gereklidir.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Parolalar eşleşmiyor.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "İsim gereklidir.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Soyisim gereklidir.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Telefon numarası gereklidir.")]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "İşletme adı gereklidir.")]
        public string? BusinessCompany { get; set; }

        [Required(ErrorMessage = "İşletme kimlik numarası gereklidir.")]
        [Display(Name = "Business ID")]
        public string? BusinessID { get; set; }

        [Required(ErrorMessage = "İşletme e-posta adresi gereklidir.")]
        [Display(Name = "Business Mail")]
        public string? BusinessMail { get; set; }

        [Required(ErrorMessage = "İşletme adresi gereklidir.")]
        [Display(Name = "Business Address")]
        public string? BusinessAddress { get; set; }

        [Required(ErrorMessage = "İşletme telefon numarası gereklidir.")]
        [Display(Name = "Business Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public string? BusinessPhoneNumber { get; set; }
    }
}
