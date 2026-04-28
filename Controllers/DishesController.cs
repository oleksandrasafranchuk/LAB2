using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CyberFork.Data;
using CyberFork.DTOs;
using CyberFork.Models;

namespace CyberFork.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DishesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<DishesController> _logger;

        public DishesController(AppDbContext db, ILogger<DishesController> logger)
        {
            _db = db;
            _logger = logger;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DishDto>>> GetDishes(
            [FromQuery] string? search,
            [FromQuery] int? categoryId,
            [FromQuery] bool? available,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            var query = _db.Dishes
                .Include(d => d.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(d =>
                    d.Name.ToLower().Contains(search.ToLower()) ||
                    (d.Description != null && d.Description.ToLower().Contains(search.ToLower())));

            if (categoryId.HasValue)
                query = query.Where(d => d.CategoryId == categoryId.Value);

            if (available.HasValue)
                query = query.Where(d => d.IsAvailable == available.Value);

            if (minPrice.HasValue)
                query = query.Where(d => d.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(d => d.Price <= maxPrice.Value);

            var dishes = await query
                .OrderBy(d => d.CategoryId)
                .ThenBy(d => d.Name)
                .Select(d => ToDto(d))
                .ToListAsync();

            return Ok(dishes);
        }

        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DishDto>> GetDish(int id)
        {
            var dish = await _db.Dishes
                .Include(d => d.Category)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dish is null)
                return NotFound(new { message = $"Страву з Id={id} не знайдено" });

            return Ok(ToDto(dish));
        }

        [HttpPost]
        public async Task<ActionResult<DishDto>> CreateDish([FromBody] CreateDishRequest req)
        {
            if (!await _db.DishCategories.AnyAsync(c => c.Id == req.CategoryId))
                return BadRequest(new { message = "Вказану категорію не знайдено" });

            if (await _db.Dishes.AnyAsync(d => d.Name.ToLower() == req.Name.ToLower()))
                return Conflict(new { message = $"Страва з назвою «{req.Name}» вже існує" });

            var dish = new Dish
            {
                Name        = req.Name,
                Description = req.Description,
                Price       = req.Price,
                IsAvailable = req.IsAvailable,
                ImageUrl    = req.ImageUrl,
                CategoryId  = req.CategoryId,
                CreatedAt   = DateTime.UtcNow,
                UpdatedAt   = DateTime.UtcNow
            };

            _db.Dishes.Add(dish);
            await _db.SaveChangesAsync();

            await _db.Entry(dish).Reference(d => d.Category).LoadAsync();

            _logger.LogInformation("Додано нову страву: {Name}", dish.Name);
            return CreatedAtAction(nameof(GetDish), new { id = dish.Id }, ToDto(dish));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<DishDto>> UpdateDish(int id, [FromBody] UpdateDishRequest req)
        {
            var dish = await _db.Dishes.Include(d => d.Category).FirstOrDefaultAsync(d => d.Id == id);
            if (dish is null)
                return NotFound(new { message = $"Страву з Id={id} не знайдено" });

            var changes = new List<string>();

            if (req.Name is not null && req.Name != dish.Name)
            {
                if (await _db.Dishes.AnyAsync(d => d.Id != id && d.Name.ToLower() == req.Name.ToLower()))
                    return Conflict(new { message = $"Страва з назвою «{req.Name}» вже існує" });
                changes.Add($"Назва: «{dish.Name}» → «{req.Name}»");
                dish.Name = req.Name;
            }

            if (req.Description is not null && req.Description != dish.Description)
            {
                changes.Add("Оновлено опис");
                dish.Description = req.Description;
            }

            if (req.Price.HasValue && req.Price.Value != dish.Price)
            {
                changes.Add($"Ціна: {dish.Price} → {req.Price.Value} грн");
                dish.Price = req.Price.Value;
            }

            if (req.IsAvailable.HasValue && req.IsAvailable.Value != dish.IsAvailable)
            {
                changes.Add($"Доступність: {(dish.IsAvailable ? "доступна" : "недоступна")} → {(req.IsAvailable.Value ? "доступна" : "недоступна")}");
                dish.IsAvailable = req.IsAvailable.Value;
            }

            if (req.ImageUrl is not null) dish.ImageUrl = req.ImageUrl;

            if (req.CategoryId.HasValue && req.CategoryId.Value != dish.CategoryId)
            {
                if (!await _db.DishCategories.AnyAsync(c => c.Id == req.CategoryId.Value))
                    return BadRequest(new { message = "Вказану категорію не знайдено" });
                changes.Add($"Категорію змінено");
                dish.CategoryId = req.CategoryId.Value;
            }

            if (changes.Count == 0)
                return Ok(ToDto(dish));

            dish.UpdatedAt = DateTime.UtcNow;

            _db.DishChangeLogs.Add(new DishChangeLog
            {
                DishId = dish.Id,
                ChangeDescription = string.Join("; ", changes),
                ChangedBy = "admin",
                ChangedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            await _db.Entry(dish).Reference(d => d.Category).LoadAsync();

            _logger.LogInformation("Страву Id={Id} оновлено: {Changes}", id, string.Join("; ", changes));
            return Ok(ToDto(dish));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _db.Dishes.FindAsync(id);
            if (dish is null)
                return NotFound(new { message = $"Страву з Id={id} не знайдено" });

            var activeOrderExists = await _db.OrderItems
                .AnyAsync(oi => oi.DishId == id &&
                    _db.Orders.Any(o => o.Id == oi.OrderId && o.StatusId != 4 && o.StatusId != 5));

            if (activeOrderExists)
                return Conflict(new { message = "Страву неможливо видалити: є активні замовлення з нею" });

            _db.Dishes.Remove(dish);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Страву Id={Id} видалено", id);
            return NoContent();
        }

        [HttpGet("{id:int}/changelog")]
        public async Task<ActionResult<IEnumerable<ChangeLogDto>>> GetChangeLog(int id)
        {
            if (!await _db.Dishes.AnyAsync(d => d.Id == id))
                return NotFound(new { message = $"Страву з Id={id} не знайдено" });

            var logs = await _db.DishChangeLogs
                .Where(cl => cl.DishId == id)
                .Include(cl => cl.Dish)
                .OrderByDescending(cl => cl.ChangedAt)
                .Select(cl => new ChangeLogDto(
                    cl.Id, cl.DishId, cl.Dish.Name,
                    cl.ChangeDescription, cl.ChangedBy, cl.ChangedAt))
                .ToListAsync();

            return Ok(logs);
        }

        private static DishDto ToDto(Dish d) => new(
            d.Id, d.Name, d.Description, d.Price, d.IsAvailable,
            d.ImageUrl, d.CategoryId, d.Category?.Name ?? "",
            d.CreatedAt, d.UpdatedAt);
    }
}
