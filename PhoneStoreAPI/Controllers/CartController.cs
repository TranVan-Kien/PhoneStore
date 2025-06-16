using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStoreAPI.Data;
using PhoneStoreAPI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]

        public async Task<IActionResult> AddToCart([FromBody] AddToCartModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Invalid data.",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                    });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.UserId);
                if (user == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "User not found."
                    });
                }

                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == model.ProductId && p.Available == true);
                if (product == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Product not found or not available."
                    });
                }

                // Tìm hoặc tạo đơn hàng tạm thời (Cart) cho user
                var cartOrder = await _context.Orders
                    .FirstOrDefaultAsync(o => o.CustomerId == model.UserId && o.Description == "Cart");
                if (cartOrder == null)
                {
                    cartOrder = new Order
                    {
                        CustomerId = model.UserId,
                        OrderDate = DateTime.UtcNow,
                        Description = "Cart",
                        Amount = 0 // Sẽ cập nhật sau khi thêm chi tiết
                    };
                    _context.Orders.Add(cartOrder);
                    await _context.SaveChangesAsync();
                }

                // Kiểm tra xem sản phẩm đã có trong giỏ chưa
                var existingDetail = await _context.OrderDetails
                    .FirstOrDefaultAsync(od => od.OrderId == cartOrder.Id && od.ProductId == model.ProductId);
                if (existingDetail != null)
                {
                    existingDetail.Quantity += model.Quantity;
                }
                else
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = cartOrder.Id,
                        ProductId = model.ProductId,
                        UnitPrice = product.UnitPrice,
                        Quantity = model.Quantity,
                        Discount = 0
                    };
                    _context.OrderDetails.Add(orderDetail);
                }

                // Cập nhật Amount của đơn hàng
                cartOrder.Amount = await _context.OrderDetails
                    .Where(od => od.OrderId == cartOrder.Id)
                    .SumAsync(od => od.UnitPrice * od.Quantity * (1 - od.Discount));
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Product added to cart successfully.",
                    Data = new
                    {
                        OrderId = cartOrder.Id,
                        ProductId = model.ProductId,
                        Quantity = model.Quantity
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while adding to cart.",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // Xem giỏ hàng
        [HttpGet]
        public async Task<IActionResult> GetCart([FromQuery] Guid userId)
        {
            try
            {
                var cartOrder = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Category)
                    .FirstOrDefaultAsync(o => o.CustomerId == userId && o.Description == "Cart");

                if (cartOrder == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Cart not found."
                    });
                }

                var cartItems = cartOrder.OrderDetails.Select(od => new
                {
                    DetailId = od.Id,
                    ProductId = od.ProductId,
                    ProductName = od.Product.Name,
                    UnitPrice = od.UnitPrice,
                    Quantity = od.Quantity,
                    Discount = od.Discount,
                    Subtotal = od.UnitPrice * od.Quantity * (1 - od.Discount)
                }).ToList();

                return Ok(new
                {
                    Success = true,
                    Message = "Cart retrieved successfully.",
                    Data = new
                    {
                        OrderId = cartOrder.Id,
                        TotalAmount = cartOrder.Amount,
                        Items = cartItems
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while retrieving cart.",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // Xóa sản phẩm khỏi giỏ hàng
        [HttpDelete("{detailId}")]
        public async Task<IActionResult> RemoveFromCart(int detailId)
        {
            try
            {
                var orderDetail = await _context.OrderDetails
                    .Include(od => od.Order)
                    .FirstOrDefaultAsync(od => od.Id == detailId);

                if (orderDetail == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Cart item not found."
                    });
                }

                if (orderDetail.Order.Description != "Cart")
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = "This item is not in a cart."
                    });
                }

                _context.OrderDetails.Remove(orderDetail);
                await _context.SaveChangesAsync();

                // Cập nhật Amount của đơn hàng
                var cartOrder = orderDetail.Order;
                cartOrder.Amount = await _context.OrderDetails
                    .Where(od => od.OrderId == cartOrder.Id)
                    .SumAsync(od => od.UnitPrice * od.Quantity * (1 - od.Discount));
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = "Product removed from cart successfully.",
                    Data = new { DetailId = detailId }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while removing from cart.",
                    Errors = new[] { ex.Message }
                });
            }
        }
    }
}
    