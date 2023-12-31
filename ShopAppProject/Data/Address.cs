//Data/Address.cs
using System.ComponentModel.DataAnnotations.Schema;
namespace ShopAppProject.Data
{
    public class Address
    {
        public int Id { get; set; }
        public string? AdresBasligi { get; set; }
        public string? AdSoyad { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public string? DetayliAdres { get; set; }
        public string? mobilephone { get; set; }
        public string? TCKimlikNo { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
