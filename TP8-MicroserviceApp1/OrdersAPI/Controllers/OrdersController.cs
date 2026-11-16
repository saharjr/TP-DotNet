using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersAPI;

[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly OrdersAPIContext _context;
    public OrdersController(OrdersAPIContext context)
    {
        _context = context;
    }

    // GET: api/Orders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Orders>>> GetOrders()
    {
        return await _context.Orders.ToListAsync();
    }

    // GET: api/Orders/5
    [HttpGet("{ordersid}")]
    public async Task<ActionResult<Orders>> GetOrders(int ordersid)
    {
        var orders = await _context.Orders.FindAsync(ordersid);

        if (orders == null)
        {
            return NotFound();
        }

        return orders;
    }

    // PUT: api/Orders/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{ordersid}")]
    public async Task<IActionResult> PutOrders(int? ordersid, Orders orders)
    {
        if (ordersid != orders.OrdersId)
        {
            return BadRequest();
        }

        _context.Entry(orders).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrdersExists(ordersid))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Orders
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Orders>> PostOrders(Orders orders)
    {
        _context.Orders.Add(orders);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetOrders", new { ordersid = orders.OrdersId }, orders);
    }

    // DELETE: api/Orders/5
    [HttpDelete("{ordersid}")]
    public async Task<IActionResult> DeleteOrders(int? ordersid)
    {
        var orders = await _context.Orders.FindAsync(ordersid);
        if (orders == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(orders);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool OrdersExists(int? ordersid)
    {
        return _context.Orders.Any(e => e.OrdersId == ordersid);
    }
}
