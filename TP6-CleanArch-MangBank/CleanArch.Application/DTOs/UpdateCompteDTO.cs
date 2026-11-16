using System.ComponentModel.DataAnnotations;
using CleanArch.Domain.Entities;

namespace CleanArch.Application.DTOs
{
    public class UpdateCompteDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int NumeroCompte { get; set; }

        [Required]
        public TypeCompte TypeCompte { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ClientId { get; set; }
    }
}