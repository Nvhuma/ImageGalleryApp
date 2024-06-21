using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class Names : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comments_images_ImageId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageTag_images_ImageId",
                table: "ImageTag");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageTag_tag_TagId",
                table: "ImageTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_images",
                table: "images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_comments",
                table: "comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tag",
                table: "tag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageTag",
                table: "ImageTag");

            migrationBuilder.RenameTable(
                name: "images",
                newName: "Images");

            migrationBuilder.RenameTable(
                name: "comments",
                newName: "Comments");

            migrationBuilder.RenameTable(
                name: "tag",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "ImageTag",
                newName: "ImageTags");

            migrationBuilder.RenameIndex(
                name: "IX_comments_ImageId",
                table: "Comments",
                newName: "IX_Comments_ImageId");

            migrationBuilder.RenameIndex(
                name: "IX_ImageTag_TagId",
                table: "ImageTags",
                newName: "IX_ImageTags_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "ImageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageTags",
                table: "ImageTags",
                columns: new[] { "ImageId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Images_ImageId",
                table: "Comments",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageTags_Images_ImageId",
                table: "ImageTags",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "ImageId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageTags_Tags_TagId",
                table: "ImageTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Images_ImageId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageTags_Images_ImageId",
                table: "ImageTags");

            migrationBuilder.DropForeignKey(
                name: "FK_ImageTags_Tags_TagId",
                table: "ImageTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImageTags",
                table: "ImageTags");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "images");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "comments");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "tag");

            migrationBuilder.RenameTable(
                name: "ImageTags",
                newName: "ImageTag");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ImageId",
                table: "comments",
                newName: "IX_comments_ImageId");

            migrationBuilder.RenameIndex(
                name: "IX_ImageTags_TagId",
                table: "ImageTag",
                newName: "IX_ImageTag_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_images",
                table: "images",
                column: "ImageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_comments",
                table: "comments",
                column: "CommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tag",
                table: "tag",
                column: "TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImageTag",
                table: "ImageTag",
                columns: new[] { "ImageId", "TagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_comments_images_ImageId",
                table: "comments",
                column: "ImageId",
                principalTable: "images",
                principalColumn: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageTag_images_ImageId",
                table: "ImageTag",
                column: "ImageId",
                principalTable: "images",
                principalColumn: "ImageId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImageTag_tag_TagId",
                table: "ImageTag",
                column: "TagId",
                principalTable: "tag",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
