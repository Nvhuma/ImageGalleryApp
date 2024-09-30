using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPasswordHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "592a6469-4407-4049-9ec7-b0afc19da3d1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7c0c4390-345e-4e17-9314-0ba83b8d6d6d");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8a57514e-d0fc-44d4-a8d3-260bb3acba90", null, "Admin", "Admin" },
                    { "d7af7019-7062-4bcf-b56d-9b0071b66a38", null, "User", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a57514e-d0fc-44d4-a8d3-260bb3acba90");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d7af7019-7062-4bcf-b56d-9b0071b66a38");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "592a6469-4407-4049-9ec7-b0afc19da3d1", null, "Admin", "Admin" },
                    { "7c0c4390-345e-4e17-9314-0ba83b8d6d6d", null, "User", "User" }
                });
        }
    }
}
