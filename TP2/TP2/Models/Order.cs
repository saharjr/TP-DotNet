using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TP2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public float TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }

        public string UserId { get; set; }
        public IdentityUser User { get; set; }

        public List<OrderItem> Items { get; set; }
    }
}