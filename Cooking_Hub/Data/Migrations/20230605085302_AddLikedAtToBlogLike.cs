using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cooking_Hub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLikedAtToBlogLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
        name: "LikedAt",
        table: "BlogLike",
        nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
       name: "LikedAt",
       table: "BlogLike");
        }
    }
}
