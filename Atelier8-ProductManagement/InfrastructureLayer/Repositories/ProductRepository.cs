using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ApplicationLayer.Interfaces.Repositories;
using DomainLayer.Entities;
using InfrastructureLayer.Data;

using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Récupérer tous les produits
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .AsNoTracking()
                .ToListAsync();
        }

        // Récupérer un produit par son identifiant
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(product => product.Id == id);
        }

        // Ajouter un produit
        public async Task<Product?> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();

            return product;
        }

        // Modifier un produit
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);

            await _context.SaveChangesAsync();
        }

        // Supprimer un produit
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                _context.Products.Remove(product);

                await _context.SaveChangesAsync();
            }
        }
    }
}