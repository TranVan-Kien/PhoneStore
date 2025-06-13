using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneStoreAPI.Data;
using PhoneStoreAPI.Models;

namespace PhoneStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // Đăng ký
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Kiểm tra ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new
                {
                    Success = false,
                    Message = "Validation failed.",
                    Errors = errors
                });
            }
            // Kiểm tra đã đồng ý điều khoản chưa
            if (model.IsAgree != true)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "You must agree to the terms and conditions.",
                    Errors = new[] { "IsAgree must be true." }
                });
            }

            // Kiểm tra email đã tồn tại
            bool emailExists = await _context.Users.AnyAsync(u => u.Email == model.Email);
            if (emailExists)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Email already exists.",
                    Errors = new[] { "The email address is already registered." }
                });
            }

            // Tạo đối tượng User
            var user = new User
            {
                Fullname = model.Fullname,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                BirthDate = model.BirthDate,
                IsAgree = model.IsAgree,
                Admin = model.Admin , 
                Activated = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                // Thêm user vào cơ sở dữ liệu
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Trả về phản hồi thành công
                return Ok(new
                {
                    Success = true,
                    Message = "Registration successful.",
                    Data = new
                    {
                        UserId = user.Id,
                        Fullname = user.Fullname,
                        Email = user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi cơ sở dữ liệu hoặc lỗi bất ngờ
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while registering the user.",
                    Errors = new[] { ex.Message } // Chỉ trả về chi tiết lỗi trong môi trường phát triển
                });
            }
        }


        // Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(new { Message = "Login successful.", UserId = user.Id });
        }
    }
}
