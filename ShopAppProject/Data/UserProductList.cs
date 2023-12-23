//Data/Product.cs
using ShopAppProject.Models;
using System.ComponentModel.DataAnnotations;

namespace ShopAppProject.Data
{
    public class UserProductList
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // Foreign key for the product
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }

}