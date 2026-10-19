using TP2.Models;

namespace TP2.Models.Repositories
{
    public interface IOrderRepository
    {
        Order GetById(int Id);
        void Add(Order o);
        List<Order> GetOrdersByUserId(string userId);
        List<Order> GetAllOrders(); // <--- Doit retourner List<Order>
    }
}