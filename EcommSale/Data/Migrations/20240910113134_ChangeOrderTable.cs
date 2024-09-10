using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EcommSale.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    { "2b73d189-3047-4d8f-9e5b-f1d14dde4252", null, "Admin", "ADMIN" },
                    { "9019c083-12fe-45a3-9bd1-e82852ad3960", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2b73d189-3047-4d8f-9e5b-f1d14dde4252");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9019c083-12fe-45a3-9bd1-e82852ad3960");

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
    }
}
