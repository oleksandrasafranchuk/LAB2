using System.ComponentModel.DataAnnotations;

namespace CyberFork.DTOs
{
       public record LoginRequest(
        [Required, EmailAddress] string Email,
        [Required] string Password
    );

    public record RegisterRequest(
        [Required, MaxLength(200)] string Name,
        [Required, EmailAddress] string Email,
        [Required, MinLength(6)] string Password,
        [MaxLength(200)] string? Phone,
        [MaxLength(500)] string? Address
    );

    public record AuthResponse(int Id, string Name, string Email, string Role);

   

    public record DishDto(
        int Id, string Name, string? Description,
        decimal Price, bool IsAvailable,
        string? ImageUrl, int CategoryId, string CategoryName,
        DateTime CreatedAt, DateTime UpdatedAt
    );

    public record CreateDishRequest(
        [Required, MaxLength(200)] string Name,
        [MaxLength(1000)] string? Description,
        [Range(0.01, 100000)] decimal Price,
        bool IsAvailable,
        [MaxLength(500)] string? ImageUrl,
        [Required] int CategoryId
    );

    public record UpdateDishRequest(
        [MaxLength(200)] string? Name,
        [MaxLength(1000)] string? Description,
        [Range(0.01, 100000)] decimal? Price,
        bool? IsAvailable,
        [MaxLength(500)] string? ImageUrl,
        int? CategoryId
    );

    
    public record CategoryDto(int Id, string Name, string? Description, int DishCount);

    public record CreateCategoryRequest(
        [Required, MaxLength(100)] string Name,
        [MaxLength(300)] string? Description
    );

    

    public record OrderItemRequest(
        [Required] int DishId,
        [Range(1, 100)] int Quantity
    );

    public record CreateOrderRequest(
        int? UserId,
        [MaxLength(200)] string? GuestName,
        [MaxLength(200)] string? GuestPhone,
        [MaxLength(500)] string? GuestAddress,
        [MaxLength(1000)] string? Comment,
        [Required, MinLength(1)] List<OrderItemRequest> Items
    );

    public record UpdateOrderStatusRequest([Required] int StatusId);

    public record AddCommentRequest([Required, MaxLength(1000)] string Comment);

    public record OrderItemDto(int Id, int DishId, string DishName, int Quantity, decimal UnitPrice, decimal LineTotal);

    public record OrderDto(
        int Id,
        int? UserId,
        string? UserName,
        string? UserPhone,   
        string? UserAddress,
        string? GuestName,
        string? GuestPhone,
        string? GuestAddress,
        string? Comment,
        int StatusId,
        string StatusName,
        decimal TotalAmount,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        List<OrderItemDto> Items
    );

   

    public record OrderStatisticsDto(
        int TotalOrders,
        decimal TotalRevenue,
        int PendingOrders,
        int CompletedOrders,
        int CancelledOrders,
        List<TopDishDto> TopDishes
    );

    public record TopDishDto(int DishId, string DishName, int TotalQuantity, decimal TotalRevenue);

  
    public record UserDto(int Id, string Name, string Email, string Role, string? Phone, string? Address, DateTime CreatedAt);

    public record UpdateUserRequest(
        [MaxLength(200)] string? Name,
        [MaxLength(200)] string? Phone,
        [MaxLength(500)] string? Address
    );


    public record ChangeLogDto(int Id, int DishId, string DishName, string ChangeDescription, string ChangedBy, DateTime ChangedAt);
}
