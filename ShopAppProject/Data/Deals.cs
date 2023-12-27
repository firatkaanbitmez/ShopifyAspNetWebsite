//Data/Deals.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        public double? minimumsepettutari { get; set; }



        //Sepette Yüzde İndirimi Kampanyası için Alanlar
        public double? DPercentage { get; set; }


        // Sepet Fiyat İndirimi Kampanyası için Alanlar
        public double? DPrice { get; set; }

        // x Al y Öde Kampanyası için Alanlar
        public double? xProduct { get; set; }
        public double? yProduct { get; set; }



        // İndirim Kuponu Kampanyası için Alanlar
        public string? DCouponCode { get; set; }


        // Ücretsiz Kargo Kampanyası için Alan
        public int? ShippingId { get; set; }


        [ForeignKey("ShippingId")]
        public Shipping? Shipping { get; set; }

        // Hediye Ürün Kampanyası için Alan
        public int? GProductId { get; set; }

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
        [Display(Name = "Sepette Yüzde İndirimi")]
        DiscountPercentage,

        [Display(Name = "Sepet Fiyat İndirimi")]
        DiscountPrice,
        [Display(Name = "x Al y Öde")]
        xydeal,

        [Display(Name = "İndirim Kuponu")]
        DiscountCoupon,

        [Display(Name = "Ücretsiz Kargo")]
        FreeShipping,

        [Display(Name = "Hediye Ürün")]
        GiftProduct,

        [Display(Name = "Puan Kazanımları")]
        PointEarning,
    }

}
