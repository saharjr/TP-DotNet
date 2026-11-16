using CleanArch.Application.DTOs;
using CleanArch.Application.Exceptions;
using CleanArch.Application.Interfaces.Repositories;
using CleanArch.Application.Interfaces.Services;
using CleanArch.Application.Mappings;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Exceptions;

namespace CleanArch.Application.Services;

public class CompteService : ICompteService
{
    private readonly ICompteRepository _repository;

    public CompteService(ICompteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CompteDTO>> GetComptesAsync()
    {
        var comptes = await _repository.GetAll();
        return comptes.Select(c => c.ToDTO()).ToList();
    }

    public async Task<CompteDTO> GetCompteAsync(int id)
    {
        var compte = await _repository.GetById(id)
            ?? throw new NotFoundException($"Le compte {id} est introuvable.");

        return compte.ToDTO();
    }

    public async Task<CompteDTO> CreateCompteAsync(CreateCompteDTO dto)
    {
        // Le client doit exister
        if (!await _repository.ClientExists(dto.ClientId))
            throw new NotFoundException($"Le client {dto.ClientId} n'existe pas.");

        if (await _repository.NumeroExists(dto.NumeroCompte))
            throw new DomainException($"Le numéro de compte {dto.NumeroCompte} est déjà utilisé.");

        var compte = new Compte
        {
            NumeroCompte = dto.NumeroCompte,
            TypeCompte = dto.TypeCompte,
            ClientId = dto.ClientId,
            Solde = dto.SoldeInitial,
            DateCreation = DateTime.UtcNow
        };

        compte.ValidateBusinessRules();

        var created = await _repository.Add(compte);

        // Recharge avec le Client
        var full = await _repository.GetById(created.Id);

        return full!.ToDTO();
    }

    public async Task<CompteDTO> UpdateCompteAsync(int id, UpdateCompteDTO dto)
    {
        var compte = await _repository.GetById(id)
            ?? throw new NotFoundException($"Le compte {id} est introuvable.");

        if (!await _repository.ClientExists(dto.ClientId))
            throw new NotFoundException($"Le client {dto.ClientId} n'existe pas.");

        if (compte.NumeroCompte != dto.NumeroCompte &&
            await _repository.NumeroExists(dto.NumeroCompte))
        {
            throw new DomainException($"Le numéro de compte {dto.NumeroCompte} est déjà utilisé.");
        }

        compte.NumeroCompte = dto.NumeroCompte;
        compte.TypeCompte = dto.TypeCompte;
        compte.ClientId = dto.ClientId;

        compte.ValidateBusinessRules();

        await _repository.Update(compte);

        var full = await _repository.GetById(id);

        return full!.ToDTO();
    }

    public async Task DeleteCompteAsync(int id)
    {
        var compte = await _repository.GetById(id)
            ?? throw new NotFoundException($"Le compte {id} est introuvable.");

        // Règle : interdire la suppression si le compte a des opérations
        if (await _repository.Nombre_op(compte.NumeroCompte) > 0)
            throw new DomainException("Suppression interdite : ce compte possède des opérations.");

        await _repository.Delete(id);
    }

    public async Task<IEnumerable<CompteDTO>> SearchAsync(int clientId)
    {
        if (!await _repository.ClientExists(clientId))
            throw new NotFoundException($"Le client {clientId} n'existe pas.");

        var comptes = await _repository.Search(clientId);

        return comptes.Select(c => c.ToDTO()).ToList();
    }

    public async Task<OperationDTO> CreateOperationAsync(CreateOperationDTO dto)
    {
        if (dto.TypeOperation == TypeOperation.Virement)
            throw new DomainException("Pour un virement, utilisez l'endpoint POST api/Compte/virement.");

        var compte = await _repository.GetById(dto.CompteId)
            ?? throw new NotFoundException($"Le compte {dto.CompteId} est introuvable.");

        var operation = new Operation
        {
            CompteId = compte.Id,
            TypeOperation = dto.TypeOperation,
            Montant = dto.Montant,
            DateOperation = DateTime.UtcNow
        };

        // Solde + opération dans une seule transaction.
        // Retrait avec solde insuffisant => DomainException => 400 BadRequest.
        await _repository.ExecuteInTransactionAsync(async () =>
        {
            await _repository.Maj_solde(compte.NumeroCompte, operation);
            await _repository.Add(operation);
        });

        return operation.ToDTO();
    }

    public async Task<int> NombreOperationsAsync(int numeroCompte)
    {
        _ = await _repository.GetByNumero(numeroCompte)
            ?? throw new NotFoundException($"Le compte numéro {numeroCompte} est introuvable.");

        return await _repository.Nombre_op(numeroCompte);
    }

    public async Task<IEnumerable<OperationDTO>> ExtraitAsync(int numeroCompte, DateTime date)
    {
        _ = await _repository.GetByNumero(numeroCompte)
            ?? throw new NotFoundException($"Le compte numéro {numeroCompte} est introuvable.");

        var ops = await _repository.Extrait_Compte(numeroCompte, date);

        return ops.Select(o => o.ToDTO()).ToList();
    }

    public async Task<decimal> SoldeTotalAsync(int clientId)
    {
        if (!await _repository.ClientExists(clientId))
            throw new NotFoundException($"Le client {clientId} n'existe pas.");

        return await _repository.Solde_Total(clientId);
    }

    public async Task VirementAsync(VirementDTO dto)
    {
        if (dto.NumCompteSource == dto.NumCompteDestination)
            throw new DomainException("Les comptes source et destination doivent être différents.");

        _ = await _repository.GetByNumero(dto.NumCompteSource)
            ?? throw new NotFoundException($"Le compte source {dto.NumCompteSource} est introuvable.");

        _ = await _repository.GetByNumero(dto.NumCompteDestination)
            ?? throw new NotFoundException($"Le compte destination {dto.NumCompteDestination} est introuvable.");

        await _repository.MAJVirement(
            dto.NumCompteSource,
            dto.NumCompteDestination,
            dto.Montant
        );
    }

    public async Task<decimal> TotalRetraitsAsync(int clientId)
    {
        if (!await _repository.ClientExists(clientId))
            throw new NotFoundException($"Le client {clientId} n'existe pas.");

        return await _repository.Total_Retraits(clientId);
    }
}