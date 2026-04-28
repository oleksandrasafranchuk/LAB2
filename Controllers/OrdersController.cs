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
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(AppDbContext db, ILogger<OrdersController> logger)
        {
            _db = db;
            _logger = logger;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(
            [FromQuery] int? userId,
            [FromQuery] int? statusId,
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] string? search)
        {
            var query = _db.Orders
                .Include(o => o.User)
                .Include(o => o.Status)
                .Include(o => o.Items).ThenInclude(i => i.Dish)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(o => o.UserId == userId.Value);

            if (statusId.HasValue)
                query = query.Where(o => o.StatusId == statusId.Value);

            if (from.HasValue)
                query = query.Where(o => o.CreatedAt >= from.Value.ToUniversalTime());

            if (to.HasValue)
                query = query.Where(o => o.CreatedAt <= to.Value.ToUniversalTime());

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(o =>
                    (o.GuestName != null && o.GuestName.ToLower().Contains(search.ToLower())) ||
                    (o.GuestPhone != null && o.GuestPhone.Contains(search)) ||
                    (o.Comment   != null && o.Comment.ToLower().Contains(search.ToLower())) ||
                    (o.User != null && o.User.Name.ToLower().Contains(search.ToLower())));

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders.Select(MapOrder));
        }

       
       
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Status)
                .Include(o => o.Items).ThenInclude(i => i.Dish)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
                return NotFound(new { message = $"Замовлення Id={id} не знайдено" });

            return Ok(MapOrder(order));
        }

      
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest req)
        {
            
            if (req.UserId is null)
            {
                if (string.IsNullOrWhiteSpace(req.GuestName))
                    return BadRequest(new { message = "Для гостя обов'язкове поле «Ім'я»" });
                if (string.IsNullOrWhiteSpace(req.GuestPhone))
                    return BadRequest(new { message = "Для гостя обов'язкове поле «Телефон»" });
            }
            else
            {
                if (!await _db.Users.AnyAsync(u => u.Id == req.UserId.Value))
                    return BadRequest(new { message = "Вказаного користувача не знайдено" });
            }

            if (req.Items is null || req.Items.Count == 0)
                return BadRequest(new { message = "Замовлення не може бути порожнім" });

           
            var dishIds = req.Items.Select(i => i.DishId).Distinct().ToList();
            var dishes = await _db.Dishes
                .Where(d => dishIds.Contains(d.Id) && d.IsAvailable)
                .ToDictionaryAsync(d => d.Id);

            var missingIds = dishIds.Except(dishes.Keys).ToList();
            if (missingIds.Any())
                return BadRequest(new { message = $"Страви не знайдено або недоступні: Id={string.Join(",", missingIds)}" });

           
            var items = req.Items.Select(i => new OrderItem
            {
                DishId    = i.DishId,
                Quantity  = i.Quantity,
                UnitPrice = dishes[i.DishId].Price
            }).ToList();

            var total = items.Sum(i => i.UnitPrice * i.Quantity);

            var order = new Order
            {
                UserId       = req.UserId,
                GuestName    = req.GuestName,
                GuestPhone   = req.GuestPhone,
                GuestAddress = req.GuestAddress,
                Comment      = req.Comment,
                StatusId     = 1,   
                TotalAmount  = total,
                CreatedAt    = DateTime.UtcNow,
                UpdatedAt    = DateTime.UtcNow,
                Items        = items
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

           
            var saved = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Status)
                .Include(o => o.Items).ThenInclude(i => i.Dish)
                .FirstAsync(o => o.Id == order.Id);

            _logger.LogInformation("Створено замовлення Id={Id}, сума={Total}", order.Id, total);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, MapOrder(saved));
        }

        
       
