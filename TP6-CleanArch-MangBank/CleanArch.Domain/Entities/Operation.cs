using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Domain.Entities;

public class Operation
{
    public int Id { get; set; }

    [Required]
    public TypeOperation TypeOperation { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, 1000000000, ErrorMessage = "Le montant doit être strictement positif.")]
    public decimal Montant { get; set; }

    public DateTime DateOperation { get; set; } = DateTime.UtcNow;

    [Required]
    public int CompteId { get; set; }

    public Compte? Compte { get; set; }
}