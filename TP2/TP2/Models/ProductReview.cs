using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace TP2.Models
{
    public class ProductReview
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [JsonIgnore]
        public Product Product { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        [Required]
        public string UserName { get; set; }

        [Range(1, 5, ErrorMessage = "La note doit être comprise entre 1 et 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Veuillez saisir un commentaire.")]
        [StringLength(500, ErrorMessage = "Le commentaire ne peut pas dépasser 500 caractères.")]
        public string Comment { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.Now;
    }
}