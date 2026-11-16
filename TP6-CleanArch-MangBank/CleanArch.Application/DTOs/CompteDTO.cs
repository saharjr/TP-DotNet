using CleanArch.Domain.Entities;
using System;

namespace CleanArch.Application.DTOs
{
    public class CompteDTO
    {
        public int Id { get; set; }

        public int NumeroCompte { get; set; }

        public DateTime DateCreation { get; set; }

        public decimal Solde { get; set; }

        public TypeCompte TypeCompte { get; set; }

        public int ClientId { get; set; }

        public string? ClientNomComplet { get; set; }
    }
}