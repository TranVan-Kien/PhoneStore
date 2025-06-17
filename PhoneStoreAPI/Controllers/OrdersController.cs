using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStoreAPI.Data;
using PhoneStoreAPI.Models;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cart = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.CustomerId == request.CustomerId && o.Description == "Cart");

            if (cart == null || !cart.OrderDetails.Any())
                return NotFound("No cart found or cart is empty.");

            var order = new Order
            {
                CustomerId = request.CustomerId,
                OrderDate = DateTime.Now,
                Address = request.Address,
                Amount = cart.Amount,
                Description = "Order",
                OrderDetails = cart.OrderDetails.ToList(),
                Status = "Pending" // Thêm trạng thái mặc định
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            _context.Orders.Remove(cart);
            await _context.SaveChangesAsync();

            return Ok(new { OrderId = order.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] Guid customerId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.CustomerId == customerId && o.Description != "Cart")
                .ToListAsync();

            if (orders == null || !orders.Any())
                return NotFound("No orders found.");

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                OrderDate = o.OrderDate,
                Address = o.Address,
                Amount = o.Amount,
                Description = o.Description,
                Status = o.Status,
                OrderDetails = o.OrderDetails.Select(d => new OrderDetailDto
                {
                    Id = d.Id,
                    OrderId = d.OrderId,
                    ProductId = d.ProductId,
                    UnitPrice = d.UnitPrice,
                    Quantity = d.Quantity,
                    Discount = d.Discount
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound("Order not found.");

            // Điều kiện hủy (ví dụ: chỉ hủy nếu trong 24 giờ và trạng thái là Pending)
            if (order.Status != "Pending" || (DateTime.Now - order.OrderDate?.ToLocalTime() > TimeSpan.FromHours(24)))
                return BadRequest("Cannot cancel this order.");

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Order cancelled successfully" });
        }
    }

}