using System.ComponentModel.DataAnnotations;

namespace TP2.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Prix en dinar :")]
        public float Price { get; set; }

        [Required]
        [Display(Name = "Quantité en unité :")]
        public int QteStock { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        [Required]
        [Display(Name = "Image :")]
        public string Image { get; set; }

        public List<ProductReview> Reviews { get; set; } = new List<ProductReview>();

        public double AverageRating => Reviews != null && Reviews.Any()
            ? Math.Round(Reviews.Average(r => r.Rating), 1)
            : 0;
    }
}
