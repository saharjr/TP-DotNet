namespace TP2.Models.Repositories
{
    public interface IProductRepository
    {
        Product GetById(int id);
        IList<Product> GetAll();
        void Add(Product product);
        Product Update(Product product);
        void Delete(int id);
        IList<Product> GetProductsByCategID(int? CategId);
        IList<Product> FindByName(string name);
    }
}
