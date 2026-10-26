using System.Collections.Generic;
using TP2.Models;

namespace TP2.Models.Repositories
{
    public interface IOrderRepository
    {
        Order GetById(int Id);
        void Add(Order o);
        void Update(Order o); // <-- Méthode ajoutée
        List<Order> GetOrdersByUserId(string userId);
        List<Order> GetAllOrders();
    }
}