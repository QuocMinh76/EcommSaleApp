using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EcommSale.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateOrderPaymentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2b73d189-3047-4d8f-9e5b-f1d14dde4252");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9019c083-12fe-45a3-9bd1-e82852ad3960");

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a458f5ed-c9a6-4e52-83f0-e2b6a403da39", null, "User", "USER" },
                    { "daef5b8b-495f-4b8c-9529-219332fb6584", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a458f5ed-c9a6-4e52-83f0-e2b6a403da39");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "daef5b8b-495f-4b8c-9529-219332fb6584");

            migrationBuilder.DropColumn(
                name: "PaymentType",
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
    }
}
