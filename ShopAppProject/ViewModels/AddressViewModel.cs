//ViewModels/AddressViewModel.cs
using System.ComponentModel.DataAnnotations;
//test

namespace ShopAppProject.Data
{
    public class AddressViewModel
    {
        public int Id { get; set; }
        [Required]
        public string? AdresBasligi { get; set; }
        [Required]
        public string? AdSoyad { get; set; }

        [Required]
        public string? Street { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? State { get; set; }
        [Required]
        public string? ZipCode { get; set; }
        [Required]
        public string? Country { get; set; }
        [Required]
        public string? DetayliAdres { get; set; }
        [Required]
        public string? mobilephone { get; set; }
        [Required]
        public string? TCKimlikNo { get; set; }


    }

}
