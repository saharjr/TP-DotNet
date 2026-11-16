
using CleanArch.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleanArch.Domain.Entities;

public class Compte
{
    public int Id { get; set; }

    [Required, Range(1, int.MaxValue, ErrorMessage = "Le numéro de compte doit être positif.")]
    public int NumeroCompte { get; set; }

    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue, ErrorMessage = "Le solde ne peut pas être négatif.")]
    public decimal Solde { get; set; }

    [Required]
    public TypeCompte TypeCompte { get; set; }

    [Required]
    public int ClientId { get; set; }

    public Client? Client { get; set; }

    public ICollection<Operation> Operations { get; set; } = new List<Operation>();

    // ---------- Règles métier ----------

    public void ValidateBusinessRules()
    {
        if (NumeroCompte <= 0)
            throw new DomainException("Le numéro de compte doit être positif.");

        if (Solde < 0)
            throw new DomainException("Le solde ne peut pas être négatif.");
    }

    public void Crediter(decimal montant)
    {
        if (montant <= 0)
            throw new DomainException("Le montant doit être strictement positif.");

        Solde += montant;
    }

    public void Debiter(decimal montant)
    {
        if (montant <= 0)
            throw new DomainException("Le montant doit être strictement positif.");

        if (Solde < montant)
            throw new DomainException("Solde insuffisant pour effectuer cette opération.");

        Solde -= montant;
    }
}
