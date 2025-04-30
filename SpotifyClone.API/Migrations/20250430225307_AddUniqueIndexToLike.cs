using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpotifyClone.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToLike : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_UserId",
                table: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId_SongId",
                table: "Likes",
                columns: new[] { "UserId", "SongId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_UserId_SongId",
                table: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");
        }
    }
}
