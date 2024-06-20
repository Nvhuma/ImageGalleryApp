using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "images",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_images", x => x.ImageId);
                });

            migrationBuilder.CreateTable(
                name: "tag",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tag", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageId = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_comments_images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "images",
                        principalColumn: "ImageId");
                });

            migrationBuilder.CreateTable(
                name: "ImageTag",
                columns: table => new
                {
                    ImageId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageTag", x => new { x.ImageId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ImageTag_images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "images",
                        principalColumn: "ImageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImageTag_tag_TagId",
                        column: x => x.TagId,
                        principalTable: "tag",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comments_ImageId",
                table: "comments",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageTag_TagId",
                table: "ImageTag",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comments");

            migrationBuilder.DropTable(
                name: "ImageTag");

            migrationBuilder.DropTable(
                name: "images");

            migrationBuilder.DropTable(
                name: "tag");
        }
    }
}
