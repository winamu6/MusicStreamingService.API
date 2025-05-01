using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SpotifyClone.API.Migrations
{
    /// <inheritdoc />
    public partial class AddGenreModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genre",
                table: "Songs");

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "Songs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Songs_GenreId",
                table: "Songs",
                column: "GenreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Songs_Genres_GenreId",
                table: "Songs",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Songs_Genres_GenreId",
                table: "Songs");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Songs_GenreId",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "Songs");

            migrationBuilder.AddColumn<string>(
                name: "Genre",
                table: "Songs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
