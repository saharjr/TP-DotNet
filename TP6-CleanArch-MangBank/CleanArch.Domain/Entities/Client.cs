using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
namespace CleanArch.Domain.Entities;

public class Client
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire."),
        StringLength(100, MinimumLength = 2)]
    public string Nom { get; set; } = null!;
    [Required(ErrorMessage = "Le prénom est obligatoire."),
        StringLength(100, MinimumLength = 2)]
    public string Prenom { get; set; } = null!;
    [Phone, StringLength(20)] public string? Tel { get; set; }
    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = null!;
    [StringLength(250)] public string? Adresse { get; set; }
    [StringLength(500)] public string? Image { get; set; }
    public ICollection<Compte> Comptes { get; set; } = new List<Compte>();
}