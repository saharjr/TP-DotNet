using CleanArch.Application.Exceptions;
using CleanArch.Application.Interfaces.Repositories;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Exceptions;
using CleanArch.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Repositories;

public class CompteRepository : ICompteRepository
{
    private readonly AppDbContext _context;

    // Injection de la dépendance de la classe de contexte
    public CompteRepository(AppDbContext context)
    {
        _context = context;
    }

    // Comptes triés par date de création, avec l'objet Client
    public async Task<IEnumerable<Compte>> GetAll()
    {
        return await _context.Comptes
            .AsNoTracking()
            .Include(c => c.Client)
            .OrderBy(c => c.DateCreation)
            .ToListAsync();
    }

    public async Task<Compte?> GetById(int compteId)
    {
        return await _context.Comptes
            .AsNoTracking()
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.Id == compteId);
    }

    public async Task<Compte?> GetByNumero(int numeroCompte)
    {
        return await _context.Comptes
            .AsNoTracking()
            .Include(c => c.Client)
            .FirstOrDefaultAsync(c => c.NumeroCompte == numeroCompte);
    }

    // Enregistre un nouveau compte
    public async Task<Compte> Add(Compte c)
    {
        await _context.Comptes.AddAsync(c);
        await _context.SaveChangesAsync();
        return c;
    }

    // Nombre d'opérations réalisées sur un compte (par numéro de compte)
    public async Task<int> Nombre_op(int numeroCompte)
    {
        return await _context.Operations
            .CountAsync(o => o.Compte!.NumeroCompte == numeroCompte);
    }

    // Met à jour les données d'un compte
    public async Task<Compte?> Update(Compte c)
    {
        var existing = await _context.Comptes.FindAsync(c.Id);
        if (existing == null)
            return null;

        // Copie des propriétés scalaires uniquement
        _context.Entry(existing).CurrentValues.SetValues(c);
        await _context.SaveChangesAsync();
        return existing;
    }

    // Supprime un compte
    public async Task<Compte?> Delete(int compteId)
    {
        var compte = await _context.Comptes.FindAsync(compteId);
        if (compte == null)
            return null;

        _context.Comptes.Remove(compte);
        await _context.SaveChangesAsync();
        return compte;
    }

    // Comptes d'un client donné
    public async Task<IEnumerable<Compte>> Search(int clientId)
    {
        return await _context.Comptes
            .AsNoTracking()
            .Include(c => c.Client)
            .Where(c => c.ClientId == clientId)
            .OrderBy(c => c.DateCreation)
            .ToListAsync();
    }

    // Enregistre une opération pour un compte
    public async Task<Operation> Add(Operation o)
    {
        await _context.Operations.AddAsync(o);
        await _context.SaveChangesAsync();
        return o;
    }

    // Met à jour le solde suite à un retrait ou un versement
    public async Task<Compte> Maj_solde(int numcompte, Operation o)
    {
        var compte = await _context.Comptes
            .FirstOrDefaultAsync(c => c.NumeroCompte == numcompte)
            ?? throw new NotFoundException($"Le compte numéro {numcompte} est introuvable.");

        switch (o.TypeOperation)
        {
            case TypeOperation.Retrait:
                compte.Debiter(o.Montant);      // DomainException si solde insuffisant
                break;

            case TypeOperation.Versement:
                compte.Crediter(o.Montant);
                break;

            default:
                throw new DomainException("Seuls les retraits et versements sont gérés ici.");
        }

        await _context.SaveChangesAsync();
        return compte;
    }

    // Opérations d'un compte à partir d'une date, triées par date d'opération
    public async Task<IEnumerable<Operation>> Extrait_Compte(int numcompte, DateTime d)
    {
        return await _context.Operations
            .AsNoTracking()
            .Where(o => o.Compte!.NumeroCompte == numcompte && o.DateOperation >= d)
            .OrderBy(o => o.DateOperation)
            .ToListAsync();
    }

    // Somme des soldes des comptes d'un client
    public async Task<decimal> Solde_Total(int clientId)
    {
        return await _context.Comptes
            .Where(c => c.ClientId == clientId)
            .SumAsync(c => c.Solde);
    }

    // Virement compte1 -> compte2 (atomique : un seul SaveChanges)
    public async Task MAJVirement(int numcompte1, int numcompte2, decimal m)
    {
        var c1 = await _context.Comptes
            .FirstOrDefaultAsync(c => c.NumeroCompte == numcompte1)
            ?? throw new NotFoundException($"Le compte {numcompte1} est introuvable.");

        var c2 = await _context.Comptes
            .FirstOrDefaultAsync(c => c.NumeroCompte == numcompte2)
            ?? throw new NotFoundException($"Le compte {numcompte2} est introuvable.");

        c1.Debiter(m);
        c2.Crediter(m);

        // Trace du virement sur les deux comptes
        var now = DateTime.UtcNow;
        _context.Operations.AddRange(
            new Operation { CompteId = c1.Id, TypeOperation = TypeOperation.Virement, Montant = m, DateOperation = now },
            new Operation { CompteId = c2.Id, TypeOperation = TypeOperation.Virement, Montant = m, DateOperation = now });

        await _context.SaveChangesAsync();
    }

    // Somme des montants retirés par un client sur tous ses comptes
    public async Task<decimal> Total_Retraits(int clientId)
    {
        return await _context.Operations
            .Where(o => o.Compte!.ClientId == clientId && o.TypeOperation == TypeOperation.Retrait)
            .SumAsync(o => o.Montant);
    }

    public Task<bool> ClientExists(int clientId) =>
        _context.Clients.AnyAsync(c => c.Id == clientId);

    public Task<bool> NumeroExists(int numeroCompte) =>
        _context.Comptes.AnyAsync(c => c.NumeroCompte == numeroCompte);

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            await action();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}