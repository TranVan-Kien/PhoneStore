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
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // Xem danh sách điện thoại
        [HttpGet("phones")]
        public async Task<IActionResult> GetPhones()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Available == true && p.CategoryId == 1) // Chỉ lấy điện thoại (CategoryId = 1)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.UnitPrice,
                        p.Image,
                        Category = new
                        {
                            p.Category.NameVN
                        }
                    }).ToListAsync();

                if (!products.Any())
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "No phones found."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Phones retrieved successfully.",
                    Data = products
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while retrieving phones.",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // Xem danh sách phụ kiện điện thoại
        [HttpGet("accessories")]
        public async Task<IActionResult> GetAccessories()
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Available == true && p.CategoryId == 2) // Chỉ lấy phụ kiện (CategoryId = 2)
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.UnitPrice,
                        p.Image,
                        Category = new
                        {
                            p.Category.NameVN
                        }
                    }).ToListAsync();

                if (!products.Any())
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "No accessories found."
                    });
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Accessories retrieved successfully.",
                    Data = products
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while retrieving accessories.",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // Xem chi tiết điện thoại theo Id
        [HttpGet("phonesDetail/{id}")]
        public async Task<IActionResult> GetPhone(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Id == id && p.Available == true && p.CategoryId == 1)
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
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = $"Phone with ID {id} not found or is not available."
                    });
                }

                // Tăng ViewCount
                var productEntity = await _context.Products.FindAsync(id);
                if (productEntity != null)
                {
                    productEntity.ViewCount += 1;
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Phone retrieved successfully.",
                    Data = product
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while retrieving the phone.",
                    Errors = new[] { ex.Message }
                });
            }
        }

        // Xem chi tiết phụ kiện theo Id
        [HttpGet("accessoriesDetail/{id}")]
        public async Task<IActionResult> GetAccessory(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Where(p => p.Id == id && p.Available == true && p.CategoryId == 2)
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
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = $"Accessory with ID {id} not found or is not available."
                    });
                }

                // Tăng ViewCount
                var productEntity = await _context.Products.FindAsync(id);
                if (productEntity != null)
                {
                    productEntity.ViewCount += 1;
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    Success = true,
                    Message = "Accessory retrieved successfully.",
                    Data = product
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while retrieving the accessory.",
                    Errors = new[] { ex.Message }
                });
            }
        }
    }
}