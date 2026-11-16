using CleanArch.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArch.Application.Interfaces.Services
{
    public interface ICompteService
    {
        Task<IEnumerable<CompteDTO>> GetComptesAsync();

        Task<CompteDTO> GetCompteAsync(int id);

        Task<CompteDTO> CreateCompteAsync(CreateCompteDTO dto);

        Task<CompteDTO> UpdateCompteAsync(int id, UpdateCompteDTO dto);

        Task DeleteCompteAsync(int id);

        Task<IEnumerable<CompteDTO>> SearchAsync(int clientId);

        Task<OperationDTO> CreateOperationAsync(CreateOperationDTO dto);

        Task<int> NombreOperationsAsync(int numeroCompte);

        Task<IEnumerable<OperationDTO>> ExtraitAsync(int numeroCompte, DateTime date);

        Task<decimal> SoldeTotalAsync(int clientId);

        Task VirementAsync(VirementDTO dto);

        Task<decimal> TotalRetraitsAsync(int clientId);
    }
}