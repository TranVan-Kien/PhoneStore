using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStoreAPI.Data;
using PhoneStoreAPI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/orders
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                    .ThenInclude(p => p.Category)
                    .ToListAsync();

                if (!orders.Any())
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "No orders found."
                    });
                }

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
                        Discount = d.Discount,
                        ProductName = d.Product.Name,
                        CategoryNameVN = d.Product.Category.NameVN
                    }).ToList()
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Message = "Orders retrieved successfully.",
                    Data = orderDtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while retrieving orders.",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // PUT: api/admin/orders/{id}/status
        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { Success = false, Message = "Invalid data." });

                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                    return NotFound(new { Success = false, Message = $"Order with ID {id} not found." });

                order.Status = request.Status;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Order status updated successfully.",
                    Data = order
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while updating order status.",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                if (!users.Any())
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "No users found."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Users retrieved successfully.",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while retrieving users.",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // POST: api/admin/products
        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] Products product)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { Success = false, Message = "Invalid product data." });

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, new
                {
                    Success = true,
                    Message = "Product created successfully.",
                    Data = product
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); // Log lỗi để debug
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while creating product.",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // PUT: api/admin/products/{id}
        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Products product)
        {
            try
            {
                if (id != product.Id || !ModelState.IsValid)
                    return BadRequest(new { Success = false, Message = "Invalid product ID or data." });

                var existingProduct = await _context.Products.FindAsync(id);
                if (existingProduct == null)
                    return NotFound(new { Success = false, Message = $"Product with ID {id} not found." });

                _context.Entry(existingProduct).CurrentValues.SetValues(product);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Product updated successfully.",
                    Data = product
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while updating product.",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // DELETE: api/admin/products/{id}
        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                    return NotFound(new { Success = false, Message = $"Product with ID {id} not found." });

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Product deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while deleting product.",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // GET: api/admin/products/{id}
        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Id == id && p.Available)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.UnitPrice,
                        p.Image,
                        p.AvailableDate,
                        p.Available,
                        Category = new
                        {
                            p.Category.Id,
                            p.Category.Name,
                            p.Category.NameVN
                        },
                        p.Quantity,
                        p.Description,
                        p.ViewCount,
                        p.Special
                    })
                    .FirstOrDefaultAsync();

                if (product == null)
                    return NotFound(new { Success = false, Message = $"Product with ID {id} not found or is not available." });

                return Ok(new
                {
                    Success = true,
                    Message = "Product retrieved successfully.",
                    Data = product
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while retrieving product.",
                    Errors = new[] { ex.Message }
                });
            }
        }
    }
}