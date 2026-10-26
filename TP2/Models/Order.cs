using System;
using System.Collections.Generic;

namespace TP2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalAmount { get; set; }

        // --- Champs de suivi paiement Stripe ---
        public string PaymentStatus { get; set; } = "En attente";
        public string PaymentMethod { get; set; } = "Carte Bancaire";
        public string? StripeSessionId { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}