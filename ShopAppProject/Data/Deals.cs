//Data/Deals.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ShopAppProject.Data
{
    public class Deals
    {
        [Key]
        public int DealsId { get; set; }

        [Display(Name = "Deals Type")]
        public DealsType DealsType { get; set; }

        [Display(Name = "Deals Name")]
        public string? DealsName { get; set; }

        [Display(Name = "Deals Description")]
        public string? DealsDescription { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        // Sabit İndirim Yüzdesi Kampanyası için Alanlar
        public double? DPercentage { get; set; }
        public double? DPercentageMinimumPrice { get; set; }

        // Miktar İndirimi Kampanyası için Alanlar
        public double? DPrice { get; set; }
        public double? DPriceMinimumPrice { get; set; }

        // İndirim Kuponu Kampanyası için Alanlar
        public string? DCouponCode { get; set; }
        public double? DCodeMinimumPrice { get; set; }

        // Ücretsiz Kargo Kampanyası için Alan
        public double? FSMinimumPrice { get; set; }

        // Hediye Ürün Kampanyası için Alan
        public int? GProductId { get; set; }

        // Kategori İndirimleri Kampanyası için Alanlar
        public string? ProductCategory { get; set; } // Add this line for category

        public double? CDiscountPercentage { get; set; }
        public double? CDMinimumPrice { get; set; }

        // Puan Kazanımları Kampanyası için Alan
        public int? Point { get; set; }

        [Display(Name = "Status")]
        public DealsStatus Status
        {
            get
            {
                var now = DateTime.Now;
                return (now >= StartDate && now <= EndDate) ? DealsStatus.Active : DealsStatus.Expired;
            }
        }
    }

    public enum DealsStatus
    {
        Active,
        Expired
    }
    public enum DealsType
    {
        [Display(Name = "Sabit İndirim Yüzdesi")]
        DiscountPercentage,

        [Display(Name = "Miktar İndirimi")]
        DiscountPrice,

        [Display(Name = "İndirim Kuponu")]
        DiscountCoupon,

        [Display(Name = "Ücretsiz Kargo")]
        FreeShipping,

        [Display(Name = "Hediye Ürün")]
        GiftProduct,

        [Display(Name = "Kategori İndirimleri")]
        CategoryDiscount,

        [Display(Name = "Puan Kazanımları")]
        PointEarning,
    }

}
