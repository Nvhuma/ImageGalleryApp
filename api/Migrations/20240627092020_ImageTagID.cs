using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class ImageTagID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageTags",
                table: "ImageTags");

            migrationBuilder.AddColumn<int>(
                name: "ImageTagID",
                table: "ImageTags",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageTags",
                table: "ImageTags",
                column: "ImageTagID");

            migrationBuilder.CreateIndex(
                name: "IX_ImageTags_ImageId",
                table: "ImageTags",
                column: "ImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageTags",
                table: "ImageTags");

            migrationBuilder.DropIndex(
                name: "IX_ImageTags_ImageId",
                table: "ImageTags");

            migrationBuilder.DropColumn(
                name: "ImageTagID",
                table: "ImageTags");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageTags",
                table: "ImageTags",
                columns: new[] { "ImageId", "TagId" });
        }
    }
}
