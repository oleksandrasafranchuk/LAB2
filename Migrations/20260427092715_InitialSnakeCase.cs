using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CyberFork.Migrations
{
    /// <inheritdoc />
    public partial class InitialSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dish_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dish_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order_statuses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_statuses", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    phone = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dishes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    is_available = table.Column<bool>(type: "boolean", nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dishes", x => x.id);
                    table.ForeignKey(
                        name: "fk_dishes_dish_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "dish_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    guest_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    guest_phone = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    guest_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_orders", x => x.id);
                    table.ForeignKey(
                        name: "fk_orders_order_statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "order_statuses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_orders_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "dish_change_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dish_id = table.Column<int>(type: "integer", nullable: false),
                    change_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    changed_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dish_change_logs", x => x.id);
                    table.ForeignKey(
                        name: "fk_dish_change_logs_dishes_dish_id",
                        column: x => x.dish_id,
                        principalTable: "dishes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order_id = table.Column<int>(type: "integer", nullable: false),
                    dish_id = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_order_items_dishes_dish_id",
                        column: x => x.dish_id,
                        principalTable: "dishes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_order_items_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "dish_categories",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { 1, "Класична та авторська піца з дров'яної печі", "Піца" },
                    { 2, "Соковиті бургери з яловичини та курки", "Бургери" },
                    { 3, "Свіжі роли та нігірі від шеф-кухаря", "Суші" },
                    { 4, "Авторські десерти та випічка", "Десерти" },
                    { 5, "Безалкогольні напої, коктейлі, кава", "Напої" }
                });

            migrationBuilder.InsertData(
                table: "order_statuses",
                columns: new[] { "id", "description", "name" },
                values: new object[,]
                {
                    { 1, "Замовлення щойно створено", "Нове" },
                    { 2, "Кухня приступила до приготування", "Готується" },
                    { 3, "Замовлення передано кур'єру", "Доставляється" },
                    { 4, "Замовлення успішно доставлено", "Виконано" },
                    { 5, "Замовлення скасовано", "Скасовано" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "address", "created_at", "email", "name", "password_hash", "phone", "role", "updated_at" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@cyberfork.ua", "Адміністратор", "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9", "+380501234567", "admin", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "вул. Хрещатик, 1, Київ", new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), "olena@example.com", "Олена Коваль", "e606e38b0d8c19b24cf0ee3808183162ea7cd63ff7912dbb22b5e803286b4446", "+380671234567", "user", new DateTime(2024, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "вул. Сагайдачного, 5, Київ", new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc), "mykhailo@example.com", "Михайло Бондаренко", "e606e38b0d8c19b24cf0ee3808183162ea7cd63ff7912dbb22b5e803286b4446", "+380631234567", "user", new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "dishes",
                columns: new[] { "id", "category_id", "created_at", "description", "image_url", "is_available", "name", "price", "updated_at" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Томат, моцарела, базилік", null, true, "Піца «Маргарита»", 180m, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 1, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Томат, моцарела, пепероні, перець чилі", null, true, "Піца «Пепероні»", 210m, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 1, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Моцарела, горгонзола, пармезан, рікота", null, true, "Піца «Чотири сири»", 240m, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 2, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Яловичина, чедер, салат, томат, соус BBQ", null, true, "CyberBurger Classic", 195m, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 2, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Яловичина, халапеньо, чедер, соус шрірача", null, true, "CyberBurger Spicy", 215m, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 3, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Краб, авокадо, огірок, ікра тобіко", null, true, "Рол «Каліфорнія»", 165m, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 3, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Краб, огірок, авокадо зверху, соус унагі", null, true, "Рол «Дракон»", 195m, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 4, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Вершковий сир, ванільний крем, пісочне тісто", null, true, "Чізкейк «Нью-Йорк»", 120m, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 4, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Маскарпоне, савоярді, еспресо, какао", null, true, "Тірамісу", 110m, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 5, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Лимон, апельсин, м'ята, газована вода", null, true, "Лимонад «Цитрус Mix»", 75m, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 5, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "100% арабіка, подвійний шот", null, true, "Еспресо", 55m, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "orders",
                columns: new[] { "id", "comment", "created_at", "guest_address", "guest_name", "guest_phone", "status_id", "total_amount", "updated_at", "user_id" },
                values: new object[,]
                {
                    { 1, "Без цибулі, будь ласка", new DateTime(2024, 4, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, null, null, 4, 375m, new DateTime(2024, 4, 1, 13, 0, 0, 0, DateTimeKind.Utc), 2 },
                    { 2, null, new DateTime(2024, 4, 2, 12, 0, 0, 0, DateTimeKind.Utc), "вул. Золотоворітська, 3, Київ", "Тарас Шевченко", "+380991112233", 2, 270m, new DateTime(2024, 4, 2, 12, 20, 0, 0, DateTimeKind.Utc), null },
                    { 3, "Доставити до 19:00", new DateTime(2024, 4, 3, 12, 0, 0, 0, DateTimeKind.Utc), null, null, null, 1, 580m, new DateTime(2024, 4, 3, 12, 0, 0, 0, DateTimeKind.Utc), 3 }
                });

            migrationBuilder.InsertData(
                table: "dish_change_logs",
                columns: new[] { "id", "change_description", "changed_at", "changed_by", "dish_id" },
                values: new object[,]
                {
                    { 1, "Ціну змінено з 160 на 180 грн", new DateTime(2024, 3, 5, 10, 0, 0, 0, DateTimeKind.Utc), "admin", 1 },
                    { 2, "Оновлено опис: додано соус BBQ", new DateTime(2024, 3, 10, 14, 30, 0, 0, DateTimeKind.Utc), "admin", 4 }
                });

            migrationBuilder.InsertData(
                table: "order_items",
                columns: new[] { "id", "dish_id", "order_id", "quantity", "unit_price" },
                values: new object[,]
                {
                    { 1, 1, 1, 1, 180m },
                    { 2, 4, 1, 1, 195m },
                    { 3, 6, 2, 1, 165m },
                    { 4, 7, 2, 1, 195m },
                    { 5, 2, 3, 2, 210m },
                    { 6, 5, 3, 1, 215m },
                    { 7, 8, 3, 1, 120m }
                });

            migrationBuilder.CreateIndex(
                name: "ix_dish_change_logs_dish_id",
                table: "dish_change_logs",
                column: "dish_id");

            migrationBuilder.CreateIndex(
                name: "ix_dishes_category_id",
                table: "dishes",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_items_dish_id",
                table: "order_items",
                column: "dish_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_items_order_id",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_created_at",
                table: "orders",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_orders_status_id",
                table: "orders",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_user_id",
                table: "orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dish_change_logs");

            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.DropTable(
                name: "dishes");

            migrationBuilder.DropTable(
                name: "orders");

            migrationBuilder.DropTable(
                name: "dish_categories");

            migrationBuilder.DropTable(
                name: "order_statuses");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