[HttpPatch("{id:int}/status")]
public async Task<ActionResult<OrderDto>> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest req)
{
    var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id);

    if (order is null)
        return NotFound(new { message = $"Замовлення Id={id} не знайдено" });

    var newStatus = await _db.OrderStatuses.FirstOrDefaultAsync(s => s.Id == req.StatusId);
    if (newStatus is null)
        return BadRequest(new { message = "Вказаного статусу не існує" });

    if (order.StatusId == 5)
        return Conflict(new { message = "Скасоване замовлення не можна змінити" });

    if (req.StatusId != 5 && req.StatusId < order.StatusId)
        return Conflict(new { message = "Статус замовлення не може бути знижений" });

    order.StatusId = req.StatusId;
    order.UpdatedAt = DateTime.UtcNow;

    try 
    {
        await _db.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Помилка збереження статусу для замовлення {Id}", id);
        return StatusCode(500, new { message = "Помилка бази даних при оновленні" });
    }

    var updatedOrder = await _db.Orders
        .Include(o => o.User)
        .Include(o => o.Status)
        .Include(o => o.Items).ThenInclude(i => i.Dish)
        .FirstOrDefaultAsync(o => o.Id == id);

    return Ok(MapOrder(updatedOrder!));
}
        
        [HttpPatch("{id:int}/comment")]
        public async Task<ActionResult<OrderDto>> AddComment(int id, [FromBody] AddCommentRequest req)
        {
            var order = await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Status)
                .Include(o => o.Items).ThenInclude(i => i.Dish)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null)
                return NotFound(new { message = $"Замовлення Id={id} не знайдено" });

            if (order.StatusId is 4 or 5)
                return Conflict(new { message = "Коментар можна додавати лише до активних замовлень" });

            order.Comment   = req.Comment;
            order.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(MapOrder(order));
        }

      
        [HttpGet("statistics")]
public async Task<ActionResult<OrderStatisticsDto>> GetStatistics(
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to)
{
    try 
    {
        
        var query = _db.Orders.AsNoTracking();

        if (from.HasValue) 
            query = query.Where(o => o.CreatedAt >= DateTime.SpecifyKind(from.Value, DateTimeKind.Utc));
        if (to.HasValue)   
            query = query.Where(o => o.CreatedAt <= DateTime.SpecifyKind(to.Value, DateTimeKind.Utc));

        var orders = await query.ToListAsync();
        var orderIds = orders.Select(o => o.Id).ToList();

        
        var topDishesRaw = await _db.OrderItems
            .AsNoTracking()
            .Where(oi => orderIds.Contains(oi.OrderId))
            .Include(oi => oi.Dish)
            .ToListAsync();

        var topDishes = topDishesRaw
            .GroupBy(oi => new { oi.DishId, Name = oi.Dish?.Name ?? "Невідомо" })
            .Select(g => new TopDishDto(
                g.Key.DishId, 
                g.Key.Name,
                g.Sum(x => x.Quantity),
                g.Sum(x => x.Quantity * x.UnitPrice)))
            .OrderByDescending(x => x.TotalQuantity)
            .Take(5)
            .ToList();

      
        var stats = new OrderStatisticsDto(
            TotalOrders:    orders.Count,
            TotalRevenue:   orders.Where(o => o.StatusId == 4).Sum(o => o.TotalAmount),
            PendingOrders:  orders.Count(o => o.StatusId >= 1 && o.StatusId <= 3),
            CompletedOrders:orders.Count(o => o.StatusId == 4),
            CancelledOrders:orders.Count(o => o.StatusId == 5),
            TopDishes:      topDishes
        );

        return Ok(stats);
    }
    catch (Exception ex)
    {
       
        _logger.LogError("ex: {Message}", ex.Message);
        return StatusCode(500, new { message = "Помилка при розрахунку статистики" });
    }
}

      

       private static OrderDto MapOrder(Order o) => new OrderDto(
    o.Id,
    o.UserId, 
    o.User?.Name,
    o.User?.Phone,   
    o.User?.Address,
    o.GuestName, 
    o.GuestPhone, 
    o.GuestAddress,
    o.Comment,
    o.StatusId, 
    o.Status?.Name ?? "",
    o.TotalAmount,
    o.CreatedAt, 
    o.UpdatedAt,
    o.Items.Select(i => new OrderItemDto(
        i.Id, i.DishId, i.Dish?.Name ?? "",
        i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice
    )).ToList()
);
    }
}
