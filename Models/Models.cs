using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberFork.Models
{
    
    public class DishCategory
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;   

        [MaxLength(300)]
        public string? Description { get; set; }

       
        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();
    }

    
    public class OrderStatus
    {
        public int Id { get; set; }

        [Required, MaxLength(60)]
        public string Name { get; set; } = string.Empty;   

        [MaxLength(200)]
        public string? Description { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

   
    public class Dish
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "numeric(10,2)")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        
        public int CategoryId { get; set; }
        public DishCategory Category { get; set; } = null!;

        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        
        public ICollection<DishChangeLog> ChangeLogs { get; set; } = new List<DishChangeLog>();
    }

   
    public class DishChangeLog
    {
        public int Id { get; set; }

        public int DishId { get; set; }
        public Dish Dish { get; set; } = null!;

        [Required, MaxLength(500)]
        public string ChangeDescription { get; set; } = string.Empty;  

        [MaxLength(100)]
        public string ChangedBy { get; set; } = "admin";

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }

   
    public class Order
    {
        public int Id { get; set; }

       
        public int? UserId { get; set; }
        public User? User { get; set; }

        [MaxLength(200)]
        public string? GuestName { get; set; }

        [MaxLength(200)]
        public string? GuestPhone { get; set; }

        [MaxLength(500)]
        public string? GuestAddress { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public int StatusId { get; set; }
        public OrderStatus Status { get; set; } = null!;

        [Column(TypeName = "numeric(10,2)")]
        public decimal TotalAmount { get; set; }

      
        [Column(TypeName = "timestamptz")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "timestamptz")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int DishId { get; set; }
        public Dish Dish { get; set; } = null!;

        [Range(1, 100)]
        public int Quantity { get; set; }

        [Column(TypeName = "numeric(10,2)")]
        public decimal UnitPrice { get; set; }   
    }

    
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        
        [Required, MaxLength(20)]
        public string Role { get; set; } = "user";

        [MaxLength(200)]
        public string? Phone { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
