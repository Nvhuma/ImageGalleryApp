using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddNamesToAspNetUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2da8a137-c8f1-4211-b2f3-906a1f1c2618");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ddff7312-d43d-4a75-a0c2-06a7169b880d");

            migrationBuilder.AlterColumn<string>(
                name: "Names",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "53754eb6-a29c-476f-9407-b31854fc66f7", null, "User", "User" },
                    { "e6716419-0c63-4a15-a213-b9b25e10644e", null, "Admin", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "53754eb6-a29c-476f-9407-b31854fc66f7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e6716419-0c63-4a15-a213-b9b25e10644e");

            migrationBuilder.AlterColumn<string>(
                name: "Names",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2da8a137-c8f1-4211-b2f3-906a1f1c2618", null, "User", "User" },
                    { "ddff7312-d43d-4a75-a0c2-06a7169b880d", null, "Admin", "Admin" }
                });
        }
    }
}
