using System.Text.Json.Serialization;

namespace TP2.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        [JsonIgnore] // Empêche la référence circulaire
        public Order Order { get; set; }

        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
    }
}