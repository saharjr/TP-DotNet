using MassTransit;
using Shared.Models;

namespace OrdersAPI.Consumer
{
    public class ProductCreatedConsumer : IConsumer<ProductCreated>
    {
        private readonly OrdersAPIContext _ordersAPIContext;

        public ProductCreatedConsumer(OrdersAPIContext ordersAPIContext)
        {
            _ordersAPIContext = ordersAPIContext;
        }

        public async Task Consume(ConsumeContext<ProductCreated> context)
        {
            var newProduct = new Product
            {
                Name = context.Message.Name
            };

            _ordersAPIContext.Add(newProduct);
            await _ordersAPIContext.SaveChangesAsync();
        }
    }
}