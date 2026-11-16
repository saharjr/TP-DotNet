using System.ComponentModel.DataAnnotations;
using CleanArch.Domain.Entities;

namespace CleanArch.Application.DTOs
{
    public class CreateCompteDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Le numéro de compte doit être positif.")]
        public int NumeroCompte { get; set; }

        [Required]
        public TypeCompte TypeCompte { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ClientId invalide.")]
        public int ClientId { get; set; }

        [Range(0, 1000000000, ErrorMessage = "Le solde initial ne peut pas être négatif.")]
        public decimal SoldeInitial { get; set; }
    }
}