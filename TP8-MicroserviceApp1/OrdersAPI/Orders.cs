namespace OrdersAPI
{
    public class Orders
    {
        public int OrdersId { get; set; }
        public DateTime OrderDate { get; }
        public string? CustomerName { get; set; }
        public int ProductId { get; set; }
    }
}
