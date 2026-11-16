using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainLayer.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue,
            ErrorMessage = "Stock cannot be negative.")]
        public int Stock { get; set; }

        // Business rules
        public void ValidateBusinessRules()
        {
            if (Price <= 0)
            {
                throw new InvalidOperationException(
                    "Product price must be positive.");
            }

            if (Stock < 0)
            {
                throw new InvalidOperationException(
                    "Product stock cannot be negative.");
            }
        }
    }
}