using Microsoft.EntityFrameworkCore;
using CyberFork.Models;
using System.Security.Cryptography;
using System.Text;

namespace CyberFork.Data
{
   
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        

        public DbSet<User> Users => Set<User>();
        public DbSet<DishCategory> DishCategories => Set<DishCategory>();
        public DbSet<Dish> Dishes => Set<Dish>();
        public DbSet<DishChangeLog> DishChangeLogs => Set<DishChangeLog>();
        public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

           
            mb.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            mb.Entity<Order>()
                .HasIndex(o => o.CreatedAt);

            mb.Entity<DishChangeLog>()
                .HasIndex(cl => cl.DishId);

           mb.Entity<Dish>()
              .HasIndex(d => d.Name).IsUnique();

            mb.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            mb.Entity<OrderItem>()
                .HasOne(oi => oi.Dish)
                .WithMany(d => d.OrderItems)
                .HasForeignKey(oi => oi.DishId)
                .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<DishChangeLog>()
                .HasOne(cl => cl.Dish)
                .WithMany(d => d.ChangeLogs)
                .HasForeignKey(cl => cl.DishId)
                .OnDelete(DeleteBehavior.Cascade);

           

            SeedData(mb);
        }

       
        private static void SeedData(ModelBuilder mb)
        {
            
            mb.Entity<OrderStatus>().HasData(
                new OrderStatus { Id = 1, Name = "Нове",           Description = "Замовлення щойно створено" },
                new OrderStatus { Id = 2, Name = "Готується",      Description = "Кухня приступила до приготування" },
                new OrderStatus { Id = 3, Name = "Доставляється",  Description = "Замовлення передано кур'єру" },
                new OrderStatus { Id = 4, Name = "Виконано",       Description = "Замовлення успішно доставлено" },
                new OrderStatus { Id = 5, Name = "Скасовано",      Description = "Замовлення скасовано" }
            );

            
            mb.Entity<DishCategory>().HasData(
                new DishCategory { Id = 1, Name = "Піца",      Description = "Класична та авторська піца з дров'яної печі" },
                new DishCategory { Id = 2, Name = "Бургери",   Description = "Соковиті бургери з яловичини та курки" },
                new DishCategory { Id = 3, Name = "Суші",      Description = "Свіжі роли та нігірі від шеф-кухаря" },
                new DishCategory { Id = 4, Name = "Десерти",   Description = "Авторські десерти та випічка" },
                new DishCategory { Id = 5, Name = "Напої",     Description = "Безалкогольні напої, коктейлі, кава" }
            );

            
            mb.Entity<Dish>().HasData(
                new Dish { Id=1,  Name="Піца «Маргарита»",       CategoryId=1, Price=180, Description="Томат, моцарела, базилік",                IsAvailable=true, CreatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc), UpdatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc) },
                new Dish { Id=2,  Name="Піца «Пепероні»",         CategoryId=1, Price=210, Description="Томат, моцарела, пепероні, перець чилі",   IsAvailable=true, CreatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc), UpdatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc) },
                new Dish { Id=3,  Name="Піца «Чотири сири»",      CategoryId=1, Price=240, Description="Моцарела, горгонзола, пармезан, рікота",   IsAvailable=true, CreatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc), UpdatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc) },
                new Dish { Id=4,  Name="CyberBurger Classic",     CategoryId=2, Price=195, Description="Яловичина, чедер, салат, томат, соус BBQ", IsAvailable=true, CreatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc), UpdatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc) },
                new Dish { Id=5,  Name="CyberBurger Spicy",       CategoryId=2, Price=215, Description="Яловичина, халапеньо, чедер, соус шрірача",IsAvailable=true, CreatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc), UpdatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc) },
                new Dish { Id=6,  Name="Рол «Каліфорнія»",        CategoryId=3, Price=165, Description="Краб, авокадо, огірок, ікра тобіко",       IsAvailable=true, CreatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc), UpdatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc) },
                new Dish { Id=7,  Name="Рол «Дракон»",            CategoryId=3, Price=195, Description="Краб, огірок, авокадо зверху, соус унагі",  IsAvailable=true, CreatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc), UpdatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc) },
                new Dish { Id=8,  Name="Чізкейк «Нью-Йорк»",      CategoryId=4, Price=120, Description="Вершковий сир, ванільний крем, пісочне тісто",IsAvailable=true,CreatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc), UpdatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc) },
                new Dish { Id=9,  Name="Тірамісу",                 CategoryId=4, Price=110, Description="Маскарпоне, савоярді, еспресо, какао",     IsAvailable=true, CreatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc), UpdatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc) },
                new Dish { Id=10, Name="Лимонад «Цитрус Mix»",     CategoryId=5, Price=75,  Description="Лимон, апельсин, м'ята, газована вода",     IsAvailable=true, CreatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc), UpdatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc) },
                new Dish { Id=11, Name="Еспресо",                  CategoryId=5, Price=55,  Description="100% арабіка, подвійний шот",               IsAvailable=true, CreatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc), UpdatedAt=new DateTime(2024,1,10,0,0,0,DateTimeKind.Utc) }
            );

           
            mb.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Адміністратор",
                    Email = "admin@cyberfork.ua",
                    PasswordHash = HashPassword("admin123"),
                    Role = "admin",
                    Phone = "+380501234567",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    Name = "Олена Коваль",
                    Email = "olena@example.com",
                    PasswordHash = HashPassword("user123"),
                    Role = "user",
                    Phone = "+380671234567",
                    Address = "вул. Хрещатик, 1, Київ",
                    CreatedAt = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 2, 15, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 3,
                    Name = "Михайло Бондаренко",
                    Email = "mykhailo@example.com",
                    PasswordHash = HashPassword("user123"),
                    Role = "user",
                    Phone = "+380631234567",
                    Address = "вул. Сагайдачного, 5, Київ",
                    CreatedAt = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            
            var baseDate = new DateTime(2024, 4, 1, 12, 0, 0, DateTimeKind.Utc);

            mb.Entity<Order>().HasData(
                new Order
                {
                    Id = 1, UserId = 2, StatusId = 4,
                    TotalAmount = 375, Comment = "Без цибулі, будь ласка",
                    CreatedAt = baseDate, UpdatedAt = baseDate.AddHours(1)
                },
                new Order
                {
                    Id = 2, UserId = null,
                    GuestName = "Тарас Шевченко", GuestPhone = "+380991112233",
                    GuestAddress = "вул. Золотоворітська, 3, Київ",
                    StatusId = 2, TotalAmount = 270, Comment = null,
                    CreatedAt = baseDate.AddDays(1), UpdatedAt = baseDate.AddDays(1).AddMinutes(20)
                },
                new Order
                {
                    Id = 3, UserId = 3, StatusId = 1,
                    TotalAmount = 580, Comment = "Доставити до 19:00",
                    CreatedAt = baseDate.AddDays(2), UpdatedAt = baseDate.AddDays(2)
                }
            );

            mb.Entity<OrderItem>().HasData(
                new OrderItem { Id=1, OrderId=1, DishId=1, Quantity=1, UnitPrice=180 },
                new OrderItem { Id=2, OrderId=1, DishId=4, Quantity=1, UnitPrice=195 },
                new OrderItem { Id=3, OrderId=2, DishId=6, Quantity=1, UnitPrice=165 },
                new OrderItem { Id=4, OrderId=2, DishId=7, Quantity=1, UnitPrice=195 }, 
                new OrderItem { Id=5, OrderId=3, DishId=2, Quantity=2, UnitPrice=210 },
                new OrderItem { Id=6, OrderId=3, DishId=5, Quantity=1, UnitPrice=215 },
                new OrderItem { Id=7, OrderId=3, DishId=8, Quantity=1, UnitPrice=120 }
            );

          
            mb.Entity<DishChangeLog>().HasData(
                new DishChangeLog
                {
                    Id = 1, DishId = 1,
                    ChangeDescription = "Ціну змінено з 160 на 180 грн",
                    ChangedBy = "admin",
                    ChangedAt = new DateTime(2024, 3, 5, 10, 0, 0, DateTimeKind.Utc)
                },
                new DishChangeLog
                {
                    Id = 2, DishId = 4,
                    ChangeDescription = "Оновлено опис: додано соус BBQ",
                    ChangedBy = "admin",
                    ChangedAt = new DateTime(2024, 3, 10, 14, 30, 0, DateTimeKind.Utc)
                }
            );
        }

       
        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
