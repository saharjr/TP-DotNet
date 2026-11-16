using CleanArch.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CleanArch.Application.Interfaces.Repositories
{
    public interface ICompteRepository
    {
        Task<IEnumerable<Compte>> GetAll();

        Task<Compte?> GetById(int compteId);

        Task<Compte> Add(Compte c);

        Task<int> Nombre_op(int numeroCompte);

        Task<Compte?> Update(Compte c);

        Task<Compte?> Delete(int compteId);

        Task<IEnumerable<Compte>> Search(int clientId);

        Task<Operation> Add(Operation o);

        Task<Compte> Maj_solde(int numcompte, Operation o);

        Task<IEnumerable<Operation>> Extrait_Compte(int numcompte, DateTime d);

        Task<decimal> Solde_Total(int clientId);

        Task MAJVirement(int numcompte1, int numcompte2, decimal m);

        Task<decimal> Total_Retraits(int clientId);

        // Méthodes utilitaires
        Task<Compte?> GetByNumero(int numeroCompte);

        Task<bool> ClientExists(int clientId);

        Task<bool> NumeroExists(int numeroCompte);

        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}