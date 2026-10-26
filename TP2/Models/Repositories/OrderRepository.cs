using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using TP2.Models;

namespace TP2.Models.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public Order GetById(int id)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == id);
        }

        public void Add(Order o)
        {
            _context.Orders.Add(o);
            _context.SaveChanges();
        }

        public void Update(Order o)
        {
            _context.Orders.Update(o);
            _context.SaveChanges();
        }

        public List<Order> GetOrdersByUserId(string userId)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public List<Order> GetAllOrders()
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
    }
}