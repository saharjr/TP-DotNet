using CleanArch.Application.DTOs;
using CleanArch.Domain.Entities;

namespace CleanArch.Application.Mappings
{
    public static class MappingExtensions
    {
        public static CompteDTO ToDTO(this Compte c) => new()
        {
            Id = c.Id,
            NumeroCompte = c.NumeroCompte,
            DateCreation = c.DateCreation,
            Solde = c.Solde,
            TypeCompte = c.TypeCompte,
            ClientId = c.ClientId,
            ClientNomComplet = c.Client is null
                ? null
                : $"{c.Client.Prenom} {c.Client.Nom}"
        };

        public static OperationDTO ToDTO(this Operation o) => new()
        {
            Id = o.Id,
            TypeOperation = o.TypeOperation,
            Montant = o.Montant,
            DateOperation = o.DateOperation,
            CompteId = o.CompteId
        };
    }
}