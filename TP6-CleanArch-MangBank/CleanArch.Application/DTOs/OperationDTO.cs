using CleanArch.Domain.Entities;
using System;

namespace CleanArch.Application.DTOs
{
    public class OperationDTO
    {
        public int Id { get; set; }

        public TypeOperation TypeOperation { get; set; }

        public decimal Montant { get; set; }

        public DateTime DateOperation { get; set; }

        public int CompteId { get; set; }
    }
}