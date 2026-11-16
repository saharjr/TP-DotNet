using System.ComponentModel.DataAnnotations;
using CleanArch.Domain.Entities;

namespace CleanArch.Application.DTOs
{
    public class CreateOperationDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "CompteId invalide.")]
        public int CompteId { get; set; }

        [Required]
        public TypeOperation TypeOperation { get; set; }

        [Range(0.01, 1000000000, ErrorMessage = "Le montant doit être strictement positif.")]
        public decimal Montant { get; set; }
    }
}