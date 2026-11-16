using System.ComponentModel.DataAnnotations;

namespace CleanArch.Application.DTOs
{
    public class VirementDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int NumCompteSource { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int NumCompteDestination { get; set; }

        [Range(0.01, 1000000000, ErrorMessage = "Le montant doit être strictement positif.")]
        public decimal Montant { get; set; }
    }
}