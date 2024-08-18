using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EcommSale.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bafc5983-6e64-400b-9767-c1755bbf07b2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cda85bf5-5628-475d-9f77-43ae3f532689");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "538c7045-0043-40de-b0f4-c7bf0766c825", null, "User", "USER" },
                    { "8b805fc4-2e58-4bbc-b595-49a8298a1df5", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "538c7045-0043-40de-b0f4-c7bf0766c825");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8b805fc4-2e58-4bbc-b595-49a8298a1df5");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "Order");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "bafc5983-6e64-400b-9767-c1755bbf07b2", null, "Admin", "ADMIN" },
                    { "cda85bf5-5628-475d-9f77-43ae3f532689", null, "User", "USER" }
                });
        }
    }
}
