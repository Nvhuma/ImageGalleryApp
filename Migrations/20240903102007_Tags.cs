using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class Tags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "37372130-7908-4a88-926f-7eb770d1886d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1b82534-4f65-41c7-baa9-c21259a3a662");

            migrationBuilder.AddColumn<int>(
                name: "TagName",
                table: "ImageTags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3c22e625-7f8f-4569-8ddd-1f3540f8f8e1", null, "Admin", "Admin" },
                    { "e90cfc72-739c-4509-a103-6211f0cbcc74", null, "User", "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_TagId",
                table: "Images",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Tags_TagId",
                table: "Images",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Tags_TagId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_TagId",
                table: "Images");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c22e625-7f8f-4569-8ddd-1f3540f8f8e1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e90cfc72-739c-4509-a103-6211f0cbcc74");

            migrationBuilder.DropColumn(
                name: "TagName",
                table: "ImageTags");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Images");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "37372130-7908-4a88-926f-7eb770d1886d", null, "Admin", "Admin" },
                    { "b1b82534-4f65-41c7-baa9-c21259a3a662", null, "User", "User" }
                });
        }
    }
}
