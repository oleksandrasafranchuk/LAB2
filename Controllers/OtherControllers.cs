using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CyberFork.Data;
using CyberFork.DTOs;
using CyberFork.Models;
using System.Security.Cryptography;
using System.Text;

namespace CyberFork.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AuthController(AppDbContext db) => _db = db;

        
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
        {
            var hash = HashPassword(req.Password);
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == req.Email && u.PasswordHash == hash);

            if (user is null)
                return Unauthorized(new { message = "Невірна електронна пошта або пароль" });

            return Ok(new AuthResponse(user.Id, user.Name, user.Email, user.Role));
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
        {
            if (await _db.Users.AnyAsync(u => u.Email == req.Email))
                return Conflict(new { message = "Користувач з такою поштою вже існує" });

            var user = new User
            {
                Name         = req.Name,
                Email        = req.Email,
                PasswordHash = HashPassword(req.Password),
                Role         = "user",
                Phone        = req.Phone,
                Address      = req.Address,
                CreatedAt    = DateTime.UtcNow,
                UpdatedAt    = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return CreatedAtAction(null, null, new AuthResponse(user.Id, user.Name, user.Email, user.Role));
        }

        private static string HashPassword(string p)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(p));
            return Convert.ToHexString(bytes).ToLower();
        }
    }

   
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public CategoriesController(AppDbContext db) => _db = db;

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var cats = await _db.DishCategories
                .Select(c => new CategoryDto(
                    c.Id, c.Name, c.Description,
                    c.Dishes.Count(d => d.IsAvailable)))
                .ToListAsync();

            return Ok(cats);
        }

       
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryRequest req)
        {
            if (await _db.DishCategories.AnyAsync(c => c.Name.ToLower() == req.Name.ToLower()))
                return Conflict(new { message = $"Категорія «{req.Name}» вже існує" });

            var cat = new DishCategory { Name = req.Name, Description = req.Description };
            _db.DishCategories.Add(cat);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), new { id = cat.Id },
                new CategoryDto(cat.Id, cat.Name, cat.Description, 0));
        }

        
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var cat = await _db.DishCategories.Include(c => c.Dishes).FirstOrDefaultAsync(c => c.Id == id);
            if (cat is null) return NotFound(new { message = "Категорію не знайдено" });

            if (cat.Dishes.Any())
                return Conflict(new { message = "Неможливо видалити категорію, яка містить страви" });

            _db.DishCategories.Remove(cat);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public UsersController(AppDbContext db) => _db = db;

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] string? search)
        {
            var query = _db.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u =>
                    u.Name.ToLower().Contains(search.ToLower()) ||
                    u.Email.ToLower().Contains(search.ToLower()));

            var users = await query
                .OrderBy(u => u.Name)
                .Select(u => new UserDto(u.Id, u.Name, u.Email, u.Role, u.Phone, u.Address, u.CreatedAt))
                .ToListAsync();

            return Ok(users);
        }

       
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var u = await _db.Users.FindAsync(id);
            if (u is null) return NotFound(new { message = "Користувача не знайдено" });
            return Ok(new UserDto(u.Id, u.Name, u.Email, u.Role, u.Phone, u.Address, u.CreatedAt));
        }

        
        [HttpPatch("{id:int}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserRequest req)
        {
            var u = await _db.Users.FindAsync(id);
            if (u is null) return NotFound(new { message = "Користувача не знайдено" });

            if (req.Name    is not null) u.Name    = req.Name;
            if (req.Phone   is not null) u.Phone   = req.Phone;
            if (req.Address is not null) u.Address = req.Address;
            u.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(new UserDto(u.Id, u.Name, u.Email, u.Role, u.Phone, u.Address, u.CreatedAt));
        }
    }

   
    [ApiController]
    [Route("api/orderstatuses")]
    [Produces("application/json")]
    public class OrderStatusesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public OrderStatusesController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderStatus>>> GetStatuses()
            => Ok(await _db.OrderStatuses.OrderBy(s => s.Id).ToListAsync());
    }
}
